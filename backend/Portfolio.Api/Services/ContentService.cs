using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ContentService(CacheService _cache, PortfolioContext _portfolioContext)
{
    private const string CACHE_FEATURED_CONTENT = "Content_FeaturedContent";
    private const string CACHE_LINKS = "Content_Links";

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
            .Include(p => p.Releases.Where(r => r.Status != ReleaseStatus.Unpublished))
                .ThenInclude(r => r.Downloads)
            .Where(g => g.projectType == ProjectType.Game)
            .AsQueryable();

        if (featured.HasValue)
            query = query.OrderByDescending(g => g.id == featured.Value)
                        .ThenByDescending(g => g.UpdatedDate);
        else
            query = query.OrderByDescending(g => g.UpdatedDate);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        // need to do licence filtering here?

        return (await query.ToArrayAsync()).Select(Map).ToArray();

        GameLauncherMetadata Map(ProjectModel model)
        {
            var data = new GameLauncherMetadata()
            {
                id = model.id,
                gameName = model.name,
                versions = model.Releases.Select(MapRelease).ToArray()
            };
            var launcherData = model.Elements.FirstOrDefault(e => e.Type == ElementType.LauncherMetadata);

            if (launcherData != null)
            {
                data.iconUrl = launcherData.Parameters.FirstOrDefault(p => !string.IsNullOrEmpty(p.ParameterValue2) && p.ParameterValue2.Equals("icon"))?.ParameterValue1;
                data.imageUrls = launcherData.Parameters.Where(p => string.IsNullOrEmpty(p.ParameterValue2) || !p.ParameterValue2.Equals("icon"))?
                    .Select(p => p.ParameterValue1!)
                    .ToArray() ?? [];
            }

            return data;
        }

        GameLauncherMetadata.Releases MapRelease(ReleaseModel release)
        {
            return new GameLauncherMetadata.Releases()
            {
                versionId = release.ReleaseId,
                versionName = release.VersionName ?? "",
                patchNotes = release.PatchNotes,

                platforms = release.Downloads.Select(d => new GameLauncherMetadata.Releases.Platform
                {
                    platform = d.Platform,
                    size = d.DownloadSize,
                    link = d.ReleaseEngineManifest,
                    entrypoint = d.EntryPoint

                }).ToArray()
            };
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
}
