namespace Portfolio.Core.Data;

public class GameLauncherMetadata
{
    public Guid id { get; set; }

    public string gameName { get; set; } = "";
    public string about { get; set; } = "";
    public string genre { get; set; } = "";

    public long? releaseDate { get; set; }
    public long? updateDate { get; set; } // version

    public string? iconUrl { get; set; } = "";
    public string[] imageUrls { get; set; } = [];

    public Releases[] versions { get; set; } = [];

    public struct Releases
    {
        public int versionId { get; set; }
        public string versionName { get; set; }
        public string? patchNotes { get; set; }

        public Platform[] platforms { get; set; }

        public struct Platform
        {
            public string platform { get; set; }
            public string? link { get; set; }

            public string? entrypoint { get; set; }
            public long? size { get; set; }
        }
    }
}
