using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class Block : IVersionedRow
    {
        public int BlockID { get; set; }

        public string UID { get; set; }

        public string Number { get; set; }

        public string SoftwareVersion { get; set; }

        [Required]
        public int EnclosureID { get; set; }

        public virtual TypicalBlock BlockName { get; set; }

        public Guid BlockGuid { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }

        public Block()
        {
            BlockGuid = Guid.NewGuid();
        }
    }
}
