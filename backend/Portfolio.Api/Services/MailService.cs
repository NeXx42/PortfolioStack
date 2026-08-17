using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public class MailService
{
    private readonly MailSettings _settings;
    private readonly CacheService _cache;

    private const string htmlTemplate = """
    <html lang="en">
        <head>
            <meta charset = "UTF-8" >
        </head>
        <body style="margin:0; padding:0; background-color:#0a0e14; background-image: linear-gradient(rgba(255,255,255,0.015) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.015) 1px, transparent 1px); background-size:28px 28px;">
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="center" style="padding:56px 20px;">
                        <table role="presentation" width="480" cellpadding="0" cellspacing="0" border="0" style="max-width:480px; width:100%;">
                            <tr>
                                <td style="padding-bottom:28px;" >
                                    <img src="https://nexx42.info/Profile.png" width="36" height="36" alt="NEXX42" style="display:inline-block; vertical-align:middle; width:36px; height:36px; border:0; border-radius:4px;">
                                    <h1 style="display:inline-block; vertical-align:middle; margin:0 0 0 10px; font-family: Cinzel Decorative, serif; color:#eef2f6; letter-spacing:0.3px; color:#7ec8e3;">NeXx</h1>
                                </td>
                            </tr>
                            <tr>
                                <td style="background-color:#10151f; border:1px solid #232d3a; border-radius:4px;">
                                    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td style = "padding:40px 40px 32px 40px;" >
                                                <div style="font-family: Georgia, 'Times New Roman', serif; font-size:20px; color:#eef2f6; letter-spacing:0.3px; padding-bottom:10px;">
                                                    Verify it's you
                                                </div>
                            
                                                <div style = "font-family: Arial, Helvetica, sans-serif; font-size:14px; line-height:22px; color:#8a95a5; padding-bottom:28px;" >
                                                    Enter this code to finish creating your account. It expires in 10 minutes.
                                                </div>
                            
                                                <!-- Code pill, styled like the site's tag badges -->
                                                <table role = "presentation" cellpadding= "0" cellspacing= "0" border= "0" >
                                                    <tr>
                                                        <td style="border:1px solid #3a4a56; border-radius:3px; background-color:#0d1219; padding:18px 28px;" >
                                                            <span style= "font-family: 'Courier New', Courier, monospace; font-size:32px; letter-spacing:10px; color:#8fd6e8;">{0}</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div style = "font-family: Arial, Helvetica, sans-serif; font-size:13px; line-height:20px; color:#5f6b7a; padding-top:24px;" >
                                                    Didn't request this? You can safely ignore this email. Your account is still secure.
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style = "border-top:1px solid #1c2430; padding:16px 40px; font-family: Arial, Helvetica, sans-serif; font-size:11px; letter-spacing:1.5px; text-transform:uppercase; color:#4a5563;" >
                                                One - Time Passcode
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style = "padding:28px 4px 0 4px; font-family: Arial, Helvetica, sans-serif; font-size:12px; line-height:18px; color:#4a5563;" >
                                  Sent by https://nexx42.info
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
    </html>
    """;

    public MailService(IOptions<MailSettings> settings, CacheService cache)
    {
        _settings = settings.Value;
        _cache = cache;
    }

    public async Task SendEmailVerification(string target)
    {
        long emailCode = new Random().NextInt64(100000, 999999);

        if (!_cache.SetIfNotExists(target, emailCode, TimeSpan.FromMinutes(5)))
        {
            throw new Exception("Already sent email");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("noreply", _settings.sender));
        message.To.Add(new MailboxAddress("User", target));
        message.Subject = "Verify Email";

        string emailBody = string.Format(htmlTemplate, emailCode.ToString());

        message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = emailBody
        };

        using (var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput())))
        {
            await client.ConnectAsync(_settings.host, _settings.port, MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_settings.sender, _settings.password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public bool ConfirmCode(string address, long? code)
    {
        if (!code.HasValue)
            return false;

        if (_cache.TryGetValue(address, out long actualCode))
            return actualCode == code;

        return false;
    }
}
