using System.Web;
using System.Web.Mvc;
using EnclosuresASP.BLL.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using EnclosuresASP.PL.Models;
using EnclosuresASP.DAL.Identity;
using Microsoft.AspNet.Identity;
using System.Net;
using System;

namespace EnclosuresASP.PL.ActivityTrack
{
    [AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TraceFilterAttribute : ActionFilterAttribute
    {
        private string Parameter { get; set; }
        private ActionDescriptor CurrentAction {get; set;}

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var parameters = filterContext.ActionDescriptor.GetParameters();
            AppUserManager appUserManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            AppUser user = (appUserManager.FindByIdAsync(HttpContext.Current.User.Identity.GetUserId())).Result;
            user.LastActivity = DateTime.Now.ToString();
            IdentityResult result = (appUserManager.UpdateAsync(user)).Result;
        }
    }
}