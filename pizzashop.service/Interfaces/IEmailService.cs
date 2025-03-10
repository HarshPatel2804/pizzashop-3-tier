namespace pizzashop.service.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string HtmlBody, string imagePath);

    Task<bool> SendResetPasswordEmailAsync(string toEmail, string resetLink);

    Task<bool> SendEmailtoNewUserAsync(string toEmail,string FirstName,string Password);
}
