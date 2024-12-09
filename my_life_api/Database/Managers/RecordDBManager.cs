using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Resources;
using my_life_api.Models;

namespace my_life_api.Database.Managers;

public class SecondaryImgToSave {
    public string id { get; set; }
    public string url { get; set; }
}

public class RecordDBManager : BaseDBManager {
    public async Task<IEnumerable<RecordDTO>> GetBasicRecords() {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = "Select id, createdAt, name, year, mainImageUrl From Records;";

        List<RecordDTO> records = new List<RecordDTO>();
        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            RecordDTO recordToAdd = new RecordDTO() {
                id = myReader.GetInt32("id"),
                dataCriacao = myReader.GetDateTime("createdAt"),
                nome = myReader.GetString("name"),
                ano = myReader.GetInt16("year").ToString(),
                urlImagemPrincipal = 
                    myReader.IsDBNull("mainImageUrl") 
                        ? null 
                        : myReader.GetString("mainImageUrl"),
            };
            records.Add(recordToAdd);
        }

        await DataBase.CloseConnection();

        return records;
    }

    public async Task<RecordDTO?> GetRecordById(int recordId) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = "Select " +
                "id, createdAt, year, name, content, mainImageUrl " +
                "From Records " +
                $"Where id = {recordId};";

        RecordDTO record = null;
        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            record = new RecordDTO() {
                id = myReader.GetInt32("id"),
                dataCriacao = myReader.GetDateTime("createdAt"),
                nome = myReader.GetString("name"),
                ano = myReader.GetInt16("year").ToString(),
                conteudo = myReader.GetString("content"),
                urlImagemPrincipal = 
                    myReader.IsDBNull("mainImageUrl") 
                        ? null 
                        : myReader.GetString("mainImageUrl"),
            };
        }

        await DataBase.CloseConnection();

        if (record != null) {
            await DataBase.OpenConnectionIfClosed();

            List<SecondaryImgDTO> secondaryImgs = new List<SecondaryImgDTO>();

            myCommand.CommandText = "Select " +
                "id, imageUrl, recordId " +
                "From SecondaryImages " +
                $"Where recordId = {recordId};";

            using var imgsReader = await myCommand.ExecuteReaderAsync();

            while (imgsReader.Read()) {
                secondaryImgs.Add(new SecondaryImgDTO() {
                    id = imgsReader.GetString("id"),
                    idRegistro = imgsReader.GetInt32("recordId"),
                    urlImagem = imgsReader.GetString("imageUrl")
                });
            }

            record.imagensSecundarias = secondaryImgs;

            await DataBase.CloseConnection();
        }

        return record;
    }

    public async Task<int> CreateRecord(RecordDTO record) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            "Insert Into Records" +
                "(name, year, content)" +
                "Values" +
                    $"('{record.nome}', '{record.ano}', '{record.conteudo}');"
            + "Select Last_Insert_Id();";

        var result = await myCommand.ExecuteScalarAsync();
        int recordId = int.Parse(result.ToString());

        await DataBase.CloseConnection();

        return recordId;
    }

    public async Task UpdateRecord(RecordDTO record) {
        await DataBase.OpenConnectionIfClosed();

        string treatedUrlImage = record.urlImagemPrincipal != null
            ? $"'{record.urlImagemPrincipal}'"
            : "NULL";

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            "Update Records " +
                "Set " +
                    $"name = '{record.nome}'," +
                    $"year = '{record.ano}'," +
                    $"content = '{record.conteudo}'," +
                    $"mainImageUrl = {treatedUrlImage} " +
            $"Where id = {record.id};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task DeleteRecordById(int recordId) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            "Delete From SecondaryImages " +
                $"Where recordId = {recordId};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
        await DataBase.OpenConnectionIfClosed();

        myCommand.CommandText =
            "Delete From Records " +
                $"Where id = {recordId};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task CreateSecondaryImagesRelations(
        int recordId,
        IEnumerable<SecondaryImgToSave> imgsDataToSave
    ) {
        IEnumerable<string> sqlImgsToInsert =
            imgsDataToSave.Select((imgData) =>
                $"('{imgData.id}', '{imgData.url}', {recordId})"
            );

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            "Insert Into SecondaryImages " +
                "(id, imageUrl, recordId) " +
                "Values " +
                    String.Join(",", sqlImgsToInsert) +
            ";";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task CreateSecondaryImageRelation(
        int recordId,
        SecondaryImgToSave imgDataToSave
    ) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            "Insert Into SecondaryImages " +
                "(id, imageUrl, recordId) " +
                "Values " +
                    $"('{imgDataToSave.id}', '{imgDataToSave.url}', {recordId});";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task DeleteSecondaryImageRelation(
        int recordId,
        string imgId
    ) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            "Delete from SecondaryImages " +
                $"Where recordId = {recordId} And id = '{imgId}';";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }
}
