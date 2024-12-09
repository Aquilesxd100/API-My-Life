using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Models.Requests.Record;
using my_life_api.ValidatorsFilters.Security;
using my_life_api.ValidatorsFilters.Record;
using my_life_api.Models;

namespace my_life_api.Controllers;

[ApiController]
public class RecordController : ControllerBase {

    private readonly ILogger<RecordController> _logger;

    public RecordController(ILogger<RecordController> logger) {
        _logger = logger;
    }

    [HttpGet("registros", Name = "registros")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    public async Task<IActionResult> GetRegistros() {
        RecordService service = new RecordService();

        IEnumerable<RecordDTO> records = await service.GetBasicRecords();

        return Ok(ApiResponse.CreateBody(
            200,
            "Lista de registros simplificados recebida com sucesso!",
            new { registros = records }
        ));
    }

    [HttpGet("registro", Name = "registro")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(GetRecordValidationFilter))]
    public async Task<IActionResult> GetRegistro([FromQuery] string idRegistro) {
        RecordDTO dbRecord = JsonConvert.DeserializeObject<RecordDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        return Ok(ApiResponse.CreateBody(
            200,
            "Registro recebido com sucesso!",
            new { registro = dbRecord }
        ));
    }

    [HttpPost("registro", Name = "registro")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(CreateRecordValidationFilter))]
    public async Task<IActionResult> Post(
        [FromForm] string nome,
        [FromForm] string ano,
        [FromForm] string conteudo,
        [FromForm] IFormFile imagemPrincipal,
        [FromForm] IEnumerable<IFormFile> imagensSecundarias
    ) {
        RecordService service = new RecordService();

        RecordCreateRequestDTO recordReq = new RecordCreateRequestDTO {
            nome = nome,
            ano = ano,
            conteudo = conteudo,
            imagemPrincipal = imagemPrincipal,
            imagensSecundarias = imagensSecundarias
        };

        await service.CreateRecord(recordReq);

        return Ok(ApiResponse.CreateBody(201, "Registro criado com sucesso!"));
    }

    [HttpPut("registro", Name = "registro")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(UpdateRecordValidationFilter))]
    public async Task<IActionResult> Put(
        [FromForm] int id,
        [FromForm] string nome,
        [FromForm] string ano,
        [FromForm] string conteudo,
        [FromForm] IFormFile imagemPrincipal
    ) {
        RecordService service = new RecordService();

        RecordUpdateRequestDTO recordReq = new RecordUpdateRequestDTO {
            id = id,
            nome = nome,
            ano = ano,
            conteudo = conteudo,
            imagemPrincipal = imagemPrincipal
        };

        RecordDTO dbRecord = JsonConvert.DeserializeObject<RecordDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.UpdateRecord(recordReq, dbRecord);

        return Ok(ApiResponse.CreateBody(200, "Registro atualizado com sucesso!"));
    }

    [HttpDelete("registro", Name = "registro")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteRecordValidationFilter))]
    public async Task<IActionResult> Delete(
        [FromQuery] string idRegistro
    ) {
        RecordService service = new RecordService();

        await service.DeleteRecordById(
            Int32.Parse(idRegistro)
        );

        return Ok(ApiResponse.CreateBody(200, "O registro foi excluído com sucesso!"));
    }

    [HttpDelete("registroImgPrincipal", Name = "registroImgPrincipal")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteRecordMainImgValidationFilter))]
    public async Task<IActionResult> DeleteImgPrincipal(
        [FromQuery] string idRegistro
    ) {
        RecordService service = new RecordService();

        RecordDTO dbRecord = JsonConvert.DeserializeObject<RecordDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.DeleteRecordMainImg(dbRecord);

        return Ok(ApiResponse.CreateBody(200, "A imagem principal do registro foi excluída com sucesso!"));
    }

    [HttpPost("registroImgSecundaria", Name = "registroImgSecundaria")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(AddRecordSecondaryImgValidationFilter))]
    public async Task<IActionResult> PostSecondaryImg(
        [FromForm] int idRegistro,
        [FromForm] IFormFile imagemSecundaria
    ) {
        RecordService service = new RecordService();

        RecordDTO dbRecord = JsonConvert.DeserializeObject<RecordDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.AddRecordSecondaryImg((int)dbRecord.id, imagemSecundaria);

        return Ok(ApiResponse.CreateBody(201, "Imagem secundária adicionada ao registro!"));
    }

    [HttpDelete("registroImgSecundaria", Name = "registroImgSecundaria")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteRecordSecondaryImgValidationFilter))]
    public async Task<IActionResult> DeleteSecondaryImg(
        [FromQuery] int idRegistro,
        [FromQuery] string idImagemSecundaria        
    ) {
        RecordService service = new RecordService();

        SecondaryImgDTO dbImg = JsonConvert.DeserializeObject<SecondaryImgDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.DeleteRecordSecondaryImg(
            idRegistro, 
            dbImg
        );

        return Ok(ApiResponse.CreateBody(200, "Imagem secundária removida do registro!"));
    }
}
