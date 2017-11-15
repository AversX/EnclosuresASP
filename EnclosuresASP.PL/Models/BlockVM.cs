using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.ComponentModel.DataAnnotations;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.PL.Models
{
    public class BlockVM
    {
        public int BlockID { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string UID { get; set; }

        public int TypicalBlockID { get; set; }

        public IEnumerable<SelectListItem> TypicalBlocks { get; set; }

        public int EnclosureID { get; set; }
    }
}