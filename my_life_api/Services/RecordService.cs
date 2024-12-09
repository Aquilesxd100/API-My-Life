using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Record;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class RecordService
    {
        private RecordDBManager dbManager = new RecordDBManager();

        public async Task<IEnumerable<RecordDTO>> GetBasicRecords()
        {
            IEnumerable<RecordDTO> records = await dbManager.GetBasicRecords();

            return records;
        }

        public async Task CreateRecord(RecordCreateRequestDTO recordReq)
        {
            RecordDTO record = new RecordDTO
            {
                nome = recordReq.nome,
                ano = recordReq.ano,
                conteudo = recordReq.conteudo,             
            };

            int recordId = await dbManager.CreateRecord(record);
            record.id = recordId;

           string folderPath = $"{FtpManager.recordPicturesFolder}/{recordId}";
           await FtpManager.CreateFolder(folderPath);

            if (recordReq.imagemPrincipal != null) {
                string imageUrl = await FtpManager.UploadRecordMainPicture(
                    recordId, 
                    recordReq.imagemPrincipal
                );
                record.urlImagemPrincipal = imageUrl;

                await dbManager.UpdateRecord(record);
            }

            if (recordReq.imagensSecundarias.Count() > 0) {
                IFormFile[] secondaryImgsFiles = recordReq.imagensSecundarias.ToArray();
                List<SecondaryImgToSave> secondaryImgsToSave = new();

                foreach(IFormFile imgFile in secondaryImgsFiles) {
                    Guid myuuid = Guid.NewGuid();
                    string imgId = recordId + "-" + myuuid.ToString();

                    string secondaryImgUrl = await FtpManager.UploadRecordSecondaryPicture(
                        recordId,
                        imgId,
                        imgFile
                    );

                    secondaryImgsToSave.Add(new SecondaryImgToSave() {
                        id  = imgId,
                        url = secondaryImgUrl
                    });
                }

                await dbManager.CreateSecondaryImagesRelations(
                    recordId,
                    secondaryImgsToSave
                );
            }
        }

        public async Task UpdateRecord(RecordUpdateRequestDTO recordReq, RecordDTO dbRecord)
        {
            RecordDTO record = new RecordDTO
            {
                id = dbRecord.id,
                nome = dbRecord.nome,
                ano = dbRecord.ano,
                conteudo = dbRecord.conteudo,
                urlImagemPrincipal = dbRecord.urlImagemPrincipal
            };

            if (recordReq.imagemPrincipal != null) {
                string imageUrl = await FtpManager.UploadRecordMainPicture(
                    (int)recordReq.id, 
                    recordReq.imagemPrincipal
                );
                record.urlImagemPrincipal = imageUrl;
            }

            if (!string.IsNullOrEmpty(recordReq.nome)) {
                record.nome = recordReq.nome;
            }

            if (!string.IsNullOrEmpty(recordReq.ano)) {
                record.ano = recordReq.ano;
            }
            
            if (!string.IsNullOrEmpty(recordReq.conteudo)) {
                record.conteudo = recordReq.conteudo;
            }

            await dbManager.UpdateRecord(record);
        }

        public async Task DeleteRecordById(int recordId) {
            await dbManager.DeleteRecordById(recordId);

            string folderPath = $"{FtpManager.recordPicturesFolder}/{recordId}";
            await FtpManager.DeleteFolder(folderPath);
        }

        public async Task DeleteRecordMainImg(RecordDTO record) {        
            await FtpManager.DeleteFile(
                FtpManager.GetImageNameFromUrl(record.urlImagemPrincipal), 
                FtpManager.recordPicturesFolder + "/" + record.id
            );

            record.urlImagemPrincipal = null;
            await dbManager.UpdateRecord(record);
        }

        public async Task AddRecordSecondaryImg(
            int recordId,
            IFormFile secondaryImg
        ) {
            Guid myuuid = Guid.NewGuid();
            string imgId = recordId + "-" + myuuid.ToString();

            string imageUrl = await FtpManager.UploadRecordSecondaryPicture(
                recordId, 
                imgId,
                secondaryImg
            );

            await dbManager.CreateSecondaryImageRelation(
                recordId,
                new SecondaryImgToSave() { 
                    id = imgId, 
                    url = imageUrl 
                }
            );    
        }

        public async Task DeleteRecordSecondaryImg(
            int recordId,
            SecondaryImgDTO img
        ) {
            await FtpManager.DeleteFile(
                FtpManager.GetImageNameFromUrl(img.urlImagem), 
                FtpManager.recordPicturesFolder + "/" + recordId
            );

            await dbManager.DeleteSecondaryImageRelation(recordId, img.id);
        }
    }
}
