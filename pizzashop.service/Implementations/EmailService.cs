using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using pizzashop.service.Interfaces;
namespace pizzashop.service.Implementations;

public class EmailService : IEmailService
{
    private readonly string _smtpServer = "mail.etatvasoft.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUsername = "test.dotnet@etatvasoft.com";
    private readonly string _smtpPassword = "P}N^{z-]7Ilp";

    public async Task<bool> SendResetPasswordEmailAsync(string toEmail, string resetPasswordLink)
    {
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png"); 

        string htmlBody = $@"
            <div style=""font-family: Arial, sans-serif;"">
                    <div style=""justify-content: center; align-items: center; margin: auto; background-color: #0066A7; display: flex; text-align: center; margin-top: 50px; padding: 20px;"">
                        <img src=""cid:pizzashoplogo"" alt=""Logo"" style=""margin-right: 10px; width: 90px; height: 76px;"">
                        <h1 style=""color: white; font-size: 50px; margin: 0;"">PIZZASHOP</h1>
                    </div>
                <div style='padding: 10px;'>
                    <p style='font-family: sans-serif;'>
                        PizzaShop,<br><br>
                        Please click <a style='color: blue;' href='{resetPasswordLink}'>here</a> to reset your password.<br><br>
                        If you encounter any issues or have any questions, please do not hesitate to contact our support team.<br><br>
                        <span style='color: rgb(235, 183, 13);'>Important Note:</span> For security reasons, the link will expire in 24 hours.
                    </p>
                </div>
            </div>";

        return await SendEmailAsync(toEmail, "Reset Password", htmlBody, imagePath);
    }


    public async Task<bool> SendEmailAsync(string toEmail, string subject, string HtmlBody, string imagePath)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PizzaShop", _smtpUsername));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            var image = bodyBuilder.LinkedResources.Add(imagePath);
            image.ContentId = "pizzashoplogo";

            bodyBuilder.HtmlBody = HtmlBody.Replace("cid:pizzashoplogo", $"cid:{image.ContentId}");

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
