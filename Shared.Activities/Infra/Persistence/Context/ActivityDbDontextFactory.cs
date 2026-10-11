using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework.Interceptors;

namespace Shared.Activities.Infra.Persistence.Context;

public static class ActivityDbContextFactory
{
    public static ActivityDbContext Create(DbDataSource dataSource)
    {
        return new ActivityDbContext(CreateOptions(dataSource));
    }

    public static DbContextOptions<ActivityDbContext> CreateOptions(DbDataSource dataSource)
    {
        return CreateOptionsBuilder(dataSource).Options;
    }
    
    
    public static DbContextOptionsBuilder<ActivityDbContext> CreateOptionsBuilder(DbDataSource dataSource)
    {
        var builder = new DbContextOptionsBuilder<ActivityDbContext>();
        ConfigureOptions(dataSource, builder);
        return builder;
    }
    
    
    public static void ConfigureOptions(DbDataSource dataSource,
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(dataSource,
            x =>
            {
                x.MigrationsAssembly(typeof(ActivityDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("__EFMigrationsHistory", "activities");
            });
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.AddInterceptors(new AuditInterceptor(), new SoftDeleteInterceptor());
    }





}