using my_life_api.Database.Managers;
using my_life_api.Models;

namespace my_life_api.Shared
{
    public class ContentValidator
    {
        public void ValidateName(string name, bool isRequired = false) {
            if (isRequired) {
                if (string.IsNullOrEmpty(name) || name.Trim().Length == 0)
                {
                    throw new CustomException(400, "O nome é obrigatório e não pode ficar vazio.");
                }
            }

            if (!string.IsNullOrEmpty(name)) {
                if (name.Trim().Length == 0) {
                    throw new CustomException(400, "O nome não pode ser vazio.");
                }

                if (name.Length > 50) {
                    throw new CustomException(400, "O nome deve ter no máximo 50 caracteres.");
                }

                if (Validator.HasInvalidCharacters(name)) {
                    throw new CustomException(400, "O nome contém caracteres inválidos.");
                }
            }
        }

        public void ValidateOptionalImgFile(IFormFile? file) {
            if (file != null) {
                if (Validator.IsInvalidImageFormat(file)) {
                    throw new CustomException(
                        400,
                        "A imagem enviada está em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                    );
                }

                if (Validator.IsInvalidImageSize(file)) {
                    throw new CustomException(400, "A imagem enviada excede o tamanho máximo de 12mb.");
                }
            }
        }

        public async Task ValidateContentAuthor(int authorId, ContentTypesEnum contentType) {
            dynamic? authorDbData = null;

            if (authorId > 0) {
                AuthorDBManager authorDbManager = new AuthorDBManager();
                authorDbData = await authorDbManager.GetAuthorById(authorId);

                if (authorDbData == null) {
                    throw new CustomException(404, "Não existe nenhum autor com esse id.");
                }

                if (authorDbData.idTipoConteudo != contentType) {
                    throw new CustomException(400, "Esse autor não é válido para esse tipo de conteúdo.");
                }
            } else {
                throw new CustomException(
                    400,
                    "O id de autor informado é inválido."
                );
            }            
        }

        public void ValidateRating(float? rating) {
            if (rating != null) {
                if (Validator.IsRatingInvalid((float)rating)) {
                    throw new CustomException(400, "A nota informada é inválida, o valor deve ser entre 0 e 10.");
                }
            }
        }
    }
}
