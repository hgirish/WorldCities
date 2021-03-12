using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Data
{
    public class ApiResult<T>
    {
        private ApiResult(
            List<T> data, 
            int count,
            int pageIndex,
            int pageSize)
        {
            Data = data;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public List<T> Data { get; }
        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }

        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize)
        {
            var count = await source.CountAsync();
            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
            var data = await source.ToListAsync();

            return new ApiResult<T>(
                data,
                count,
                pageIndex,
                pageSize);
        }
    }
}
