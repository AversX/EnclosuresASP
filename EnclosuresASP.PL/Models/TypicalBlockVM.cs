using EnclosuresASP.DAL.Entities;
using System.Collections.Generic;

namespace EnclosuresASP.PL.Models
{
    public class TypicalBlockVM
    {
        public int TypicalBlockID { get; set; }
        public string BlockName { get; set; }
        public List<Block> Blocks { get; set; }
    }
}