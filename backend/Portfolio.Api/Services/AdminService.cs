using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class AdminService
{
    private const string IMAGE_URI_PREFIX = "images";

    private CacheService _cache;
    private PortfolioContext _portfolioContext;

    private GeneralSettings _settings;

    public AdminService(CacheService cache, PortfolioContext portfolioContext, IOptions<GeneralSettings> settings)
    {
        _cache = cache;
        _portfolioContext = portfolioContext;

        _settings = settings.Value;
    }

    public async Task<string> SaveImage(IFormFile file)
    {
        if (string.IsNullOrEmpty(_settings.ContentStorageFolder))
            throw new Exception("ContentStorageFolder not set");

        if (!Directory.Exists(_settings.ContentStorageFolder))
            Directory.CreateDirectory(_settings.ContentStorageFolder);

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string path = Path.Combine(_settings.ContentStorageFolder, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine(IMAGE_URI_PREFIX, fileName);
    }

    public async Task<string[]> GetImages()
    {
        if (string.IsNullOrEmpty(_settings.ContentStorageFolder))
            throw new Exception("ContentStorageFolder not set");

        if (!Directory.Exists(_settings.ContentStorageFolder))
            return [];

        return Directory.GetFiles(_settings.ContentStorageFolder).Select(x => Path.GetFileName(x)).ToArray();
    }


    public async Task<ProjectDto> GetProject(Guid id)
    {
        var res = await _portfolioContext.Projects
            .Include(p => p.Elements)
                .ThenInclude(p => p.Parameters)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .Include(p => p.Releases)
                .ThenInclude(r => r.Downloads)
            .FirstOrDefaultAsync(g => g.id == id);

        return ProjectDto.Map(res!);
    }


    public async Task<string> SaveProject(ProjectDto project)
    {
        ProjectModel model = await _portfolioContext.Projects
            .Include(m => m.Tags)
            .SingleAsync(p => p.id == project.id);

        model.name = project.gameName;
        model.icon = project.icon;
        model.description = project.shortDescription;
        model.projectType = project.type;
        model.slug = project.slug;

        _portfolioContext.RemoveRange(model.Tags);
        model.Tags.Clear();

        foreach (int tag in project.tags?.Select(t => t.id).ToArray() ?? [])
        {
            model.Tags.Add(new ProjectTagModel()
            {
                ProjectId = model.id,
                TagId = tag,
            });
        }

        await _portfolioContext.SaveChangesAsync();
        return project.slug;
    }

    public async Task<ProjectModel> CreateProject()
    {
        Guid id = Guid.NewGuid();

        ProjectModel project = new ProjectModel()
        {
            id = id,
            name = "new",
            projectType = ProjectType.Project,
            slug = id.ToString()
        };

        await _portfolioContext.Projects.AddAsync(project);
        await _portfolioContext.SaveChangesAsync();

        return project;
    }

    public async Task SaveProjectContent(Guid projectId, ProjectDto.ElementGroup newData)
    {
        if (newData.id < 0)
        {
            await _portfolioContext.Elements.AddAsync(new ProjectElementModel()
            {
                ProjectId = projectId,
                Parameters = newData.elements?.Select(MapParam).ToArray() ?? [],
                Type = newData.type,
            });
            await _portfolioContext.SaveChangesAsync();
        }
        else
        {
            ProjectElementModel data = await _portfolioContext.Elements
                .Include(e => e.Parameters)
                .SingleAsync(e => e.ProjectId == projectId && e.Id == newData.id);

            _portfolioContext.RemoveRange(data.Parameters);
            data.Parameters.Clear();

            foreach (var param in newData.elements ?? [])
                data.Parameters.Add(MapParam(param));

            await _portfolioContext.SaveChangesAsync();
        }

        ProjectElementParameterModel MapParam(ProjectDto.ElementGroup.ElementParameter param)
        {
            return new ProjectElementParameterModel()
            {
                Order = param.order,
                ParameterValue1 = param.value1,
                ParameterValue2 = param.value2,
                ParameterValue3 = param.value3,
            };
        }
    }

    public async Task<ProjectDto.Tag[]> GetTags()
    {
        return (await _portfolioContext.Tags.ToArrayAsync()).Select(ProjectDto.Tag.Map).ToArray();
    }

    public async Task SaveTags(ProjectDto.Tag[] tags)
    {
        HashSet<int> tagIds = tags.Select(x => x.id).ToHashSet();

        TagModel[] tagsToRemove = await _portfolioContext.Tags.Where(x => !tagIds.Contains(x.Id)).ToArrayAsync();
        ProjectTagModel[] oldProjectTags = await _portfolioContext.ProjectTags.Where(x => !tagIds.Contains(x.Id)).ToArrayAsync();

        foreach (var tag in tags)
        {
            if (tag.id < 0)
            {
                await _portfolioContext.Tags.AddAsync(new TagModel()
                {
                    Name = tag.name,
                    customColour = tag.customColour
                });
            }
            else
            {
                var existingDb = await _portfolioContext.Tags.SingleOrDefaultAsync(t => t.Id == tag.id);
                existingDb!.Name = tag.name;
                existingDb!.customColour = tag.customColour;
            }
        }

        _portfolioContext.RemoveRange(tagsToRemove);
        _portfolioContext.RemoveRange(oldProjectTags);

        await _portfolioContext.SaveChangesAsync();
    }

    public async Task CreateOrUpdateRelease(Guid projectId, ReleaseDTO data)
    {
        ReleaseModel model;

        if (data.versionId < 0)
        {
            model = new ReleaseModel
            {
                ProjectId = projectId,
                PatchNotes = data.patchNotes,
                VersionName = data.version,
                Downloads = data.downloads.Select(MapDownload).ToArray()
            };

            await _portfolioContext.AddAsync(model);
            await _portfolioContext.SaveChangesAsync();

            return;
        }

        model = await _portfolioContext.Releases.Include(r => r.Downloads).SingleAsync(r => r.ProjectId == projectId && r.ReleaseId == data.versionId);
        _portfolioContext.RemoveRange(model.Downloads);

        model.VersionName = data.version;
        model.PatchNotes = data.patchNotes;

        model.Downloads.Clear();

        foreach (var download in data.downloads.Select(MapDownload))
            model.Downloads.Add(download);

        await _portfolioContext.SaveChangesAsync();

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

    public async Task<string> UploadToReleaseEngine(Guid projectId, int releaseId, string platform)
    {
        ReleaseDownloadModel download = await _portfolioContext.ReleaseDownloads.SingleAsync(rd => rd.ProjectId == projectId && rd.ReleaseId == releaseId && rd.Platform == platform);
        download.ReleaseEngineManifest = Path.Combine(_settings.releaseEngineUrl, "api", "Releases", projectId.ToString());

        await _portfolioContext.SaveChangesAsync();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Path.Combine(_settings.releaseEngineUrl, "api", "Releases", projectId.ToString(), $"Create?releaseId={releaseId}");

                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
                HttpResponseMessage res = await client.SendAsync(msg);

                res.EnsureSuccessStatusCode();
            }
        }
        catch { }

        return Path.Combine(Path.Combine("api", "admin", "project", projectId.ToString(), "release", $"{releaseId}?platform={platform}"));
    }
}
