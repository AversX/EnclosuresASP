using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EnclosuresASP.DAL.Entities;
using System;

namespace EnclosuresASP.PL.Models
{
    public class EmployeVM
    {
        public int EmployeID { get; set; }

        [Required]
        public string FullName { get; set; }

        public Position EmpPosition { get; set; }
        public int? PositionID { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }

        public List<Enclosure> Enclosures { get; set; }

        [Required]
        public Guid Version { get; set; }
    }
}