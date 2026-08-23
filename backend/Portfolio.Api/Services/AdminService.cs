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
    private const string IMAGE_STORAGE_SUBFOLDER = "Images";

    private CacheService _cache;
    private PortfolioContext _portfolioContext;

    private GeneralSettings _settings;

    public AdminService(CacheService cache, PortfolioContext portfolioContext, IOptions<GeneralSettings> settings)
    {
        _cache = cache;
        _portfolioContext = portfolioContext;

        _settings = settings.Value;
    }

    public async Task<ProjectDto[]> GetAllProjects()
    {
        ProjectModel[] dbRes = await _portfolioContext.Projects
          .Include(p => p.Tags)
          .ThenInclude(t => t.Tag)
          .ToArrayAsync();

        return dbRes.Select(ProjectDto.Map).ToArray();
    }

    public async Task<string> SaveImage(IFormFile file)
    {
        if (string.IsNullOrEmpty(_settings.ContentStorageFolder))
            throw new Exception("ContentStorageFolder not set");

        if (!Directory.Exists(_settings.ContentStorageFolder))
            Directory.CreateDirectory(_settings.ContentStorageFolder);

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        string root = Path.Combine(_settings.ContentStorageFolder, IMAGE_STORAGE_SUBFOLDER);
        Directory.CreateDirectory(root);

        string path = Path.Combine(root, fileName);

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
        model.genre = project.genre;
        model.status = project.status;
        model.UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (project.dateCreated != null)
            model.CreatedDate = project.dateCreated.Value;

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
            status = ReleaseStatus.Unpublished,
            slug = id.ToString()
        };

        await _portfolioContext.Projects.AddAsync(project);
        await _portfolioContext.SaveChangesAsync();

        return project;
    }

    public async Task SaveProjectContent(Guid projectId, ElementDto newData)
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

        ProjectElementParameterModel MapParam(ElementParameterDto param)
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

    public async Task<TagDto[]> GetTags()
    {
        return (await _portfolioContext.Tags.ToArrayAsync()).Select(TagDto.Map).ToArray();
    }

    public async Task SaveTags(TagDto[] tags)
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
}
