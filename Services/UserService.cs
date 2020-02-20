using Core.Exceptions;
using Core.Extensions;
using Core.Utils;
using Data;
using Data.Extensions;
using Data.Models.Enums;
using Data.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService : IDataService<User>
    {
        Task<PaginatedData<User>> SearchAsync(int page, int size, string q, string sortBy = null, OrderType orderType = OrderType.Default);
        Task<List<Role>> GetUserRolesAsync(string userId);
    }

    public class UserService : DataService<User>, IUserService
    {
        public UserService(DataContext context) : base(context)
        {
        }

        public override async Task DeleteItemAsync(string id)
        {
            var user = await Context.Users.SingleOrDefaultAsync(t => t.Id == id);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            user.IsDeleted = true;
            user.DeleteDate = DateTimeOffset.UtcNow;
            user.UserName = $"deleted_{user.UserName}";
            user.NormalizedEmail= $"deleted_{user.NormalizedEmail}";
            user.NormalizedUserName= $"deleted_{user.NormalizedUserName}";
            user.PhoneNumber = $"deleted_{user.PhoneNumber}";
            user.Email = $"deleted_{user.Email}";
            user.IsActive = false;
            user.EmailConfirmed = false;
            user.PhoneNumberConfirmed = false;
            user.LastModifiedDate = DateTimeOffset.UtcNow;
            await Context.SaveChangesAsync();
        }

        public async Task<PaginatedData<User>> SearchAsync(int page, int size, string q, string sortBy = null, OrderType orderType = OrderType.Default)
        {
            var query = Context.Users.Where(x => x.IsDeleted == false).AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(x => x.Id == q || x.PhoneNumber.Contains(q));
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
                query = query.OrderBy(t => t.CreateDate);
            }
            return await query.GetPaginatedResultAsync(page, size);
        }

        public async Task<List<Role>> GetUserRolesAsync(string userId)
        {
            var roleIds = await Context.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).ToListAsync();
            return await Context.Roles.Where(t => roleIds.Contains(t.Id)).ToListAsync();
        }
    }
}
