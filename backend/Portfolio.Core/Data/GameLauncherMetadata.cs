namespace Portfolio.Core.Data;

public class GameLauncherMetadata
{
    public Guid Id { get; set; }
    public string GameName { get; set; } = "";

    public string? IconUrl { get; set; } = "";
    public string[] ImageUrls { get; set; } = [];
}
