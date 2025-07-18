using System.Net;
using System.Net.Mail;
namespace CourseManager.Services;

public interface IEmailService
{
    void SendEmail(string subject, string body, string toEmail);
}
public class EmailService : IEmailService
{
    //i am using appsettings.json config
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string subject, string body, string toEmail)
    {
        //getting the emailSeetings from appsettings.json
        var emailConfig = _configuration.GetSection("EmailSettings");

        var fromAddress = new MailAddress(emailConfig["FromEmail"], emailConfig["DisplayName"]);
        var toAddress = new MailAddress(toEmail);
        var fromPassword = emailConfig["Password"];

        using var smtp = new SmtpClient
        {
            Host = emailConfig["Host"],
            Port = int.Parse(emailConfig["Port"]),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };

        smtp.Send(message);
    }
}
