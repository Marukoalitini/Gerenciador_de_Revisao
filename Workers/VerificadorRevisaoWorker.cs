using Microsoft.EntityFrameworkCore;
using Motos.Data;
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
                    // Resolvemos os serviços dentro do escopo pois o Worker é Singleton
                    // e o DbContext/MotoService geralmente são Scoped.
                    var motoService = scope.ServiceProvider.GetRequiredService<MotoService>();
                    var regraService = scope.ServiceProvider.GetRequiredService<RegraRevisaoService>();
                    var notificacaoService = scope.ServiceProvider.GetRequiredService<INotificacaoService>();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 1. Obter todas as motos ativas (incluindo dados do cliente para o email)
                    // Nota: Idealmente usar paginação se houver milhares de motos.
                    var motos = await context.Motos
                        .Include(m => m.Cliente)
                        .Where(m => m.Ativo && m.Cliente.Ativo)
                        .ToListAsync(stoppingToken);

                    foreach (var moto in motos)
                    {
                        // 2. Descobrir qual é a próxima revisão
                        var ultimaRevisao = motoService.ObterUltimaRevisaoExecutada(moto.Id);
                        
                        int proximaRevisaoNum;
                        DateTime dataBase;
                        int mesesParaAdicionar;

                        if (ultimaRevisao != null)
                        {
                            // Se já houve revisão, calculamos a próxima baseada na diferença de meses
                            proximaRevisaoNum = ultimaRevisao.Numero + 1;
                            var regraUltima = regraService.ObterPorNumero(ultimaRevisao.Numero);
                            var regraProxima = regraService.ObterPorNumero(proximaRevisaoNum);

                            if (regraUltima == null || regraProxima == null || !ultimaRevisao.DataExecucao.HasValue)
                                continue;

                            dataBase = ultimaRevisao.DataExecucao.Value;
                            mesesParaAdicionar = regraProxima.MesesAlvo - regraUltima.MesesAlvo;
                        }
                        else
                        {
                            // Se nunca houve revisão, a base é a data de compra (1ª revisão)
                            proximaRevisaoNum = 1;
                            var regraProxima = regraService.ObterPorNumero(proximaRevisaoNum);
                            
                            if (regraProxima == null) continue;

                            // Assumindo que a entidade Moto tem a propriedade DataCompra
                            dataBase = moto.DataDeVenda; 
                            mesesParaAdicionar = regraProxima.MesesAlvo;
                        }

                        DateTime dataPrevista = dataBase.AddMonths(mesesParaAdicionar);

                        // Verifica quantos dias faltam
                        var diasRestantes = (dataPrevista - DateTime.Now).TotalDays;

                        // 5. Lógica de Notificação (ex: avisar se faltar entre 1 e 30 dias)
                        if (diasRestantes > 0 && diasRestantes <= 30)
                        {
                            _logger.LogInformation(
                                $"Moto {moto.Placa} precisa da revisão {proximaRevisaoNum} em {dataPrevista:d}");
                            
                            await notificacaoService.EnviarNotificacaoRevisaoAsync(
                                moto.Cliente.Email,
                                moto.Placa,
                                proximaRevisaoNum,
                                dataPrevista
                            );
                        }
                        else if (diasRestantes < 0)
                        {
                            _logger.LogWarning(
                                $"Moto {moto.Placa} está com a revisão {proximaRevisaoNum} ATRASADA! Data prevista era {dataPrevista:d}");

                            await notificacaoService.EnviarNotificacaoRevisaoAsync(
                                moto.Cliente.Email,
                                moto.Placa,
                                proximaRevisaoNum,
                                dataPrevista,
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
            // Para testes, você pode diminuir esse tempo.
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}