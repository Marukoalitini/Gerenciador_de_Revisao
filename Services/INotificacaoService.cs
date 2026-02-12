namespace Motos.Services;

public interface INotificacaoService
{
    Task EnviarNotificacaoRevisaoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateOnly dataPrevista, bool atrasada = false);

    Task EnviarNotificacaoStatusAgendamentoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateOnly? dataAgendada, bool aceite);
}