namespace gRide.Services
{
    public interface IMailSender
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}