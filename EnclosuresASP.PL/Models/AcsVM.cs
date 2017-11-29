using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.ComponentModel.DataAnnotations;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.PL.Models
{
    public class AcsVM
    {
        public int ACSID { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Code { get; set; }

        public int EnclosureID { get; set; }
    }
}