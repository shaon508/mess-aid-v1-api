namespace MassAidVOne.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string ToEmail, string Subject, string Body);
    }
}
