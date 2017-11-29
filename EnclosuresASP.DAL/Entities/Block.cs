using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.DAL.Entities
{
    public class Block
    {
        public int BlockID { get; set; }
        public string UID { get; set; } //уид предполагается полностью уникальным
        public bool Temporary { get; set; }

        [Required]
        public int EnclosureID { get; set; }

        public virtual TypicalBlock BlockName { get; set; }
    }
}
