using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ContentService
{
    private const string CACHE_FEATURED_CONTENT = "Content_FeaturedContent";
    private const string CACHE_LINKS = "Content_Links";

    private CacheService _cache;
    private PortfolioContext _portfolioContext;

    private GeneralSettings _settings;

    public ContentService(CacheService cache, PortfolioContext portfolioContext, IOptions<GeneralSettings> settings)
    {
        _cache = cache;
        _portfolioContext = portfolioContext;

        _settings = settings.Value;
    }

    public async Task<ProjectDto[]> GetContentForType(ProjectType type)
    {
        if (_cache.TryGetValue(type.ToString(), out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await _portfolioContext.Projects
            .Include(p => p.Tags)
            .ThenInclude(t => t.Tag)
            .Where(x => x.projectType == type)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.Set(type.ToString(), results);
        return results;
    }

    public async Task<ProjectDto[]> FeaturedContent()
    {
        if (_cache.TryGetValue(CACHE_FEATURED_CONTENT, out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await _portfolioContext.Projects
            .Include(p => p.Tags)
            .ThenInclude(t => t.Tag)
            .Take(3)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.Set(CACHE_FEATURED_CONTENT, results);
        return results;
    }

    public async Task<ProjectDto?> GetGame(string slug)
    {
        if (_cache.TryGetValue(slug, out ProjectDto? proj) && proj != null)
            return proj;

        ProjectModel? game = await _portfolioContext.Projects
            .Include(p => p.Elements)
                .ThenInclude(p => p.Parameters)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .Include(p => p.Releases)
                .ThenInclude(r => r.Downloads)
            .FirstOrDefaultAsync(g => g.slug.Equals(slug));

        if (game != null)
        {
            ProjectDto dto = ProjectDto.Map(game);
            _cache.Set(slug, dto);

            return dto;
        }

        return null;
    }

    public async Task Save(ProjectDto project)
    {
        if (project.id == Guid.Empty)
        {
            ProjectModel dbEntry = new ProjectModel()
            {
                id = Guid.NewGuid(),
                name = project.gameName,
                Price = project.cost,
                projectType = project.type,
                icon = project.icon,
                CreatedDate = project.dateCreated ?? DateTime.UtcNow,
                UpdatedDate = project.dateUpdated ?? DateTime.UtcNow,
                description = project.shortDescription,
                slug = ProjectDto.ConvertNameToSlug(project.gameName),
                Version = project.version,
                Elements = project.elements?.Select(e => new ProjectElementModel()
                {
                    Type = e.type,
                    Parameters = e.elements?.Select(x => new ProjectElementParameterModel()
                    {
                        Order = x.order,
                        ParameterValue1 = x.value1,
                        ParameterValue2 = x.value2,
                        ParameterValue3 = x.value3
                    }).ToArray() ?? []
                }).ToArray() ?? []
            };

            await _portfolioContext.AddAsync(dbEntry);
            await _portfolioContext.SaveChangesAsync();
        }
        else
        {
            ProjectModel dbEntry = await _portfolioContext.Projects
                .Include(p => p.Elements)
                    .ThenInclude(e => e.Parameters)
                .SingleAsync(p => p.id == project.id);

            dbEntry.name = project.gameName;
            dbEntry.Price = project.cost;
            dbEntry.projectType = project.type;
            dbEntry.icon = project.icon;
            dbEntry.UpdatedDate = DateTime.UtcNow;
            dbEntry.description = project.shortDescription;
            dbEntry.slug = ProjectDto.ConvertNameToSlug(project.gameName);
            dbEntry.Version = project.version;

            var elementContainer = dbEntry.Elements.Where(x => !(project.elements?.Any(e => e.id == x.Id) ?? false)).ToArray();
            _portfolioContext.RemoveRange(elementContainer);

            elementContainer = project.elements?.Where(x => x.id < 0).Select(x => new ProjectElementModel()
            {
                ProjectId = dbEntry.id,
                Type = x.type,

                Parameters = x.elements?.Select(p => new ProjectElementParameterModel
                {
                    Order = p.order,
                    ParameterValue1 = p.value1,
                    ParameterValue2 = p.value2,
                    ParameterValue3 = p.value3
                }).ToArray() ?? []
            }).ToArray() ?? [];
            await _portfolioContext.AddRangeAsync(elementContainer);

            foreach (var element in project.elements ?? [])
            {
                if (element.id < 0)
                    continue;

                var dbElement = dbEntry.Elements!.Single(x => x.Id == element.id);
                dbElement.Type = element.type;

                var paramContainer = dbElement.Parameters.Where(x => !(element.elements?.Any(e => e.id == x.Id) ?? false)).ToArray();
                _portfolioContext.RemoveRange(paramContainer);

                paramContainer = element.elements?.Where(x => x.id < 0).Select(p => new ProjectElementParameterModel()
                {
                    ProjectElementId = dbElement.Id,
                    Order = p.order,
                    ParameterValue1 = p.value1,
                    ParameterValue2 = p.value2,
                    ParameterValue3 = p.value3
                }).ToArray() ?? [];
                await _portfolioContext.AddRangeAsync(paramContainer);

                foreach (var param in element.elements ?? [])
                {
                    if (param.id < 0)
                        continue;

                    var dbParam = dbElement.Parameters.Single(x => x.Id == param.id);
                    dbParam.Order = param.order;
                    dbParam.ParameterValue1 = param.value1;
                    dbParam.ParameterValue2 = param.value2;
                    dbParam.ParameterValue3 = param.value3;
                }
            }

            await _portfolioContext.SaveChangesAsync();
        }
    }

    public async Task<LinkModel[]> GetLinks()
    {
        if (_cache.TryGetValue(CACHE_LINKS, out LinkModel[]? links) && links != null)
            return links;

        LinkModel[] dbRes = await _portfolioContext.Links.ToArrayAsync();

        _cache.Set(CACHE_LINKS, dbRes);
        return dbRes;
    }


    public async Task<string[]> GetSlugs()
        => await _portfolioContext.Projects.Select(x => x.slug).ToArrayAsync();

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

        return fileName;
    }

    public async Task<string[]> GetImages()
    {
        if (string.IsNullOrEmpty(_settings.ContentStorageFolder))
            throw new Exception("ContentStorageFolder not set");

        if (!Directory.Exists(_settings.ContentStorageFolder))
            return [];

        return Directory.GetFiles(_settings.ContentStorageFolder).Select(x => Path.GetFileName(x)).ToArray();
    }
}
