using Portfolio.Api.Services;
using Portfolio.Core.Data;
using Portfolio.Core.Models;

namespace Portfolio.Api.Helpers;

public class EntitlementHelper(EntitlementService _entitlements) : IDisposable
{
    private Dictionary<Guid, UserEntitlementInfo[]> userEntitlementsPerProject = new Dictionary<Guid, UserEntitlementInfo[]>();

    public async Task<UserEntitlementInfo[]> GetUserEntitlements(Guid projectId)
    {
        UserEntitlementInfo[]? results;

        if (userEntitlementsPerProject.TryGetValue(projectId, out results))
            return results ?? [];

        if (_entitlements.hasValidCookie)
        {
            try
            {
                results = await _entitlements.Enrich(projectId);
            }
            catch { }
        }

        results ??= [];
        userEntitlementsPerProject[projectId] = results;

        return results;
    }

    public async Task<bool> HasEntitlement(Guid projectId, string key)
        => (await GetUserEntitlements(projectId)).Any(e => e.Key.Equals(key));

    public async Task<bool> HasAccessToDownload(ReleaseModel release)
    {
        if (release.Status != ReleaseStatus.Prerelease)
            return release.Status == ReleaseStatus.Published;

        return await HasEntitlement(release.ProjectId, EntitlementKeys.EarlyAccess);
    }

    public void Dispose()
    {
        // dont dispose service here because it could outlive this
        userEntitlementsPerProject.Clear();
    }
}
