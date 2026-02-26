namespace Portfolio.Api.Types;

public class MailSettings
{
    public string host { get; set; } = "";
    public int port { get; set; }

    public string sender { get; set; } = "";
    public string password { get; set; } = "";
}
