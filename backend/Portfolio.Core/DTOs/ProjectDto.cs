using Portfolio.Core.Data;
using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class ProjectDto
{
    public required Guid id { get; set; }

    public required string slug { get; set; }
    public required string gameName { get; set; }

    public string? icon { get; set; }
    public string? shortDescription { get; set; }

    public DateTime? dateCreated { get; set; }
    public DateTime? dateUpdated { get; set; }

    public string? version { get; set; }
    public decimal? cost { get; set; }

    public ElementGroup[]? elements { get; set; }
    public Tag[]? tags { get; set; }
    public Release[]? releases { get; set; }

    public ProjectType type { get; set; }

    public static ProjectDto Map(ProjectModel model)
    {
        return new ProjectDto()
        {
            id = model.id,
            gameName = model.name,

            icon = model.icon,
            slug = model.slug,

            dateCreated = model.CreatedDate,
            dateUpdated = model.CreatedDate,

            version = model.Version,
            cost = model.Price,

            type = model.projectType,

            tags = model.Tags?.Select(Tag.Map).ToArray(),

            elements = model.Elements?.Select(x => new ElementGroup()
            {
                id = x.Id,
                type = x.Type,

                elements = x.Parameters?.Select(p => new ElementGroup.ElementParameter()
                {
                    id = p.Id,
                    order = p.Order,

                    value1 = p.ParameterValue1,
                    value2 = p.ParameterValue2,
                    value3 = p.ParameterValue3
                }).ToArray() ?? []
            }).ToArray() ?? [],

            releases = model.Releases?.Select(r => new Release()
            {
                id = r.Id,
                version = r.Version,
                date = r.ReleaseDate,
                size = r.Size,

                downloads = r.Downloads.Select(d => new Release.Download()
                {
                    id = d.Id,
                    link = d.DownloadLink,
                    type = d.DownloadType,
                }).ToArray()
            }).ToArray()
        };
    }

    public static string ConvertNameToSlug(string name) => name.ToLower().Replace(" ", "-");

    public class ElementGroup
    {
        public required int id { get; set; }
        public required ElementType type { get; set; }
        public ElementParameter[]? elements { get; set; }


        public class ElementParameter
        {
            public required int id { get; set; }
            public required int order { get; set; }

            public string? value1 { get; set; }
            public string? value2 { get; set; }
            public string? value3 { get; set; }
        }
    }

    public class Tag
    {
        public int id { get; set; }
        public required string name { get; set; }
        public string? customColour { get; set; }

        public static Tag Map(ProjectTagModel model)
        {
            return new Tag()
            {
                id = model.Tag!.Id,
                name = model.Tag!.Name,
                customColour = model.Tag!.customColour
            };
        }

        public static Tag Map(TagModel model)
        {
            return new Tag()
            {
                id = model.Id,
                name = model.Name,
                customColour = model.customColour
            };
        }
    }

    public class Release
    {
        public int id { get; set; }
        public required string version { get; set; }
        public string? size { get; set; }
        public DateTime? date { get; set; }

        public Download[]? downloads { get; set; }

        public class Download
        {
            public DownloadType type;
            public required int id { get; set; }
            public string? link { get; set; }
        }
    }
}
