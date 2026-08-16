using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class ElementParameterDto
{
    public required int id { get; set; }
    public required int order { get; set; }

    public string? value1 { get; set; }
    public string? value2 { get; set; }
    public string? value3 { get; set; }

    public static ElementParameterDto Map(ProjectElementParameterModel model) => new ElementParameterDto()
    {
        id = model.Id,
        order = model.Order,

        value1 = model.ParameterValue1,
        value2 = model.ParameterValue2,
        value3 = model.ParameterValue3
    };
}
