using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class TagDto
{
    public int id { get; set; }
    public required string name { get; set; }
    public string? customColour { get; set; }

    public static TagDto Map(ProjectTagModel model)
    {
        return new TagDto()
        {
            id = model.Tag!.Id,
            name = model.Tag!.Name,
            customColour = model.Tag!.customColour
        };
    }

    public static TagDto Map(TagModel model)
    {
        return new TagDto()
        {
            id = model.Id,
            name = model.Name,
            customColour = model.customColour
        };
    }
}
