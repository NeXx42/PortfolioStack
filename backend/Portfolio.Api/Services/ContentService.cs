using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
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

    public async Task<ProjectDto[]> GetContentForType(ProjectType type)
    {
        if (_cache.TryGetValue(type, out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await _portfolioContext.Projects
            .Include(p => p.Tags)
            .ThenInclude(t => t.Tag)
            .Where(x => x.projectType == type)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.Set(type, results);
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
        ProjectModel? game = await _portfolioContext.Projects
            .Include(p => p.Elements)
                .ThenInclude(p => p.Parameters)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .FirstOrDefaultAsync(g => g.slug.Equals(slug));

        if (game != null)
        {
            ProjectDto dto = ProjectDto.Map(game);
            _cache.Set(slug, dto);

            return dto;
        }

        return null;
    }

    public async Task UpdateGame(string slug, ProjectDto.ElementGroup[] newElements, ProjectDto.ElementGroup[] modifications)
    {
        ProjectModel? dbEntry = await _portfolioContext.Projects.FirstOrDefaultAsync(p => p.slug.Equals(slug));

        if (dbEntry == null)
            throw new Exception("Project not found");

        using (var trans = await _portfolioContext.Database.BeginTransactionAsync())
        {
            foreach (ProjectDto.ElementGroup newElement in newElements)
            {
                List<ProjectElementParameterModel> ps = new List<ProjectElementParameterModel>();

                foreach (ProjectDto.ElementGroup.ElementParameter param in (newElement.elements ?? []))
                {

                }

                await _portfolioContext.Elements.AddAsync(new ProjectElementModel()
                {
                    Parameters = ps,
                    Type = newElement.type,
                    ProjectId = dbEntry.id
                });
            }

            await _portfolioContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
    }




    public async Task ClearCache()
    {
        foreach (ProjectType key in Enum.GetValues<ProjectType>())
            _cache.Remove(key);
    }
}
