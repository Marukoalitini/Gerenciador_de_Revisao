using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Motos.Exceptions;

namespace Motos.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception excecao,
        CancellationToken cancellationToken)
    {
        _logger.LogError(excecao, "Ocorreu um erro: {Message}", excecao.Message);

        var detalhesProblema = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (excecao)
        {
            case BadRequestException excecaoDominio:
                detalhesProblema.Title = "Erro de Validação";
                detalhesProblema.Status = StatusCodes.Status400BadRequest;
                detalhesProblema.Detail = excecaoDominio.Message;
                break;
            case NotFoundException excecaoNaoEncontrado:
                detalhesProblema.Title = "Recurso Não Encontrado";
                detalhesProblema.Status = StatusCodes.Status404NotFound;
                detalhesProblema.Detail = excecaoNaoEncontrado.Message;
                break;
            case ConflictException excecaoConflito:
                detalhesProblema.Title = "Conflito de Dados";
                detalhesProblema.Status = StatusCodes.Status409Conflict;
                detalhesProblema.Detail = excecaoConflito.Message;
                break;
            case UnauthorizedAccessException:
                detalhesProblema.Title = "Não Autorizado";
                detalhesProblema.Status = StatusCodes.Status401Unauthorized;
                detalhesProblema.Detail = excecao.Message;
                break;
            default:
                detalhesProblema.Title = "Erro Interno do Servidor";
                detalhesProblema.Status = StatusCodes.Status500InternalServerError;
                detalhesProblema.Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
                break;
        }

        httpContext.Response.StatusCode = detalhesProblema.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(detalhesProblema, cancellationToken);

        return true;
    }
}