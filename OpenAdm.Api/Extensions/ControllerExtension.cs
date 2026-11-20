using Microsoft.AspNetCore.Mvc;
using OpenAdm.Application.Dtos.Response;
using OpenAdm.Domain.Enuns;
using OpenAdm.Domain.Model;

namespace OpenAdm.Api.Extensions;

public static class ControllerExtension
{
    public static IResult ToIResult<T>(this Resultado<T> resultado) where T : class
    {
        return resultado switch
        {
            T value => Results.Ok(value),
            ErroResultado erro => erro.Tipo switch
            {
                TipoErroEnum.BadRequest => Results.BadRequest(erro),
                TipoErroEnum.NotFound => Results.NotFound(erro),
                TipoErroEnum.InternalServerError => Results.InternalServerError(erro),
                _ => Results.InternalServerError(new ErroResultado
                {
                    Tipo = TipoErroEnum.InternalServerError,
                    Mensagem = "Erro desconhecido."
                })
            },
        };
    }

    public static IActionResult ToActionResult<T>(this ResultPartner<T> result)
    {
        if (!string.IsNullOrWhiteSpace(result.Error))
        {
            return new BadRequestObjectResult(new ErrorResponse()
            {
                Mensagem = result.Error,
            });
        }

        return new OkObjectResult(result.Result);
    }

    public static IResult ToActionResults<T>(this ResultPartner<T> result)
    {
        if (!string.IsNullOrWhiteSpace(result.Error))
        {
            return Results.BadRequest(new ErrorResponse()
            {
                Mensagem = result.Error,
            });
        }

        return Results.Ok(result.Result);
    }
}