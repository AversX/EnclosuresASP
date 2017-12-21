using EnclosuresASP.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.PL.Models
{
    public class PositionVM
    {
        public int PositionID { get; set; }
        public string PosName { get; set; }
        public List<Employe> Employes { get; set; }

        [Required]
        public Guid Version { get; set; }
    }
}