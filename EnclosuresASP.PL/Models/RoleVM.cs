using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using EnclosuresASP.DAL.Identity;

namespace EnclosuresASP.PL.Models
{
    public class RoleVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        public List<AppUser> Users { get; set; }
    }
}