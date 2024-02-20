using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Find(object id);
        Task<IEnumerable<TEntity>> GetAll();
        IQueryable<TEntity> GetAllQuerable();
        int Count();
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);



        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate);

        Task<int> Add(TEntity entity);




        Task<int> AddRange(IEnumerable<TEntity> entities);



        Task<int> Update(TEntity entity);



        Task<int> UpdateRange(IEnumerable<TEntity> entity);



        Task<int> Remove(TEntity entity);



        Task<int> RemoveRange(IEnumerable<TEntity> entities);


    }
}
