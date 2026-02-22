using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Models;

public class ProjectElementParameterModel
{
    public int Id { get; set; }

    [Required]
    public int ProjectElementId { get; set; }
    public ProjectElementModel? ProjectElement { get; set; } = null;

    public string? ParameterValue1 { get; set; }
    public string? ParameterValue2 { get; set; }
    public string? ParameterValue3 { get; set; }

    public int Order { get; set; }
}
