using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace gRide.Services
{
    public class MailSender : IMailSender
    {
        private readonly IOptions<MailSenderSettings> _mailSenderSettings;

        public MailSender(IOptions<MailSenderSettings> mailSenderSettings)
        {
            _mailSenderSettings = mailSenderSettings;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            MailMessage message = new(from, to, subject, body);
            SmtpClient client = new(_mailSenderSettings.Value.Host, _mailSenderSettings.Value.Port);
            client.Credentials = new NetworkCredential(_mailSenderSettings.Value.Login, _mailSenderSettings.Value.Password);
            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in MailSender.SendAsync(): {0}",
                    ex.ToString());
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
