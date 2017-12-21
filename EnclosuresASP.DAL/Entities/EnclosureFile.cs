using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class EnclosureFile : IVersionedRow
    {
        public int EnclosureFileID { get; set; }
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }
        public string MimeType { get; set; }
        public bool Temporary { get; set; }
        public string Username { get; set; }

        [Required]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Version { get; set; }

        [Required]
        public int EnclosureID { get; set; }
    }
}
