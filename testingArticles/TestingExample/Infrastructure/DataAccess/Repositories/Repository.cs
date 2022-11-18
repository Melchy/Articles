using CommonExtensionMethods;
using Iotc.SharedKernel.DomainExceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public abstract class Repository<TEntity, TKey>
         where TEntity : Entity<TKey>
    {
        private readonly DbContext _dbContext;
        private readonly IMediator _mediator;
        public DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        protected Repository(DbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public virtual async Task<TEntity> GetById(TKey id)
        {
            var entity = await _dbContext.FindAsync<TEntity>(id);
            if (entity == null)
            {
                throw new NotFoundException(ErrorCode.EntityNotFound, $"Entity {typeof(TEntity)} not found");
            }

            return entity;
        }

        public virtual async Task<TEntity> GetByIdOrDefault(TKey id)
        {
            return await _dbContext.FindAsync<TEntity>(id);
        }

        public virtual async Task Remove(TEntity entity)
        {
            _dbContext.Remove(entity);
            await SaveChanges();
        }

        public virtual async Task Add(TEntity entity)
        {
            _dbContext.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Update(TEntity entity)
        {
            _dbContext.Update(entity);
            await ((IEventStore)entity).Events.ForEachAsync(x=>_mediator.Publish(x));
            await SaveChanges();
        }

        public virtual async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.RemoveRange(entities);
            await SaveChanges();
        }

        protected virtual async Task<int> SaveChanges()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
