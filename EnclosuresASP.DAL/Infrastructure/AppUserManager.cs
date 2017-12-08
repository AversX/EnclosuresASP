using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Threading.Tasks;

namespace EnclosuresASP.DAL.Infrastructure
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            IdentityContext db = context.Get<IdentityContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            manager.UserValidator = new CustomUserValidator(manager)
            {
                AllowOnlyAlphanumericUserNames = true
            };

            return manager;
        }
    }

    public class CustomUserValidator : UserValidator<AppUser>
    {
        public CustomUserValidator(AppUserManager manager)
            : base(manager)
        { }

        public override async Task<IdentityResult> ValidateAsync(AppUser user)
        {
            IdentityResult result = await base.ValidateAsync(user);
            return result;
        }
    }
}
