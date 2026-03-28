using Microsoft.EntityFrameworkCore;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<SchedulingTableRow> Schedulings => Set<SchedulingTableRow>();
        public DbSet<CustomerTableRow> Customers => Set<CustomerTableRow>();
        public DbSet<PlatformTableRow> Platforms => Set<PlatformTableRow>();
        public DbSet<ContentTableRow> Contents => Set<ContentTableRow>();
        public DbSet<ParametersTableRow> Parameters => Set<ParametersTableRow>();
        public DbSet<CustomerPlatformConfigurationTableRow> CustomerPlatforms => Set<CustomerPlatformConfigurationTableRow>();
        public DbSet<SchedulingStatusTableRow> SchedulingStatus => Set<SchedulingStatusTableRow>();



    }
}
