﻿using BackendTestTask.AspNetExtensions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackendTestTask.Services.Services.Interfaces;
using BackendTestTask.Database.Entities;

namespace BackendTestTask.AspNetExtensions.Filters
{
    public class ResponseModelFilter : IAsyncAlwaysRunResultFilter
    {
        private static readonly List<int> SuccessCodes = new() { StatusCodes.Status200OK, StatusCodes.Status201Created, StatusCodes.Status204NoContent };
        private readonly ILogger<ExceptionHandlerAttribute> _logger;
        public ResponseModelFilter(ILogger<ExceptionHandlerAttribute> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var statusCode = context.HttpContext.Response.StatusCode;
            var method = context.HttpContext.Request.Method;
            var result = context.Result as ObjectResult;

            if (context.ModelState.IsValid == false)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors);

                var errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));

                var data = new Dictionary<string, string>() { { "message", "Invalid request, problem -> " + errorMessage } };
                                
                if (context.HttpContext.Request.ContentLength > 0 && context.HttpContext.Request.Body.CanSeek)
                {
                    // something
                }

                _logger.LogError("Request has failed.");

            }
            else if (SuccessCodes.Contains(statusCode) && result != null)
            {
                var model = result.Value;

                if (!((HttpMethods.IsGet(method) || HttpMethods.IsPut(method) || HttpMethods.IsPatch(method))
                     && model == null))
                {
                    context.Result = new OkObjectResult(model);
                }
            }
            else if (SuccessCodes.Contains(statusCode) && result == null)
            {
                if (HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method))
                {
                    context.Result = new NoContentResult();
                }
            }

            await next();
        }
    }
}
