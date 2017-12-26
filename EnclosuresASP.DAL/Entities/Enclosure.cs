using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class Enclosure : IVersionedRow
    {
        public bool Temporary { get; set; } 

        public int EnclosureID { get; set; }

        public string Number { get; set; }

        public string ElisNumber { get; set; }

        public string AcceptanceDate { get; set; }

        public virtual Employe Supervisor { get; set; }

        public string Lvl1Password { get; set; }
        public string Lvl2Password { get; set; }
        public string Lvl3Password { get; set; }
        public string Lvl4Password { get; set; }
        public string Lvl5Password { get; set; }

        public string Object { get; set; }

        public string Comment { get; set; }

        public string Username { get; set; }

        public virtual ICollection<EnclosureFile> Files { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }
    }
}
