using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnclosuresASP.DAL.Entities
{
    public class Block
    {
        public int BlockID { get; set; }
        public string UID { get; set; } //уид предполагается полностью уникальным

        [Required]
        public int EnclosureID { get; set; }

        public virtual TypicalBlock BlockName { get; set; }

        public Guid BlockGuid { get; set; }

        public Block()
        {
            BlockGuid = Guid.NewGuid();
        }
    }
}
