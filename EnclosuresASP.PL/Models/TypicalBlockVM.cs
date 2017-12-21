using EnclosuresASP.DAL.Entities;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EnclosuresASP.PL.Models
{
    public class TypicalBlockVM
    {
        public int TypicalBlockID { get; set; }

        public string BlockName { get; set; }
        public List<Block> Blocks { get; set; }

        [Required]
        public Guid Version { get; set; }
    }
}