using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Extensions
{
    public class PaginatedData<T>
    {
        public List<T> Data { get; set; }
        public int TotalItemCount { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }

        public PaginatedData(List<T> data, int totalItemCount, int currentPage, int size)
        {
            Data = data;
            Size = size;
            TotalItemCount = totalItemCount;
            CurrentPage = currentPage;
            if (currentPage - 1 > 0)
            {
                PreviousPage = currentPage - 1;
            }

            if ((double)TotalItemCount / size > currentPage)
            {
                NextPage = currentPage + 1;
            }

        }
    }

    public static class PaginatedDataExtensions
    {
        public static async Task<PaginatedData<T>> GetPaginatedResultAsync<T>(this IQueryable<T> query, int page = 0, int size = 20)
        {
            var items = await query.Skip(page * size).Take(size).ToListAsync();
            var totalItemCount = await query.CountAsync();
            var result = new PaginatedData<T>(items, totalItemCount, page, size);
            return result;
        }

        public static PaginatedData<T> GetPaginatedResult<T>(this IQueryable<T> query, int page = 0, int size = 20)
        {
            var items = query.Skip(page * size).Take(size).ToList();
            var totalItemCount = query.Count();
            var result = new PaginatedData<T>(items, totalItemCount, page, size);
            return result;
        }
    }
}

