namespace Motos.Services;

public interface INotificacaoService
{
    Task EnviarNotificacaoRevisaoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateTime dataPrevista, bool atrasada = false);
}