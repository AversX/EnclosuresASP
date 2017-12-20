using System;

namespace EnclosuresASP.DAL.EF
{
    public interface IVersionedRow
    {
        Guid Version { get; set; }
    }
}
