using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ReleaseService(PortfolioContext portfolioContext, IOptions<GeneralSettings> settings)
{
    public async Task CreateOrUpdateRelease(Guid projectId, ReleaseDTO data)
    {
        ReleaseModel model;

        if (data.versionId < 0)
        {
            model = new ReleaseModel
            {
                ProjectId = projectId,
                PatchNotes = data.patchNotes,
                Status = Core.Data.ReleaseStatus.Unpublished,
                VersionName = data.version,
                Downloads = data.downloads.Select(MapDownload).ToArray()
            };

            await portfolioContext.AddAsync(model);
            await portfolioContext.SaveChangesAsync();

            return;
        }

        model = await portfolioContext.Releases.Include(r => r.Downloads).SingleAsync(r => r.ProjectId == projectId && r.ReleaseId == data.versionId);
        portfolioContext.RemoveRange(model.Downloads);

        model.VersionName = data.version;

        model.Status = data.status;
        model.PatchNotes = data.patchNotes;

        model.Downloads.Clear();

        foreach (var download in data.downloads.Select(MapDownload))
            model.Downloads.Add(download);

        await portfolioContext.SaveChangesAsync();

        ReleaseDownloadModel MapDownload(ReleaseDownloadDto d)
        {
            return new ReleaseDownloadModel()
            {
                DownloadSize = d.size,
                DownloadUrl = d.downloadLink,
                Platform = d.platform,
                EntryPoint = d.entryPoint,
                ReleaseEngineManifest = d.releaseEngineManifestLink
            };
        }
    }

    public async Task<UploadSession> CreateReleaseEngineEntry(Guid projectId, int releaseId, string platform)
    {
        ReleaseDownloadModel download = await portfolioContext.ReleaseDownloads.SingleAsync(rd => rd.ProjectId == projectId && rd.ReleaseId == releaseId && rd.Platform == platform);
        download.ReleaseEngineManifest = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", projectId.ToString());

        await portfolioContext.SaveChangesAsync();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", projectId.ToString(), $"Create?releaseId={releaseId}");

                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
                HttpResponseMessage res = await client.SendAsync(msg);

                res.EnsureSuccessStatusCode();
            }
        }
        catch { }

        return new UploadSession()
        {
            projectId = projectId,
            releaseId = releaseId,
            platform = platform,
        };
    }

    public async Task CompleteReleaseUpload(UploadSession session)
    {
        long size = 0;
        ReleaseDownloadModel download = await portfolioContext.ReleaseDownloads.SingleAsync(rd => rd.ProjectId == session.projectId && rd.ReleaseId == session.releaseId && rd.Platform == session.platform);

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", session.projectId.ToString(), $"{session.releaseId}/InstallSize?platform={session.platform}");

                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage res = await client.SendAsync(msg);

                res.EnsureSuccessStatusCode();

                string json = await res.Content.ReadAsStringAsync();
                long.TryParse(json, out size);
            }
        }
        catch
        {
            Console.WriteLine("Failed to derive install size");
        }

        download.DownloadSize = size;
        download.ReleaseEngineManifest = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", session.projectId.ToString());

        await portfolioContext.SaveChangesAsync();
    }

    public async Task<ReleaseDTO?> GetRelease(Guid projectId, int? versionId)
    {
        ReleaseModel? release = await portfolioContext.Releases
            .Where(r => r.ProjectId == projectId && (!versionId.HasValue || r.ReleaseId == versionId.Value))
            .Include(r => r.Downloads)
            .OrderByDescending(r => r.ReleaseId)
            .FirstOrDefaultAsync();

        return MapReleases(release).FirstOrDefault();
    }

    public async Task<ReleaseDTO[]> GetReleases(Guid projectId)
    {
        ReleaseModel[] releases = await portfolioContext.Releases
            .Where(r => r.ProjectId == projectId)
            .Include(r => r.Downloads)
            .OrderByDescending(r => r.ReleaseId)
            .ToArrayAsync();

        return MapReleases(releases);
    }

    private ReleaseDTO[] MapReleases(params ReleaseModel?[] releasesInp)
    {
        return releasesInp.Where(r => r != null).Select(release => new ReleaseDTO()
        {
            versionId = release!.ReleaseId,
            version = release.VersionName,

            status = release.Status,
            patchNotes = release.PatchNotes,

            downloads = release.Downloads.Select(d => new ReleaseDownloadDto()
            {
                platform = d.Platform,

                size = d.DownloadSize,
                entryPoint = d.EntryPoint,

                downloadLink = d.DownloadUrl,
                releaseEngineManifestLink = d.ReleaseEngineManifest

            }).ToArray()

        }).ToArray();
    }

    public async Task<FileDownloadInfo[]> GetDownloadFiles(Guid projectId, int versionId, string platform)
    {
        ReleaseModel? release = await portfolioContext.Releases
            .Include(r => r.Downloads)
            .Where(r => r.ProjectId == projectId && r.ReleaseId == versionId)
            .FirstOrDefaultAsync();

        if (release == null)
            throw new ArgumentException("Release not found");

        if (release.Status != Core.Data.ReleaseStatus.Published)
            throw new ArgumentException("Release not found");

        ReleaseDownloadModel? download = release.Downloads.FirstOrDefault(d => d.Platform.Equals(platform, StringComparison.InvariantCultureIgnoreCase));

        if (download == null)
            throw new ArgumentException($"Unsupported platform, expected {string.Join(", ", release.Downloads.Select(d => d.Platform))}");

        string releaseEngineRequestUrl = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", projectId.ToString(), versionId.ToString(), $"Files?platform={download.Platform}");
        List<FileDownloadInfo> files = new List<FileDownloadInfo>();

        using (HttpClient client = new HttpClient())
        {
            string url = Path.Combine(releaseEngineRequestUrl);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage res = await client.SendAsync(msg);

            res.EnsureSuccessStatusCode();

            string json = await res.Content.ReadAsStringAsync();
            JsonElement doc = JsonDocument.Parse(json).RootElement;

            foreach (var file in doc.EnumerateArray())
                files.Add(new FileDownloadInfo()
                {
                    downloadUrl = file.GetProperty("url").GetString()!,
                    hash = file.GetProperty("hash").GetString()!,
                    relativePath = file.GetProperty("path").GetString()!,

                    size = file.GetProperty("size").TryGetInt64(out long size) ? size : null,
                });
        }

        return files.ToArray();
    }
}
