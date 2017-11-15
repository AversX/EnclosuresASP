using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.BLL.Services
{
    public class ACSService
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public ACSService()
        {
            unitOfWork = new UnitOfWork();
        }

        public ACSService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<ACS> Get(Expression<Func<ACS, bool>> filter = null, Func<IQueryable<ACS>, IOrderedQueryable<ACS>> orderBy = null, string includeProperties = "")
        {

            return unitOfWork.ACSRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual ACS GetByID(object id)
        {
            return unitOfWork.ACSRepository.GetByID(id);
        }

        public virtual void Insert(ACS entity)
        {
            unitOfWork.ACSRepository.Insert(entity);
        }

        public virtual void Delete(object id)
        {
            unitOfWork.ACSRepository.Delete(id);
        }

        public virtual void Delete(ACS entityToDelete)
        {
            unitOfWork.ACSRepository.Delete(entityToDelete);
        }

        public virtual void Update(ACS entityToUpdate)
        {
            unitOfWork.ACSRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
