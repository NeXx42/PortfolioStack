using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Core.Data;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ContentService
{
    private const string CACHE_FEATURED_CONTENT = "Content_FeaturedContent";

    private IMemoryCache _cache;
    private PortfolioContext _portfolioContext;

    public ContentService(IMemoryCache cache, PortfolioContext portfolioContext)
    {
        _cache = cache;
        _portfolioContext = portfolioContext;
    }

    public async Task<ProjectModel[]> GetContentForType(ProjectType type)
    {
        if (_cache.TryGetValue(type, out ProjectModel[]? projects) && projects != null)
            return projects;

        ProjectModel[] results = await _portfolioContext.projects.Where(x => x.projectType == type).ToArrayAsync();
        _cache.Set(type, results);

        return results;
    }

    public async Task<ProjectModel[]> FeaturedContent()
    {
        if (_cache.TryGetValue(CACHE_FEATURED_CONTENT, out ProjectModel[]? projects) && projects != null)
            return projects;

        ProjectModel[] results = await _portfolioContext.projects.Take(3).ToArrayAsync();
        _cache.Set(CACHE_FEATURED_CONTENT, results);

        return results;
    }

    public async Task<ProjectModel?> GetGame(Guid id)
    {
        ProjectModel? game = await _portfolioContext.projects.FirstOrDefaultAsync(g => g.id == id);
        return game;
    }

    public async Task ClearCache()
    {
        foreach (ProjectType key in Enum.GetValues<ProjectType>())
            _cache.Remove(key);
    }
}
