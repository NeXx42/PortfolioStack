using Portfolio.Core.Data;
using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class ElementDto
{
    public required int id { get; set; }
    public required ElementType type { get; set; }
    public ElementParameterDto[]? elements { get; set; }

    public static ElementDto Map(ProjectElementModel model) => new ElementDto()
    {
        id = model.Id,
        type = model.Type,

        elements = model.Parameters?.Select(ElementParameterDto.Map).ToArray() ?? []
    };
}
