using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.BLL.Services
{
    public class PositionService
    {
        public UnitOfWork unitOfWork;

        public PositionService()
        {
            unitOfWork = new UnitOfWork();
        }

        public PositionService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<Position> Get(Expression<Func<Position, bool>> filter = null, Func<IQueryable<Position>, IOrderedQueryable<Position>> orderBy = null, string includeProperties = "")
        {
            return unitOfWork.PositionRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual Position GetByID(object id)
        {
            return unitOfWork.PositionRepository.GetByID(id);
        }

        public virtual void Insert(Position entity)
        {
            unitOfWork.PositionRepository.Insert(entity);
        }

        public virtual void Delete(object id)
        {
            unitOfWork.PositionRepository.Delete(id);
        }

        public virtual void Delete(Position entityToDelete)
        {
            unitOfWork.PositionRepository.Delete(entityToDelete);
        }

        public virtual void Update(Position entityToUpdate)
        {
            unitOfWork.PositionRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
