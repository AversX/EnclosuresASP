using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class Position : IVersionedRow
    {
        public int PositionID { get; set; }
        public string PosName { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }
    }
}
