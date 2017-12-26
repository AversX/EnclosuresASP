using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using EnclosuresASP.DAL.EF;

namespace EnclosuresASP.DAL.Entities
{
    public class EnclosureFile
    {
        public int EnclosureFileID { get; set; }
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }
        public string MimeType { get; set; }
        public bool Temporary { get; set; }
        public string Username { get; set; }
        
        public int EnclosureID { get; set; }
        public string EnclosureVersion { get; set; }
    }
}
