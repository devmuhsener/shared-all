using Microsoft.EntityFrameworkCore;
using Shared.Activities.Domain;
using Shared.EntityFramework.Extensions;

namespace Shared.Activities.Infra.Persistence.Context;

public class ActivityDbContext(DbContextOptions<ActivityDbContext> options) : DbContext(options)
{
    public DbSet<UserActivity> Activities { get; set; }


    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("activities");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivityDbContext).Assembly);
        modelBuilder.ApplySoftDeleteFilter();
        
    }
}