using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.PL.Models
{
    public class PositionVM
    {
        public int PositionID { get; set; }
        public string PosName { get; set; }
        public List<Employe> Employes { get; set; }
    }
}