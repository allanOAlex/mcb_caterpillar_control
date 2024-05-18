using GECA.Client.Console.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GECA.Client.Console.Persistence.DataContext
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DBContext()
        {

        }

        public class DBContextFactory : IDesignTimeDbContextFactory<DBContext>
        {
            DBContext IDesignTimeDbContextFactory<DBContext>.CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

                var connectionString = configuration.GetConnectionString("MSSQL");

                var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
                optionsBuilder.UseSqlServer(connectionString!);

                return new DBContext(optionsBuilder.Options);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureModels(builder);

        }

        protected void ConfigureModels(ModelBuilder modelBuilder)
        {

        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public DbSet<Caterpillar>? Caterpillars { get; set; }
        public DbSet<Spice>? Spices { get; set; }
        public DbSet<Booster>? Boosters { get; set; }


    }
}
