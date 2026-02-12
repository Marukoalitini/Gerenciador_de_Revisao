namespace Motos.Services;

public interface INotificacaoService
{
    Task EnviarNotificacaoRevisaoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateTime dataPrevista, bool atrasada = false);

    Task EnviarNotificacaoStatusAgendamentoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateTime? dataAgendada, bool aceite);
}