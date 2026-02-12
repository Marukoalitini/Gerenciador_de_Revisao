using System.Net;
using System.Net.Mail;

namespace Motos.Services;

public class EmailNotificacaoService : INotificacaoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailNotificacaoService> _logger;

    public EmailNotificacaoService(IConfiguration configuration, ILogger<EmailNotificacaoService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnviarNotificacaoRevisaoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateOnly dataPrevista, bool atrasada = false)
    {
        try
        {
            var host = _configuration["Smtp:Host"];
            var portStr = _configuration["Smtp:Port"];
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:From"] ?? "no-reply@oficina.com";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Configurações de SMTP incompletas. E-mail não enviado.");
                return;
            }

            using var client = new SmtpClient(host, int.Parse(portStr ?? "587"))
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            string subject;
            string body;

            if (atrasada)
            {
                subject = $"ALERTA: Revisão Atrasada - Moto {placaMoto}";
                body = $"Olá,\n\nSua moto placa {placaMoto} está com a revisão nº {numeroRevisao} ATRASADA.\n" +
                       $"A data prevista era: {dataPrevista:dd/MM/yyyy}.\n\n" +
                       $"Por favor, entre em contato urgentemente para agendar!";
            }
            else
            {
                subject = $"Lembrete de Revisão - Moto {placaMoto}";
                body = $"Olá,\n\nSua moto placa {placaMoto} está próxima da revisão nº {numeroRevisao}.\n" +
                       $"Data prevista: {dataPrevista:dd/MM/yyyy}.\n\n" +
                       $"Entre em contato para agendar!";
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(emailCliente);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"E-mail de notificação enviado para {emailCliente} - Moto {placaMoto}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar e-mail para {emailCliente}");
        }
    }

    public async Task EnviarNotificacaoStatusAgendamentoAsync(string emailCliente, string placaMoto, int numeroRevisao, DateOnly? dataAgendada, bool aceite)
    {
        try
        {
            var host = _configuration["Smtp:Host"];
            var portStr = _configuration["Smtp:Port"];
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:From"] ?? "no-reply@oficina.com";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Configurações de SMTP incompletas. E-mail não enviado.");
                return;
            }

            using var client = new SmtpClient(host, int.Parse(portStr ?? "587"))
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            string subject;
            string body;

            if (aceite)
            {
                subject = $"Revisão Confirmada - Moto {placaMoto}";
                body = $"Olá,\n\nSua solicitação de agendamento para a revisão nº {numeroRevisao} da moto {placaMoto} foi CONFIRMADA.";
                if (dataAgendada.HasValue)
                    body += $"\nData agendada: {dataAgendada.Value:dd/MM/yyyy}.";
            }
            else
            {
                subject = $"Revisão Recusada - Moto {placaMoto}";
                body = $"Olá,\n\nInfelizmente sua solicitação de agendamento para a revisão nº {numeroRevisao} da moto {placaMoto} foi RECUSADA.";
                if (dataAgendada.HasValue)
                    body += $"\nData solicitada: {dataAgendada.Value:dd/MM/yyyy}.";

                body += "\n\nVocê pode tentar agendar novamente selecionando outra data ou concessionária.";
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(emailCliente);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"E-mail de status de agendamento enviado para {emailCliente} - Moto {placaMoto}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar e-mail para {emailCliente}");
        }
    }
}