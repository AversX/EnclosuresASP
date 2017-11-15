using EnclosuresASP.DAL.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EnclosuresASP.PL.Models
{
    public class EnclosureVM
    {
        public int EnclosureID { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Number { get; set; }

        public int EmployeID { get; set; }
        public IEnumerable<SelectListItem> Employes { get; set; }

        public string RootLogin { get; set; }
        public string RootPassword { get; set; }

        public List<Block> Blocks { get; set; }

        public List<ACS> ACSs { get; set; }

        public string JsonFiles { get; set; }
    }
}