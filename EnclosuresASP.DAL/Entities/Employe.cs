using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace EnclosuresASP.DAL.Entities
{
    public class Employe
    {
        public int EmployeID { get; set; }
        public string FullName { get; set; }
        public virtual Position EmpPosition { get; set; }
    }
}
