using LinkifyBLL.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace LinkifyBLL.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly string _email;
        private readonly string _password;

        public EmailService(IConfiguration config)
        {
            _email = config["EMAIL_CONFIGURATION:EMAIL"];
            _password = config["EMAIL_CONFIGURATION:PASSWORD"];
        }

        public async Task SendEmail(string Receiver, string EmailSubject, string EmailBody ="")
        {
            if (string.IsNullOrEmpty(_email))
                throw new ArgumentNullException(nameof(_email), "Sender email is not configured.");

            #region HtmlBody
            string htmlBody = $@"

<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Linkify Email</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; background-color: #f8fafc;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin: 0; padding: 20px; background-color: #f8fafc;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width: 600px; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);"">
                    
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #0066cc 0%, #004499 100%); background-color: #0066cc; padding: 30px 40px; text-align: center; color: white;"">
                            <div style=""font-size: 32px; font-weight: 700; margin-bottom: 8px; letter-spacing: -0.5px;"">Linkify</div>
                            <div style=""font-size: 14px; opacity: 0.9; font-weight: 300;"">Your Professional Network Awaits</div>
                        </td>
                    </tr>
                    
                    <!-- Main Content -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <h1 style=""font-size: 24px; font-weight: 600; color: #1a202c; margin-bottom: 16px; margin-top: 0;"">Welcome to Linkify!</h1>
                            
                            <p style=""font-size: 16px; color: #4a5568; margin-bottom: 32px; line-height: 1.7;"">
                                Thank you for joining our professional community. We're excited to help you connect with opportunities, expand your network, and accelerate your career growth.
                            </p>
                            
                            <!-- Dynamic content placeholder -->
                            <div style=""background: #f7fafc; border-left: 4px solid #0066cc; padding: 20px; margin: 24px 0; border-radius: 0 8px 8px 0; font-size: 16px; color: #2d3748;"">
                                <strong>{Receiver}</strong>
                            </div>
                            
                            <!-- Features Section -->
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin: 32px 0;"">
                                <tr>
                                    <td width=""33.33%"" style=""text-align: center; padding: 20px; background: #f8fafc; border-radius: 8px; border: 1px solid #e2e8f0; vertical-align: top;"">
                                        <div style=""width: 48px; height: 48px; background: linear-gradient(135deg, #0066cc 0%, #004499 100%); background-color: #0066cc; border-radius: 50%; margin: 0 auto 12px; display: flex; align-items: center; justify-content: center; color: white; font-size: 20px; line-height: 48px;"">👥</div>
                                        <div style=""font-weight: 600; color: #1a202c; margin-bottom: 8px;"">Network</div>
                                        <div style=""font-size: 14px; color: #4a5568;"">Connect with industry professionals and expand your reach</div>
                                    </td>
                                    <td width=""10"" style=""width: 10px;""></td>
                                    <td width=""33.33%"" style=""text-align: center; padding: 20px; background: #f8fafc; border-radius: 8px; border: 1px solid #e2e8f0; vertical-align: top;"">
                                        <div style=""width: 48px; height: 48px; background: linear-gradient(135deg, #0066cc 0%, #004499 100%); background-color: #0066cc; border-radius: 50%; margin: 0 auto 12px; display: flex; align-items: center; justify-content: center; color: white; font-size: 20px; line-height: 48px;"">💼</div>
                                        <div style=""font-weight: 600; color: #1a202c; margin-bottom: 8px;"">Opportunities</div>
                                        <div style=""font-size: 14px; color: #4a5568;"">Discover jobs tailored to your skills and experience</div>
                                    </td>
                                    <td width=""10"" style=""width: 10px;""></td>
                                    <td width=""33.33%"" style=""text-align: center; padding: 20px; background: #f8fafc; border-radius: 8px; border: 1px solid #e2e8f0; vertical-align: top;"">
                                        <div style=""width: 48px; height: 48px; background: linear-gradient(135deg, #0066cc 0%, #004499 100%); background-color: #0066cc; border-radius: 50%; margin: 0 auto 12px; display: flex; align-items: center; justify-content: center; color: white; font-size: 20px; line-height: 48px;"">🚀</div>
                                        <div style=""font-weight: 600; color: #1a202c; margin-bottom: 8px;"">Growth</div>
                                        <div style=""font-size: 14px; color: #4a5568;"">Learn from experts and advance your career</div>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Call to Action -->
                            <div style=""text-align: center; margin: 32px 0;"">
                                <a href=""https://linkify.com"" style=""display: inline-block; padding: 16px 32px; background: linear-gradient(135deg, #0066cc 0%, #0052a3 100%); background-color: #0066cc; color: #ffffff; text-decoration: none; border-radius: 8px; font-weight: 600; font-size: 16px; box-shadow: 0 4px 12px rgba(0, 102, 204, 0.3);"">Explore Linkify</a>
                            </div>
                            
                            <p style=""font-size: 16px; color: #4a5568; margin-bottom: 32px; line-height: 1.7;"">
                                Ready to take the next step in your professional journey? Your network is waiting for you.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background: #f8fafc; padding: 30px 40px; border-top: 1px solid #e2e8f0; text-align: center;"">
                            <div style=""margin-bottom: 20px;"">
                                <a href=""#"" style=""display: inline-block; margin: 0 10px; width: 36px; height: 36px; background: #0066cc; color: white; text-decoration: none; border-radius: 50%; line-height: 36px; font-size: 16px;"">f</a>
                                <a href=""#"" style=""display: inline-block; margin: 0 10px; width: 36px; height: 36px; background: #0066cc; color: white; text-decoration: none; border-radius: 50%; line-height: 36px; font-size: 16px;"">t</a>
                                <a href=""#"" style=""display: inline-block; margin: 0 10px; width: 36px; height: 36px; background: #0066cc; color: white; text-decoration: none; border-radius: 50%; line-height: 36px; font-size: 16px;"">in</a>
                                <a href=""#"" style=""display: inline-block; margin: 0 10px; width: 36px; height: 36px; background: #0066cc; color: white; text-decoration: none; border-radius: 50%; line-height: 36px; font-size: 16px;"">@</a>
                            </div>
                            
                            <div style=""font-size: 14px; color: #718096; margin-bottom: 8px;"">
                                <strong>Linkify</strong><br>
                                Building Professional Connections Worldwide
                            </div>
                            
                            <div style=""font-size: 14px; color: #718096; margin-bottom: 8px;"">
                                Questions? Contact us at <a href=""mailto:support@linkify.com"" style=""color: #0066cc; text-decoration: none;"">support@linkify.com</a>
                            </div>
                            
                            <div style=""font-size: 12px; color: #a0aec0;"">
                                <a href=""#"" style=""color: #0066cc; text-decoration: none;"">Update preferences</a> | <a href=""#"" style=""color: #0066cc; text-decoration: none;"">Unsubscribe</a>
                            </div>
                        </td>
                    </tr>
                    
                </table>
            </td>
        </tr>
    </table>

    <!-- Mobile-specific styles -->
    <style>
        @media screen and (max-width: 600px) {{
            .mobile-center {{ text-align: center !important; }}
            .mobile-full-width {{ width: 100% !important; display: block !important; }}
            .mobile-padding {{ padding: 20px !important; }}
            .mobile-stack {{ display: block !important; width: 100% !important; }}
        }}
    </style>
</body>
</html>
";
            #endregion


            var msg = new MailMessage(_email, Receiver, EmailSubject, htmlBody)
            {
                IsBodyHtml = true // Enable HTML body
            };


            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_email, _password);

                await client.SendMailAsync(msg);
            }
        }
    }
}