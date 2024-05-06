using Core.Repository.Enums;
using Core.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Core.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected ITransaction _transaction;

        public DbContext DbContext { get { return _dbContext; } }

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            if (_transaction == null)
                _transaction = new DbTransaction(_dbContext.Database.BeginTransaction());
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async ValueTask<TEntity> GetByIdAsync(long id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<PagedList> GetPagedReponseAsync(
                                                                        Pagination pagination,
                                                                        Expression<Func<TEntity, bool>> predicate = null,
                                                                        IQueryable<TEntity> customQuery = null,
                                                                        params Expression<Func<TEntity, object>>[] includes
                                                                        )
        {
            IQueryable<TEntity> query = customQuery == null ? _dbContext.Set<TEntity>() : customQuery;

            if (!string.IsNullOrWhiteSpace(pagination.SortOrderColumn))
            {
                Sorted _sort = pagination.SortOrderBy.ToUpper() == "DESC" ? Sorted.DESC : Sorted.ASC;

                var propertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(pagination.SortOrderColumn, StringComparison.InvariantCultureIgnoreCase));
                var param = Expression.Parameter(typeof(TEntity), "x");
                Expression conversion = Expression.Convert(Expression.Property(param, objectProperty), typeof(object));   //important to use the Expression.Convert

                query = _sort == Sorted.ASC ? //Sorted is enum
                        query.OrderBy(Expression.Lambda<Func<TEntity, object>>(conversion, param)) :
                        query.OrderByDescending(Expression.Lambda<Func<TEntity, object>>(conversion, param));
            }
            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            var totalCount = query.Count();

            if (pagination.PageNumber > 0)
            {
                query = query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return new PagedList
            {
                TotalCount = totalCount,
                Data = await query.AsNoTracking().ToListAsync()
            };
        }

        public void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return await query.SingleOrDefaultAsync(predicate);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
                _transaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        public bool IsPropertyExist(dynamic filters, string name)
        {
            if (filters is ExpandoObject)
                return ((IDictionary<string, object>)filters).ContainsKey(name);

            return filters.GetType().GetProperty(name) != null;
        }

        public List<long> GetSplitValues(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return new List<long>();

            return ids.Split(',').Select(long.Parse).ToList();
        }
    }
}
