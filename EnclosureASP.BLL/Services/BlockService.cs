using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.BLL.Services
{
    public class BlockService
    {
        public UnitOfWork unitOfWork;

        public BlockService()
        {
            unitOfWork = new UnitOfWork();
        }

        public BlockService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<Block> Get(Expression<Func<Block, bool>> filter = null, Func<IQueryable<Block>, IOrderedQueryable<Block>> orderBy = null, string includeProperties = "")
        {

            return unitOfWork.BlockRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual Block GetByID(object id)
        {
            return unitOfWork.BlockRepository.GetByID(id);
        }

        public virtual void Insert(Block entity)
        {
            unitOfWork.BlockRepository.Insert(entity);
        }

        public virtual void Delete(object id)
        {
            unitOfWork.BlockRepository.Delete(id);
        }

        public virtual void Delete(Block entityToDelete)
        {
            unitOfWork.BlockRepository.Delete(entityToDelete);
        }

        public virtual void Update(Block entityToUpdate)
        {
            unitOfWork.BlockRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
