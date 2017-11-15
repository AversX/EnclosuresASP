using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.PL.Models
{
    public class LoginVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}