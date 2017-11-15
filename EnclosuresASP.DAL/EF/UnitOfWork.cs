using EnclosuresASP.DAL.Entities;
using System;

namespace EnclosuresASP.DAL.EF
{
    public class UnitOfWork : IDisposable
    {
        private DataContext context = new DataContext();
        private GenericRepository<Enclosure> enclosureRepository;
        private GenericRepository<Employe> employeRepository;
        private GenericRepository<Position> positionRepository;
        private GenericRepository<ACS> acsRepository;
        private GenericRepository<EnclosureFile> enclosureFilesRepository;
        private GenericRepository<Block> blockRepository;
        private GenericRepository<TypicalBlock> typicalBlockRepository;

        public GenericRepository<Enclosure> EnclosureRepository
        {
            get
            {
                return this.enclosureRepository ?? new GenericRepository<Enclosure>(context);
            }
        }

        public GenericRepository<Employe> EmployeRepository
        {
            get
            {
                return this.employeRepository ?? new GenericRepository<Employe>(context);
            }
        }

        public GenericRepository<Position> PositionRepository
        {
            get
            {
                return this.positionRepository ?? new GenericRepository<Position>(context);
            }
        }

        public GenericRepository<ACS> ACSRepository
        {
            get
            {
                return this.acsRepository ?? new GenericRepository<ACS>(context);
            }
        }

        public GenericRepository<EnclosureFile> EnclosureFilesRepository
        {
            get
            {
                return this.enclosureFilesRepository ?? new GenericRepository<EnclosureFile>(context);
            }
        }

        public GenericRepository<Block> BlockRepository
        {
            get
            {
                return this.blockRepository ?? new GenericRepository<Block>(context);
            }
        }

        public GenericRepository<TypicalBlock> TypicalBlockRepository
        {
            get
            {
                return this.typicalBlockRepository ?? new GenericRepository<TypicalBlock>(context);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
