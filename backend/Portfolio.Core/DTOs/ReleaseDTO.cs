using Portfolio.Core.Data;
using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class ReleaseDTO
{
    public int versionId { get; set; }
    public string? version { get; set; }

    public string? patchNotes { get; set; }

    public ReleaseStatus status { get; set; }
    public ReleaseDownloadDto[] downloads { get; set; } = [];

    public static ReleaseDTO Map(ReleaseModel model) => new ReleaseDTO
    {
        versionId = model.ReleaseId,
        version = model.VersionName,

        status = model.Status,
        patchNotes = model.PatchNotes,

        downloads = model.Downloads.Select(ReleaseDownloadDto.Map).ToArray()
    };
}
