using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.PL.Models
{
    public class BlockVM
    {
        public int BlockID { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string UID { get; set; }

        public string Number { get; set; }

        public string SoftwareVersion { get; set; }
        
        public string Comment { get; set; }

        public int? TypicalBlockID { get; set; }
        public IEnumerable<SelectListItem> TypicalBlocks { get; set; }
        public TypicalBlock BlockName { get; set; }

        public int? EnclosureID { get; set; }

        public Guid BlockGuid { get; set; }

        [Required]
        public Guid Version { get; set; }
    }
}