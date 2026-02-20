using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Models;

namespace Portfolio.Data;

public class PortfolioContext : DbContext
{
    public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
}
