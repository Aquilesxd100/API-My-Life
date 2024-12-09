using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Models.Requests.Record;
using my_life_api.Resources;
using my_life_api.Shared;

namespace my_life_api.ValidatorsFilters.Record;

public class CreateRecordValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        var recordObj = await GetFormDataContent<RecordCreateRequestDTO>(context);

        RecordCreateRequestDTO record = new RecordCreateRequestDTO().BuildFromObj(recordObj);

        if (string.IsNullOrEmpty(record.ano) || record.ano.Trim().Length == 0) {
            throw new CustomException(
                400, 
                "O ano do registro é obrigatório e não pode ficar vazio."
            );
        }

        if (record.ano.Length > 4) {
            throw new CustomException(400, "O ano do registro está em formato incorreto.");
        }

        int convertedYear = 0;
        try {
            convertedYear = int.Parse(record.ano);
        } catch (Exception ex) {
            throw new CustomException(400, "O ano do registro está em formato incorreto.");
        }

        if (convertedYear < 1800 || convertedYear > 2300) {
            throw new CustomException(400, "O ano do registro é inválido.");
        }

        if (Validator.HasInvalidCharacters(record.ano)) {
            throw new CustomException(400, "O ano do registro contém caracteres inválidos.");
        }

        ContentValidator validator = new ContentValidator();
        validator.ValidateName(record.nome, true);
        validator.ValidateOptionalImgFile(record.imagemPrincipal);

        if (string.IsNullOrEmpty(record.conteudo) || record.conteudo.Trim().Length == 0) {
            throw new CustomException(
                400, 
                "O conteúdo do registro é obrigatório e não pode ficar vazio."
            );
        }

        if (record.conteudo.Length > 3500) {
            throw new CustomException(
                400, 
                "O conteúdo do registro deve ter no máximo 3500 caracteres."
            );
        }

        if (Validator.HasInvalidCharacters(record.conteudo)) {
            throw new CustomException(
                400, 
                "O conteúdo do registro contém caracteres inválidos."
            );
        }

        if (record.imagensSecundarias.Count() > 0) { 
            if (record.imagensSecundarias.Count() > 8) {
                throw new CustomException(
                    400,
                    "São permitidas no máximo 8 imagens secundarias por registro."
                );
            }

            foreach(IFormFile img in record.imagensSecundarias) {
                if (Validator.IsInvalidImageFormat(img)) { 
                    throw new CustomException(
                        400,
                        "Uma ou mais imagens secundarias estão em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                    );
                }

                if (Validator.IsInvalidImageSize(img)) { 
                    throw new CustomException(
                        400,
                        "Uma ou mais imagens secundarias excedem o tamanho máximo de 12mb."
                    );
                }
            }
        }

        await next();
    }
}
