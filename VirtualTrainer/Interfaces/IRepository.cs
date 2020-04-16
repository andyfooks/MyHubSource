using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Interfaces
{
    //public interface IEntity
    //{
    //    //int? Id { get; set; }
    //}
    public interface IRepository<TEntity> where TEntity : class//, IEntity
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllNoTrack();

        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeExpressions);
        IQueryable<TEntity> GetAllNoTrack(params Expression<Func<TEntity, object>>[] includeExpressions);

        TEntity GetById(int Id);
        void Create(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object Id);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        //void Update(TEntity entity, bool updateNulls = false);
        //void Update(TEntity entity, int Id, bool updateNulls = false);
        void Update(TEntity entity, object PrimaryKey, bool updateNulls = false);
        void Update(TEntity entity, object PrimaryKeyOne, object PrimaryKeyTwo, bool updateNulls = false);
        void Update(TEntity entity, object PrimaryKeyOne, object PrimaryKeyTwo, object PrimaryKeyThree, bool updateNulls = false);
        void Update(TEntity entity, bool updateNulls = false, params string[] nullsToUpdate);
       // void Update(int EntityId, string Property, string Value, bool updateNulls = false);
        //TEntity UpdateRelationship<TChild>(TEntity entity, Expression<Func<TEntity, TChild>> mapping) where TChild : class;//, IEntity;
        //IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);
        TEntity Single(Expression<Func<TEntity, bool>> criteria);
    }

    //public interface IEntity
    //{
    //    int Id { get; set; }
    //}

    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;//, IEntity;
        new void Dispose();
    }
}
