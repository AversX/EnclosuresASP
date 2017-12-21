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

        [Required]
        public string Number { get; set; }

        public virtual Employe Supervisor { get; set; }

        public string RootLogin { get; set; }
        public string RootPassword { get; set; }

        public string Username { get; set; }

        public virtual ICollection<EnclosureFile> Files { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }
    }
}
