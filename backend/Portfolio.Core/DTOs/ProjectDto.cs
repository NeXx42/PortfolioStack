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

            tags = model.Tags?.Select(t => new Tag()
            {
                name = t.Tag!.Name,
                customColour = t.Tag.customColour
            }).ToArray() ?? [],

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
            }).ToArray() ?? []
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
        public required string name { get; set; }
        public string? customColour { get; set; }
    }
}
