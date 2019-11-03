using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CatalogContext _dbContext;
        public EfRepository(CatalogContext dbContext)
        {
            _dbContext = dbContext;
        }
        public T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public Task<T> GetByIdAsync(int id)
        {
            return _dbContext.Set<T>().FindAsync(id);
        }

        public T GetSingleBySpec(ISpecification<T> spec)
        {
            return FormatQuery(spec).FirstOrDefault();
        }

        public Task<T> GetSingleBySpecAsync(ISpecification<T> spec)
        {
            return FormatQuery(spec).FirstOrDefaultAsync();
        }

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            var formattedQuery = FormatQuery(spec);

            //return result of the query using the specifications's criteria expression
            return formattedQuery.AsEnumerable();
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public Task<List<T>> ListAllAsync()
        {
            return _dbContext.Set<T>().ToListAsync();
        }

        public Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            var formattedQuery = FormatQuery(spec);

            //return result of the query using the specifications's criteria expression
            return formattedQuery.ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            AddAuditInfo();
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            AddAuditInfo();
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            AddAuditInfo();
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            AddAuditInfo();
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private void AddAuditInfo()
        {
            var entries = _dbContext.ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach(var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.Now;
                }
                ((BaseEntity)entry.Entity).ModifiedAt = DateTime.Now;
            }
        }

        private IQueryable<T> FormatQuery(ISpecification<T> spec)
        {
            //fetch a queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes.Aggregate(_dbContext.Set<T>().AsQueryable(),
                (current, include) => current.Include(include));

            //modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings.Aggregate(queryableResultWithIncludes,
                (current, include) => current.Include(include));

            return secondaryResult.Where(spec.Criteria);
        }
    }
}
