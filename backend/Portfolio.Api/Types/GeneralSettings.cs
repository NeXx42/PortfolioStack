namespace Portfolio.Api.Types;

public class GeneralSettings
{
    public bool disableAccountCreation { get; set; } = false;
    public string? ContentStorageFolder { get; set; } = string.Empty;
}
