using Core.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        DbContext DbContext { get; }
        ValueTask<TEntity> GetByIdAsync(long id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<PagedList> GetPagedReponseAsync(
                                                        Pagination pagination,
                                                        Expression<Func<TEntity, bool>> predicate = null,
                                                        IQueryable<TEntity> customQuery = null,
                                                        params Expression<Func<TEntity, object>>[] includes
                                                        );
        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        bool IsPropertyExist(dynamic filters, string name);
        List<long> GetSplitValues(string ids);
    }
}
