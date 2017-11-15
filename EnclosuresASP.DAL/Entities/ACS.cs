using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.DAL.Entities
{
    public class ACS//СКУД
    {
        public int ACSID { get; set; }
        public string Code { get; set; } 

        [Required]
        public int EnclosureID { get; set; }
    }
}
