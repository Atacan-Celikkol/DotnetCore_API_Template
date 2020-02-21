using Core.Extensions;
using Data;
using Data.Extensions;
using Data.Models;
using Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public interface IDataService : IService
    {
        Task<bool> IsAuthorizedForResourceAsync(string resourceId, string userId);

        bool IsAuthorizedForResource(string resourceId, string userId);
    }

    public interface IDataService<TModel> : IDataService
    {
        #region AsyncMethods

        Task<List<TModel>> GetItemsAsync();

        Task<TModel> GetItemAsync(string id, params Expression<Func<TModel, object>>[] includeExpressions);

        Task<TModel> UpdateItemAsync(TModel item);

        Task DeleteItemAsync(string id);

        Task<TModel> CreateItemAsync(TModel item);

        Task<PaginatedData<TModel>> GetItemsAsync(int page, int size,
            Expression<Func<TModel, bool>> whereExpression = null, string sortBy = null, OrderType orderType = OrderType.Default, params Expression<Func<TModel, object>>[] includeExpressions);

        #endregion AsyncMethods

        #region SyncMethods

        List<TModel> GetItems();

        TModel GetItem(string id, params Expression<Func<TModel, object>>[] includeExpressions);

        TModel UpdateItem(TModel user);

        void DeleteItem(string id);

        TModel CreateItem(TModel data);

        PaginatedData<TModel> GetItems(int page, int size,
            Expression<Func<TModel, bool>> whereExpression = null, string sortBy = null, OrderType orderType = OrderType.Default, params Expression<Func<TModel, object>>[] includeExpressions);

        #endregion SyncMethods

        IDbContextTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted);
    }

    public interface IService
    {
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract class DataService<TContext, TModel> : IDataService
        where TContext : class
    {
        public TContext Context { get; set; }

        protected DataService(TContext context)
        {
            Context = context;
        }

        public abstract Task<bool> IsAuthorizedForResourceAsync(string resourceId, string userId);

        public abstract bool IsAuthorizedForResource(string resourceId, string userId);
    }

    public abstract class DataService : DataService<DataContext, object>
    {
        // ReSharper disable once PublicConstructorInAbstractClass
        public DataService(DataContext context) : base(context)
        {
        }
    }

    public class DataService<TModel> : DataService<DataContext, TModel>, IDataService<TModel> where TModel : class, IBaseEntity
    {
        public DataService(DataContext context) : base(context)
        {
        }

        #region Async Methods

        public virtual async Task DeleteItemAsync(string id)
        {
            var item = await Context.Set<TModel>().FindAsync(id);
            var baseEntity = item as BaseEntity;
            if (baseEntity != null)
            {
                baseEntity.IsDeleted = true;
                baseEntity.DeleteDate = DateTimeOffset.UtcNow;
                await Context.SaveChangesAsync();
            }
        }

        public virtual async Task<TModel> GetItemAsync(string id, params Expression<Func<TModel, object>>[] includeExpressions)
        {
            if (includeExpressions.Length > 0)
            {
                var query = Context.Set<TModel>().AsQueryable();
                query = includeExpressions.Aggregate(query, (current, expression) => current.Include(expression));
                return await query.SingleOrDefaultAsync(t => t.Id == id);
            }

            return await Context.Set<TModel>().SingleOrDefaultAsync(t => t.Id == id);
        }

        public virtual async Task<TModel> UpdateItemAsync(TModel user)
        {
            var entity = user as IAuditedEntity;
            await Context.SaveChangesAsync();

            if (entity == null) return user;
            var item = entity;
            item.LastModifiedDate = DateTimeOffset.UtcNow;
            await Context.SaveChangesAsync();

            return user;
        }

        public override Task<bool> IsAuthorizedForResourceAsync(string resourceId, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TModel> CreateItemAsync(TModel item)
        {
            Context.Set<TModel>().Add(item);
            await Context.SaveChangesAsync();
            Context.Set<TModel>().Attach(item);
            return item;
        }

        public virtual Task<PaginatedData<TModel>> GetItemsAsync(
            int page, int size,
            Expression<Func<TModel, bool>> whereExpression = null,
            string sortBy = null, OrderType orderType = OrderType.Default,
            params Expression<Func<TModel, object>>[] includeExpressions)
        {
            var query = Context.Set<TModel>().AsQueryable();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }
            if (includeExpressions.Length > 0)
            {
                query = includeExpressions.Aggregate(query, (current, expression) => current.Include(expression));
            }
            if (!string.IsNullOrEmpty(sortBy) && orderType != OrderType.Default)
            {
                switch (orderType)
                {
                    case OrderType.Asc:
                        query = query.OrderBy($"{sortBy} Asc");
                        break;

                    case OrderType.Desc:
                        query = query.OrderBy($"{sortBy} Desc");
                        break;
                }
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }
            return query.GetPaginatedResultAsync(page, size);
        }

        public virtual async Task<List<TModel>> GetItemsAsync()
        {
            var set = Context.Set<TModel>();
            return await set.ToListAsync();
        }

        #endregion Async Methods

        #region Sync Methods

        public override bool IsAuthorizedForResource(string resourceId, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteItem(string id)
        {
            var item = Context.Set<TModel>().Find(id);
            var baseEntity = item as BaseEntity;
            if (baseEntity != null)
            {
                baseEntity.IsDeleted = true;
                baseEntity.DeleteDate = DateTimeOffset.UtcNow;
            }
            Context.SaveChanges();
        }

        public virtual TModel GetItem(string id, params Expression<Func<TModel, object>>[] includeExpressions)
        {
            if (includeExpressions.Length > 0)
            {
                var query = Context.Set<TModel>().AsQueryable();
                query = includeExpressions.Aggregate(query, (current, expression) => current.Include(expression));
                return query.SingleOrDefault(t => t.Id == id);
            }

            return Context.Set<TModel>().SingleOrDefault(t => t.Id == id);
        }

        public virtual TModel UpdateItem(TModel user)
        {
            var entity = user as IAuditedEntity;
            Context.SaveChanges();

            if (entity == null) return user;
            var item = entity;
            item.LastModifiedDate = DateTimeOffset.UtcNow;
            Context.SaveChanges();
            return user;
        }

        public virtual TModel CreateItem(TModel item)
        {
            Context.Set<TModel>().Add(item);
            item = Context.Set<TModel>().Add(item).Entity;
            Context.SaveChanges();
            item = Context.Set<TModel>().AsNoTracking().SingleOrDefault(t => t.Id == item.Id);
            return item;
        }

        public virtual PaginatedData<TModel> GetItems(
            int page, int size,
            Expression<Func<TModel, bool>> whereExpression = null,
            string sortBy = null, OrderType orderType = OrderType.Default,
            params Expression<Func<TModel, object>>[] includeExpressions)
        {
            var query = Context.Set<TModel>().AsQueryable();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }
            if (includeExpressions.Length > 0)
            {
                query = includeExpressions.Aggregate(query, (current, expression) => current.Include(expression));
            }
            if (!string.IsNullOrEmpty(sortBy) && orderType != OrderType.Default)
            {
                switch (orderType)
                {
                    case OrderType.Asc:
                        query = query.OrderBy($"{sortBy} Asc");
                        break;

                    case OrderType.Desc:
                        query = query.OrderBy($"{sortBy} Desc");
                        break;
                }
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }
            return query.GetPaginatedResult(page, size);
        }

        public virtual List<TModel> GetItems()
        {
            var set = Context.Set<TModel>();
            return set.ToList();
        }

        #endregion Sync Methods

        public IDbContextTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            return Context.Database.BeginTransaction(level);
        }
    }
}