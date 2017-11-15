using Microsoft.AspNet.Identity.EntityFramework;

namespace EnclosuresASP.DAL.Identity
{
    public class AppUser : IdentityUser
    {
       public string LastActivity { get; set; }
    }
}