using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace EnclosuresASP.PL.Models
{
    public class EnclosureVM
    {
        public int EnclosureID { get; set; }

        public string Username { get; set; }
        
        public string Number { get; set; }
        
        public string ElisNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy'-'MM'-'dd}", ApplyFormatInEditMode = true, NullDisplayText = "Установите дату")]
        public DateTime? AcceptanceDate { get; set; }

        public int? EmployeID { get; set; }
        public IEnumerable<SelectListItem> Employes { get; set; }

        public string Lvl1Password { get; set; }
        public string Lvl2Password { get; set; }
        public string Lvl3Password { get; set; }
        public string Lvl4Password { get; set; }
        public string Lvl5Password { get; set; }

        public string Object { get; set; }

        public string Comment { get; set; }

        public string FilesJSON { get; set; }

        [Required]
        public Guid Version { get; set; }
    }
}