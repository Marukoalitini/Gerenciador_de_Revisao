using Motos.Services;

namespace Motos.Workers;

public class VerificadorRevisaoWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VerificadorRevisaoWorker> _logger;

    public VerificadorRevisaoWorker(IServiceProvider serviceProvider, ILogger<VerificadorRevisaoWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando verificação de revisões próximas...");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var agendamentoService = scope.ServiceProvider.GetRequiredService<AgendamentoService>();
                    var notificacaoService = scope.ServiceProvider.GetRequiredService<INotificacaoService>();
                    var motoService = scope.ServiceProvider.GetRequiredService<MotoService>();

                    // 1. Obter todas as motos ativas (incluindo dados do cliente e revisões)
                    var motos = await motoService.ListarMotosAtivasParaWorkerAsync(stoppingToken);

                    foreach (var moto in motos)
                    {
                        var previsao = agendamentoService.CalcularPrevisao(moto);
                        if (previsao == null) continue;

                        // 5. Lógica de Notificação (ex: avisar se faltar entre 1 e 30 dias)
                        if (previsao.DiasRestantes > 0 && previsao.DiasRestantes <= 30)
                        {
                            _logger.LogInformation(
                                $"Moto {moto.Placa} precisa da revisão {previsao.Numero} em {previsao.DataPrevista:d}");
                            
                            await notificacaoService.EnviarNotificacaoRevisaoAsync(
                                moto.Cliente.Email,
                                moto.Placa,
                                previsao.Numero,
                                previsao.DataPrevista
                            );
                        }
                        else if (previsao.Atrasada)
                        {
                            _logger.LogWarning(
                                $"Moto {moto.Placa} está com a revisão {previsao.Numero} ATRASADA! Data prevista era {previsao.DataPrevista:d}");

                            await notificacaoService.EnviarNotificacaoRevisaoAsync(
                                moto.Cliente.Email,
                                moto.Placa,
                                previsao.Numero,
                                previsao.DataPrevista,
                                true
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar revisões.");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
