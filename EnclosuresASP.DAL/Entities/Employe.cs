using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class Employe : IVersionedRow
    {
        public int EmployeID { get; set; }
        public string FullName { get; set; }
        public virtual Position EmpPosition { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }
    }
}
