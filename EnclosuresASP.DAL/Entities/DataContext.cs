using System.Data.Entity;
using WebEnclosures.DAL.Entities;
using MySql.Data.Entity;

namespace WebEnclosures.DAL.EF
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
        }

        
        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<AccessControl> AccessControlList { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccessLvl> AccessLvls { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<TypicalBlock> TBlock { get; set; }
    }
}
