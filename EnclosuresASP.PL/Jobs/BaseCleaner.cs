using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using EnclosuresASP.DAL.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;

namespace EnclosuresASP.PL.Jobs
{
    public class BaseCleaner : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            EnclosureService enclosureService = new EnclosureService();
            IEnumerable<IGrouping<string, Enclosure>> groups = enclosureService.Get().Where(x => x.Temporary == true).GroupBy(m => m.Username).ToList();
            foreach (IGrouping<string, Enclosure> group in groups)
            {
                IdentityContext identityContext = new IdentityContext();
                AppUser appUser = identityContext.Users.FirstOrDefault(x => x.UserName==group.Key);
                DateTime dt = DateTime.Parse(appUser.LastActivity);
                if (DateTime.Now.Subtract(dt).TotalMinutes >= 30.0 && group.Count() > 0)
                    foreach (Enclosure enclosure in group)
                    {
                        if (enclosure != null)
                            enclosureService.Delete(enclosure);
                    }
            }
            enclosureService.Save();
        }
    }
}