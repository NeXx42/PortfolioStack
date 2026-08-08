using Microsoft.EntityFrameworkCore;
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

    public async Task<ProjectDto[]> GetAllProjects()
    {
        ProjectModel[] dbRes = await _portfolioContext.Projects
            .Include(p => p.Tags)
            .ThenInclude(t => t.Tag)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();
        return results;
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

        _cache.SetIfNotExists(type.ToString(), results);
        return results;
    }

    public async Task<ProjectDto[]> FeaturedContent()
    {
        if (_cache.TryGetValue(CACHE_FEATURED_CONTENT, out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await _portfolioContext.Projects
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .OrderByDescending(p => p.UpdatedDate)
            .Take(3)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.SetIfNotExists(CACHE_FEATURED_CONTENT, results);
        return results;
    }

    public async Task<ProjectDto?> GetGame(string slug)
    {
        if (_cache.TryGetValue(slug, out ProjectDto? proj) && proj != null)
            return proj;

        ElementType[] excludedMetadata = [ElementType.LauncherMetadata];

        ProjectModel? game = await _portfolioContext.Projects
            .Include(p => p.Elements.Where(e => !excludedMetadata.Contains(e.Type)))
                .ThenInclude(p => p.Parameters)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .Include(p => p.Releases)
                .ThenInclude(r => r.Downloads)
            .FirstOrDefaultAsync(g => g.slug.Equals(slug));

        if (game != null)
        {
            ProjectDto dto = ProjectDto.Map(game);
            _cache.SetIfNotExists(slug, dto);

            return dto;
        }

        return null;
    }

    public async Task<GameLauncherMetadata[]> GetGameLauncherMetadata(Guid? featured, int? limit)
    {
        var query = _portfolioContext.Projects
            .Include(p => p.Elements.Where(e => e.Type == ElementType.LauncherMetadata))
                .ThenInclude(p => p.Parameters)
            .Where(g => g.projectType == ProjectType.Game)
            .AsQueryable();

        if (featured.HasValue)
            query = query.OrderByDescending(g => g.id == featured.Value)
                        .ThenByDescending(g => g.UpdatedDate);
        else
            query = query.OrderByDescending(g => g.UpdatedDate);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return (await query.ToArrayAsync()).Select(Map).ToArray();

        GameLauncherMetadata Map(ProjectModel model)
        {
            var data = new GameLauncherMetadata()
            {
                Id = model.id,
                GameName = model.name,
            };
            var launcherData = model.Elements.FirstOrDefault(e => e.Type == ElementType.LauncherMetadata);

            if (launcherData != null)
            {
                data.IconUrl = launcherData.Parameters.FirstOrDefault(p => !string.IsNullOrEmpty(p.ParameterValue2) && p.ParameterValue2.Equals("icon"))?.ParameterValue1;
                data.ImageUrls = launcherData.Parameters.Where(p => string.IsNullOrEmpty(p.ParameterValue2) || !p.ParameterValue2.Equals("icon"))?
                    .Select(p => p.ParameterValue1!)
                    .ToArray() ?? [];
            }

            return data;
        }
    }

    public async Task<LinkModel[]> GetLinks()
    {
        if (_cache.TryGetValue(CACHE_LINKS, out LinkModel[]? links) && links != null)
            return links;

        LinkModel[] dbRes = await _portfolioContext.Links.ToArrayAsync();

        _cache.SetIfNotExists(CACHE_LINKS, dbRes);
        return dbRes;
    }


    public async Task<string[]> GetSlugs()
        => await _portfolioContext.Projects.Select(x => x.slug).ToArrayAsync();

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
}
