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

    public MailService(IOptions<MailSettings> settings, CacheService cache)
    {
        _settings = settings.Value;
        _cache = cache;
    }

    public async Task SentEmailVerification(string target)
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

        message.Body = new TextPart("plain")
        {
            Text = emailCode.ToString()
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
