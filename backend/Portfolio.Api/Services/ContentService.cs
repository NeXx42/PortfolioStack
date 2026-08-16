using AuthEngineShared;
using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class ContentService(CacheService _cache, PortfolioContext _portfolioContext)
{
    private IQueryable<ProjectModel> SearchForProjectWithUser(UserObject? usr)
    {
        if (usr == null)
            return _portfolioContext.Projects.Where(p => p.status == ReleaseStatus.Published);

        if (usr.Role == UserRoles.Admin)
            return _portfolioContext.Projects.AsQueryable();

        // more complex filtering, maybe to licence engine
        return _portfolioContext.Projects.Where(p => p.status == ReleaseStatus.Published);
    }

    public async Task<ProjectDto[]> GetContentForType(UserObject? usr, ProjectType type)
    {
        string cacheKey = $"{type}_{usr?.Id}";

        if (_cache.TryGetValue(cacheKey, out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await SearchForProjectWithUser(usr)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .Where(x => x.projectType == type)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.SetIfNotExists(cacheKey, results);
        return results;
    }

    public async Task<ProjectDto[]> FeaturedContent(UserObject? usr)
    {
        string cacheKey = $"Content_FeaturedContent_{usr?.Id}";

        if (_cache.TryGetValue(cacheKey, out ProjectDto[]? projects) && projects != null)
            return projects;

        ProjectModel[] dbRes = await SearchForProjectWithUser(usr)
            .Include(p => p.Tags)
                .ThenInclude(t => t.Tag)
            .OrderByDescending(p => p.UpdatedDate)
            .Take(3)
            .ToArrayAsync();

        ProjectDto[] results = dbRes.Select(ProjectDto.Map).ToArray();

        _cache.SetIfNotExists(cacheKey, results);
        return results;
    }

    public async Task<ProjectDto?> GetGame(UserObject? usr, string slug)
    {
        string cacheKey = $"Content_Game_{slug}_{usr?.Id}";

        if (_cache.TryGetValue(cacheKey, out ProjectDto? proj))
            return proj;

        ElementType[] excludedMetadata = [ElementType.LauncherMetadata];

        ProjectModel? game = await SearchForProjectWithUser(usr)
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
            _cache.SetIfNotExists(cacheKey, dto);

            return dto;
        }

        _cache.SetIfNotExists<ProjectDto?>(cacheKey, null);
        return null;
    }

    public async Task<GameLauncherMetadata[]> GetGameLauncherMetadata(UserObject? usr, Guid? featured, int? limit)
    {
        string cacheKey = $"Content_Games_{usr?.Id}";

        if (_cache.TryGetValue(cacheKey, out GameLauncherMetadata[]? games))
            return games ?? [];

        var query = SearchForProjectWithUser(usr)
            .Include(p => p.Elements.Where(e => e.Type == ElementType.LauncherMetadata))
                .ThenInclude(p => p.Parameters)
            .Include(p => p.Releases.Where(r => r.Status != ReleaseStatus.Unpublished))
                .ThenInclude(r => r.Downloads)
            .Where(g => g.projectType == ProjectType.Game && g.Elements.Count() == 1)
            .AsQueryable();

        if (featured.HasValue)
            query = query.OrderByDescending(g => g.id == featured.Value)
                        .ThenByDescending(g => g.UpdatedDate);
        else
            query = query.OrderByDescending(g => g.UpdatedDate);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        GameLauncherMetadata[] content = (await query.ToArrayAsync()).Select(Map).ToArray();
        _cache.SetIfNotExists(cacheKey, content);

        return content;

        GameLauncherMetadata Map(ProjectModel model)
        {
            var data = new GameLauncherMetadata()
            {
                id = model.id,
                gameName = model.name,
                about = model.description ?? "",
                genre = model.genre ?? "",

                releaseDate = model.CreatedDate,

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
        if (_cache.TryGetValue("Content_Links", out LinkModel[]? links) && links != null)
            return links;

        LinkModel[] dbRes = await _portfolioContext.Links.ToArrayAsync();

        _cache.SetIfNotExists("Content_Links", dbRes);
        return dbRes;
    }
}
