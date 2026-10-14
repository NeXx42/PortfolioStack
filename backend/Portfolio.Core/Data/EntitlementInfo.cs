namespace Portfolio.Core.Data;

public class EntitlementInfo
{
    public required string Key { get; set; }
    public string? DefaultValue { get; set; }

    public EntitlementType Type { get; set; }
}
