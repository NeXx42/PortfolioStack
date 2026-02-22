using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Data;

namespace Portfolio.Core.Models;

public class ProjectElementModel
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { set; get; }

    [Required]
    public Guid ProjectId { get; set; }
    public ProjectModel? Project { get; set; } = null;

    [Required]
    public ElementType Type { get; set; }

    public ICollection<ProjectElementParameterModel> Parameters { get; set; }
        = new List<ProjectElementParameterModel>();
}
