using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using EnclosuresASP.DAL.Identity;
using MySql.Data.Entity;

namespace EnclosuresASP.DAL.EF
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class IdentityContext : IdentityDbContext<AppUser>
    {
        public IdentityContext() : base("IdentityContext")
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public static IdentityContext Create()
        {
            return new IdentityContext();
        }
    }

    public class IdentityDbInit : CreateDatabaseIfNotExists<IdentityContext>
    {
        protected override void Seed(IdentityContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(IdentityContext context)
        {

        }
    }
}
