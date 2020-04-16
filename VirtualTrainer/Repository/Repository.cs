using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer.Interfaces;

namespace VirtualTrainer.Repository
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class//, IEntity
    {
        public DbContext context;
        public DbSet<TEntity> dbSet;

        public Repository(DbContext DbContext)
        {
            this.context = DbContext;
            this.dbSet = context.Set<TEntity>();
        }

        //private IList<TEntity> LoadNavigationFields<TEntity>(IList<TEntity> entities) where TEntity : class
        //{
        //    foreach (TEntity entity in entities)
        //    {
        //        PerformEagerLoading<TEntity>(entity, this.context);
        //    }
        //    return entities;
        //}
        //private TEntity LoadNavigationFields<TEntity>(TEntity entity) where TEntity : class
        //{
        //    PerformEagerLoading<TEntity>(entity, this.context);
        //    return entity;
        //}
        //private void PerformEagerLoading<TEntity>(TEntity entity, ObjectContext context) where TEntity : class
        //{
        //    PropertyInfo[] properties = typeof(TEntity).GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        object[] keys = property.GetCustomAttributes(typeof(NavigationFieldAttribute), true);
        //        if (keys.Length > 0)
        //        {
        //            context.LoadProperty(entity, property.Name);
        //        }
        //    }
        //}

        public IQueryable<TEntity> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            IQueryable<TEntity> set = dbSet.AsQueryable();

            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            return set;
        }

        public TEntity GetById(int Id)
        {
            return null;
            //return Single(x => x.Id == Id);
        }

        public virtual void Create(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity, bool updateNulls = false, params string[] allowNullsList)
        {
            try
            {
                //var entry = context.Entry(entity);
                //if (entry.State == EntityState.Detached)
                //{
                //    var attachedEntity = GetById(entry.Entity.Id);
                //    if (attachedEntity != null)
                //    {
                //        var attachedEntry = context.Entry(attachedEntity);
                //        attachedEntry.CurrentValues.SetValues(entity);
                //        //exclude nulls
                //        foreach (var prop in attachedEntry.OriginalValues.PropertyNames)
                //        {
                //            //only update nulls if explicitly told to
                //            //TODO: Replace 'entry.Property(prop).Name.Substring(entry.Property(prop).Name.Length - 2).Equals("ID"))'
                //            // by checking if the property is foreign key
                //            bool allowNullsForThisProperty = allowNullsList != null && allowNullsList.Contains(prop);
                //            bool isNullDontUpdate = ((entry.Property(prop).CurrentValue == null || entry.Property(prop).CurrentValue.Equals("scnull")) && !updateNulls && !allowNullsForThisProperty);
                //            bool isZeroAndID = (Convert.ToString(entry.Property(prop).CurrentValue) == "0" && entry.Property(prop).Name.Substring(entry.Property(prop).Name.Length - 2).Equals("Id"));
                //            if (isNullDontUpdate || isZeroAndID)
                //            {
                //                attachedEntry.Property(prop).CurrentValue = attachedEntry.Property(prop).OriginalValue;
                //                attachedEntry.Property(prop).IsModified = false;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    dbSet.Attach(entity);
                //    entry.State = EntityState.Modified;
                //}
            }
            catch (OptimisticConcurrencyException ex)
            {
                //TODO: Add injectable exception handling module
                //Logger.WriteException(ex, string.Format("'Update' had a problem: {0}. entity Name:{1}", ex.Message, entity.GetType().Name));
                throw;
            }
        }

        //public virtual void Update(TEntity entity, int Id, bool updateNulls = false)
        //{
        //    var entry = context.Entry(entity);
        //    if (entry.State == EntityState.Detached)
        //    {
        //        var attachedEntity = dbSet.Find(Id);  //GetById(entry.Entity.Id);
        //        if (attachedEntity != null)
        //        {
        //            Update(entity, attachedEntity, updateNulls);
        //        }
        //    }
        //    else
        //    {
        //        dbSet.Attach(entity);
        //        entry.State = EntityState.Modified;
        //    }
        //}
        public virtual void Update(TEntity entity, object PrimaryKey, bool updateNulls = false)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var attachedEntity = dbSet.Find(PrimaryKey);  //GetById(entry.Entity.Id);
                if (attachedEntity != null)
                {
                    Update(entity, attachedEntity, updateNulls);
                }
            }
            else
            {
                dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }
        public virtual void Update(TEntity entity, object PrimaryKeyOne, object PrimaryKeyTwo, bool updateNulls = false)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var attachedEntity = dbSet.Find(PrimaryKeyOne, PrimaryKeyTwo);  //GetById(entry.Entity.Id);
                if (attachedEntity != null)
                {
                    Update(entity, attachedEntity, updateNulls);
                }
            }
            else
            {
                dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }
        public virtual void Update(TEntity entity, object PrimaryKeyOne, object PrimaryKeyTwo, object PrimaryKeyThree, bool updateNulls = false)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var attachedEntity = dbSet.Find(PrimaryKeyOne, PrimaryKeyTwo, PrimaryKeyThree);  //GetById(entry.Entity.Id);
                if (attachedEntity != null)
                {
                    Update(entity, attachedEntity, updateNulls);
                }
            }
            else
            {
                dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }
        private void Update(TEntity entity, TEntity attachedEntity, bool updateNulls = false)
        {
            var entry = context.Entry(entity);
            if (attachedEntity != null)
            {
                var attachedEntry = context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                //exclude nulls
                foreach (var prop in attachedEntry.OriginalValues.PropertyNames)
                {

                    //only update nulls if explicitly told to
                    //TODO: Replace 'entry.Property(prop).Name.Substring(entry.Property(prop).Name.Length - 2).Equals("ID"))'
                    // by checking if the property is foreign key
                    if (((entry.Property(prop).CurrentValue == null || entry.Property(prop).CurrentValue.Equals("scnull")) && !updateNulls)
                        || (Convert.ToString(entry.Property(prop).CurrentValue) == "0" && entry.Property(prop).Name.Substring(entry.Property(prop).Name.Length - 2).Equals("Id")))
                    {
                        attachedEntry.Property(prop).CurrentValue = attachedEntry.Property(prop).OriginalValue;
                        attachedEntry.Property(prop).IsModified = false;
                    }
                }
            }
        }

        public virtual void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(object Id)
        {
            Delete(dbSet.Find(Id));
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {

            IQueryable<TEntity> query = dbSet.Where(predicate).AsQueryable();
            foreach (TEntity obj in query)
            {
                dbSet.Remove(obj);
            }
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria)
        {
            return GetAll().Where(criteria);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> criteria)
        {
            return Find(criteria).FirstOrDefault();
        }

        #region private methods
        private object GetPropertyValue(Type type, string value, bool isNullable = false)
        {
            if (isNullable && string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                // Determine the type of the property
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.String:
                        return value;

                    case TypeCode.Int32:
                        int valueInt;
                        int.TryParse(value, out valueInt);
                        return valueInt;

                    case TypeCode.Decimal:
                        decimal valueDecimal;
                        decimal.TryParse(value, out valueDecimal);
                        return valueDecimal;

                    case TypeCode.DateTime:
                        DateTime valueDateTime;
                        DateTime.TryParse(value, out valueDateTime);
                        return valueDateTime;

                    case TypeCode.Boolean:
                        bool valueBool;
                        bool.TryParse(value, out valueBool);
                        return valueBool;

                    case TypeCode.Object:
                        // Check for nullable type
                        Type underlyingType = Nullable.GetUnderlyingType(type);
                        if (underlyingType != null)
                        {
                            // Type is nullable
                            return GetPropertyValue(underlyingType, value, true);
                        }
                        else
                        {
                            return Convert.ChangeType(value, type);
                        }

                    default:
                        return Convert.ChangeType(value, type);
                }
            }
        }
        #endregion


        public IQueryable<TEntity> GetAllNoTrack()
        {
            return dbSet.AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> GetAllNoTrack(params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            IQueryable<TEntity> set = GetAllNoTrack();

            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            return set;
        }


    }
}
