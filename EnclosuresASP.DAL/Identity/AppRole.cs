using Microsoft.AspNet.Identity.EntityFramework;

namespace EnclosuresASP.DAL.Identity
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name)
        { }
    }
}
