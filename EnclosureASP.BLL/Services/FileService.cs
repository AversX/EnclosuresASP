using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnclosuresASP.DAL.EF;
using EnclosuresASP.DAL.Entities;


namespace EnclosuresASP.BLL.Services
{
    public class FileService
    {
        public UnitOfWork unitOfWork;

        public FileService()
        {
            unitOfWork = new UnitOfWork();
        }

        public FileService(UnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public virtual IEnumerable<EnclosureFile> Get(Expression<Func<EnclosureFile, bool>> filter = null, Func<IQueryable<EnclosureFile>, IOrderedQueryable<EnclosureFile>> orderBy = null, string includeProperties = "")
        {

            return unitOfWork.EnclosureFilesRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual EnclosureFile GetByID(object id)
        {
            return unitOfWork.EnclosureFilesRepository.GetByID(id);
        }

        public virtual void Insert(EnclosureFile entity)
        {
            unitOfWork.EnclosureFilesRepository.Insert(entity);
        }

        public virtual void Delete(object id, Guid newVersionGuid)
        {
            unitOfWork.EnclosureFilesRepository.Delete(id, newVersionGuid);
        }

        public virtual void Delete(EnclosureFile entityToDelete)
        {
            unitOfWork.EnclosureFilesRepository.Delete(entityToDelete);
        }

        public virtual void Update(EnclosureFile entityToUpdate)
        {
            unitOfWork.EnclosureFilesRepository.Update(entityToUpdate);
        }

        public virtual void Save()
        {
            unitOfWork.Save();
        }
    }
}
