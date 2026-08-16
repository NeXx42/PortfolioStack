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
    public string? genre { get; set; }

    public long? dateCreated { get; set; }
    public long? dateUpdated { get; set; }

    public ElementDto[]? elements { get; set; }

    public TagDto[]? tags { get; set; }
    public ReleaseDTO[]? releases { get; set; }

    public ProjectType type { get; set; }
    public ReleaseStatus status { get; set; }

    public static ProjectDto Map(ProjectModel model)
    {
        return new ProjectDto()
        {
            id = model.id,
            gameName = model.name,

            icon = model.icon,
            slug = model.slug,
            shortDescription = model.description,
            genre = model.genre,

            dateCreated = model.CreatedDate,
            dateUpdated = model.UpdatedDate,

            type = model.projectType,
            status = model.status,

            tags = model.Tags?.Select(TagDto.Map).ToArray(),
            elements = model.Elements?.Select(ElementDto.Map).ToArray() ?? [],
            releases = model.Releases.Select(ReleaseDTO.Map).ToArray()
        };
    }
}
