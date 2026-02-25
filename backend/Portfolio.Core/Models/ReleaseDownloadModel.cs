using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Data;

namespace Portfolio.Core.Models;

public class ReleaseDownloadModel
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ReleaseId { get; set; }
    public ReleaseModel? Release { get; set; } = null;

    [Required]
    public DownloadType DownloadType { get; set; }

    [Required]
    public required string DownloadLink { get; set; }


}
