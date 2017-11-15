using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.PL.Models
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}