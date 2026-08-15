using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ReleaseService(PortfolioContext portfolioContext, IOptions<GeneralSettings> settings)
{
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
                string url = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", session.projectId.ToString(), $"{session.releaseId}?platform={session.platform}");

                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage res = await client.SendAsync(msg);

                res.EnsureSuccessStatusCode();

                string json = await res.Content.ReadAsStringAsync();
                JsonElement doc = JsonDocument.Parse(json).RootElement;

                foreach (var file in doc.GetProperty("files").EnumerateArray())
                    size += file.GetProperty("size").GetInt64();
            }
        }
        catch { }

        download.DownloadSize = size;
        download.ReleaseEngineManifest = Path.Combine(settings.Value.releaseEngineUrl, "api", "Releases", session.projectId.ToString());

        await portfolioContext.SaveChangesAsync();
    }
}
