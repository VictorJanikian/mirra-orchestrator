using Microsoft.EntityFrameworkCore;
using Mirra_Orchestrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirra_Orchestrator.Repository
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Scheduling> Schedulings => Set<Scheduling>();



    }
}
