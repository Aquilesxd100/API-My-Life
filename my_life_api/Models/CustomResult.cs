﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace my_life_api.Models
{
    public class CustomResult : ObjectResult
    {
        public CustomResult(int statusCode, string message, object content = null) 
            : base(
                  content == null
                    ? new { codigo = statusCode, mensagem = message }
                    : new { codigo = statusCode, mensagem = message, conteudo = content }
            )
        {
            StatusCode = statusCode;
        }
    }
}