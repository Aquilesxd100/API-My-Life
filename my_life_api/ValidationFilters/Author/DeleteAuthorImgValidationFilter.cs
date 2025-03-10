﻿using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.ValidationFilters.Author;

public class DeleteAuthorImgValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        string authorId = GetParamValue("idAutor", context);

        if (string.IsNullOrEmpty(authorId)) {
            throw new CustomException(
                400, 
                "O param 'idAutor' é obrigatório e não foi informado."
            );
        }

        int convertedAuthorId = 0;

        try {
            convertedAuthorId = Int32.Parse(authorId);
        } catch (Exception exception) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        if (convertedAuthorId < 0) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        AuthorDTO? author = null;

        AuthorDBManager authorDbManager = new AuthorDBManager();
        author = await authorDbManager.GetAuthorById(convertedAuthorId);

        if (author == null) {
            throw new CustomException(404, "Nenhum autor com esse id foi encontrado.");
        }

        if (String.IsNullOrEmpty(author.urlImagem)) {
            throw new CustomException(400, "Esse autor não tem imagem registrada.");
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(author)
        );

        await next();
    }
}
