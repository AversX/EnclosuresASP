using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace EnclosuresASP.DAL.Entities
{
    public class Position
    {
        public int PositionID { get; set; }
        public string PosName { get; set; }
    }
}
