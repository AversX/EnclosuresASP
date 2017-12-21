using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;

namespace EnclosuresASP.BLL.Services
{
    public class EmployeService
    {
        public UnitOfWork unitOfWork;

        public EmployeService()
        {
            unitOfWork = new UnitOfWork();
        }

        public EmployeService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<Employe> Get(Expression<Func<Employe, bool>> filter = null, Func<IQueryable<Employe>, IOrderedQueryable<Employe>> orderBy = null, string includeProperties = "")
        {
            return unitOfWork.EmployeRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual Employe GetByID(object id)
        {
            return unitOfWork.EmployeRepository.GetByID(id);
        }

        public virtual void Insert(Employe entity)
        {
            unitOfWork.EmployeRepository.Insert(entity);
        }

        public virtual void Delete(object id, Guid newVersionGuid)
        {
            unitOfWork.EmployeRepository.Delete(id, newVersionGuid);
        }

        public virtual void Delete(Employe entityToDelete)
        {
            unitOfWork.EmployeRepository.Delete(entityToDelete);
        }

        public virtual void Update(Employe entityToUpdate)
        {
            unitOfWork.EmployeRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
