using FluentReflections;
using FluentValidation;
using System;
using System.Globalization;
using Iotc.SharedKernel.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace TestingExample
{
    public class DomainExceptionFilter : IExceptionFilter
    {
        public void OnException(
            ExceptionContext context)
        {
            if (context.Exception is not DomainBaseException domainException)
            {
                return;
            }

            var resultProblemDetails = new ProblemDetails
            {
                Detail = domainException.ErrorDescription,
                Type = ((int)domainException.ErrorCode).ToString(CultureInfo.InvariantCulture),
            };

            context.Result = domainException switch
            {
                NotFoundException _ => new NotFoundObjectResult(resultProblemDetails),
                DomainErrorException _ => new UnprocessableEntityObjectResult(resultProblemDetails),
                DomainDataException _ => new BadRequestObjectResult(resultProblemDetails),
                _ => throw new InvalidOperationException(
                    $"Unknown domain exception. Add this exception to {nameof(DomainExceptionFilter)}",
                    domainException),
            };
            context.ExceptionHandled = true;
        }
    }
}
