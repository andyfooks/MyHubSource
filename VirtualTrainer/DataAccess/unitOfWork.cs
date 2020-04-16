using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer.Interfaces;
using VirtualTrainer.Repository;

namespace VirtualTrainer.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public VirtualTrainerContext Context;
        public bool isDisposed = false;

        public UnitOfWork()
        {

            Context = new VirtualTrainerContext();
            Context.Configuration.LazyLoadingEnabled = false;
            Context.Configuration.AutoDetectChangesEnabled = true;
            Context.Database.CommandTimeout = 360;

        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class//, IEntity
        {
            return new Repository<TEntity>(Context);
        }

        public void Commit()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}",
                        //                        validationError.PropertyName,
                        //                        validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                this.Context.Dispose();
                isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
