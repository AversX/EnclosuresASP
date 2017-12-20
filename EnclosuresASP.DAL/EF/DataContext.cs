using System.Data.Entity;
using EnclosuresASP.DAL.Entities;
using MySql.Data.Entity;
using System;

namespace EnclosuresASP.DAL.EF
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
            Database.ExecuteSqlCommand("SET GLOBAL max_allowed_packet = 1073741824");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enclosure>().HasMany(e => e.Blocks).WithOptional();
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var concurrencyTokenEntries = ChangeTracker.Entries<IVersionedRow>();
            foreach (var entry in concurrencyTokenEntries)
            {
                if (entry.State == EntityState.Unchanged)
                {
                    continue;
                }
                entry.Entity.Version = Guid.NewGuid();
            }

            return base.SaveChanges();
        }

        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EnclosureFile> EnclosureFiles { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<TypicalBlock> TBlocks { get; set; }
    }
}
