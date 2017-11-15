using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.DAL.Entities
{
    public class EnclosureFile
    {
        public int EnclosureFileID { get; set; }
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }
        public string MimeType { get; set; }

        [Required]
        public int EnclosureID { get; set; }
    }
}
