using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.DAL.Entities
{
    public class Enclosure
    {
        public bool Temporary { get; set; } 

        public int EnclosureID { get; set; }

        [Required]
        public string Number { get; set; }

        public virtual Employe Supervisor { get; set; }

        public string RootLogin { get; set; }
        public string RootPassword { get; set; }

        public string Username { get; set; }

        public virtual ICollection<EnclosureFile> Files { get; set; }
        public virtual ICollection<ACS> ACSs { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
    }
}
