using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using EnclosuresASP.DAL.Identity;
using MySql.Data.Entity;
using EnclosuresASP.DAL.Infrastructure;
using Microsoft.AspNet.Identity;

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
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            string adminRoleName = "Administrator";
            string userName = "Root";
            string password = "password";

            if (!roleMgr.RoleExists(adminRoleName))
            {
                roleMgr.Create(new AppRole(adminRoleName));
            }

            string userRoleName = "User";
            if (!roleMgr.RoleExists(userRoleName))
            {
                roleMgr.Create(new AppRole(userRoleName));
            }

            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                IdentityResult r = userMgr.Create(new AppUser { UserName = userName }, password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, adminRoleName))
            {
                userMgr.AddToRole(user.Id, adminRoleName);
            }
        }
    }
}
