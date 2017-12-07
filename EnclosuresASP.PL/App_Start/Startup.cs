using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using EnclosuresASP.BLL.Infrastructure;
using EnclosuresASP.DAL.EF;
using System;

namespace EnclosuresASP.PL.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(IdentityContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                ExpireTimeSpan = /*TimeSpan.FromSeconds(10),*/ TimeSpan.FromMinutes(30),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
            });
        }
    }
}