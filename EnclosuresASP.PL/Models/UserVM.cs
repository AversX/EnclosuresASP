using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EnclosuresASP.DAL.Identity;

namespace EnclosuresASP.PL.Models
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public List<string> RoleIDs { get; set; }

        public List<AppRole> Roles { get; set; }
    }
}