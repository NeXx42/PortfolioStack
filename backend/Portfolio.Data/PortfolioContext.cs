using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Models;

namespace Portfolio.Data;

public class PortfolioContext : DbContext
{
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }

    public DbSet<ProjectModel> Projects { get; set; }

    public DbSet<ReleaseModel> Releases => Set<ReleaseModel>();
    public DbSet<ReleaseDownloadModel> ReleaseDownloads => Set<ReleaseDownloadModel>();

    public DbSet<TagModel> Tags { get; set; }
    public DbSet<ProjectTagModel> ProjectTags { get; set; }

    public DbSet<ProjectElementModel> Elements { get; set; }
    public DbSet<ProjectElementParameterModel> ElementsParameters { get; set; }

    public DbSet<LinkModel> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReleaseDownloadModel>().HasKey(d => new { d.ProjectId, d.ReleaseId, d.Platform });
        modelBuilder.Entity<ReleaseDownloadModel>().HasOne(d => d.MetaData).WithMany(r => r.Downloads);

        modelBuilder.Entity<ReleaseModel>().HasKey(r => new { r.ProjectId, r.ReleaseId });
        modelBuilder.Entity<ReleaseModel>().HasOne(m => m.Project).WithMany(p => p.Releases);
    }
}
