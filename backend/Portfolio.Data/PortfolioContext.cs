using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Models;

namespace Portfolio.Data;

public class PortfolioContext : DbContext
{
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<ProjectModel> Tags { get; set; }

    public DbSet<ProjectModel> Projects { get; set; }

    public DbSet<ProjectModel> ProjectTags { get; set; }

    public DbSet<ProjectElementModel> Elements { get; set; }
    public DbSet<ProjectElementParameterModel> ElementsParameters { get; set; }

    public DbSet<LinkModel> Links { get; set; }
}
