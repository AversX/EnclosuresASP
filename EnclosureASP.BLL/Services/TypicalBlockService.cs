using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.BLL.Services
{
    public class TypicalBlockService
    {
        public UnitOfWork unitOfWork = new UnitOfWork();

        public TypicalBlockService()
        {
            unitOfWork = new UnitOfWork();
        }

        public TypicalBlockService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<TypicalBlock> Get(Expression<Func<TypicalBlock, bool>> filter = null, Func<IQueryable<TypicalBlock>, IOrderedQueryable<TypicalBlock>> orderBy = null, string includeProperties = "")
        {

            return unitOfWork.TypicalBlockRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual TypicalBlock GetByID(object id)
        {
            return unitOfWork.TypicalBlockRepository.GetByID(id);
        }

        public virtual void Insert(TypicalBlock entity)
        {
            unitOfWork.TypicalBlockRepository.Insert(entity);
        }

        public virtual void Delete(object id, Guid newVersionGuid)
        {
            unitOfWork.TypicalBlockRepository.Delete(id, newVersionGuid);
        }

        public virtual void Delete(TypicalBlock entityToDelete)
        {
            unitOfWork.TypicalBlockRepository.Delete(entityToDelete);
        }

        public virtual void Update(TypicalBlock entityToUpdate)
        {
            unitOfWork.TypicalBlockRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
