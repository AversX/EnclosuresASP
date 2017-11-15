using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;


namespace EnclosuresASP.BLL.Services
{
    public class EnclosureService
    {
        public UnitOfWork unitOfWork;

        public EnclosureService()
        {
            unitOfWork = new UnitOfWork();
        }

        public EnclosureService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<Enclosure> Get(Expression<Func<Enclosure, bool>> filter = null, Func<IQueryable<Enclosure>, IOrderedQueryable<Enclosure>> orderBy = null, string includeProperties = "")
        {
            
            return unitOfWork.EnclosureRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual Enclosure GetByID(object id)
        {
            return unitOfWork.EnclosureRepository.GetByID(id);
        }

        public virtual void Insert(Enclosure entity)
        {
            unitOfWork.EnclosureRepository.Insert(entity);
        }

        public virtual void Delete(object id)
        {
            unitOfWork.EnclosureRepository.Delete(id);
        }

        public virtual void Delete(Enclosure entityToDelete)
        {
            unitOfWork.EnclosureRepository.Delete(entityToDelete);
        }

        public virtual void Update(Enclosure entityToUpdate)
        {
            unitOfWork.EnclosureRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
