using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace WorldCities.Data
{
    public class ApiResult<T>
    {
        private ApiResult(
            List<T> data, 
            int count,
            int pageIndex,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string filterColumn ,
            string filterQuery )
        {
            Data = data;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        }

        public List<T> Data { get; }
        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public string SortColumn { get; }
        public string SortOrder { get; }
        public string FilterColumn { get; }
        public string FilterQuery { get; }
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
            int pageSize,
            string sortColumn = null,
            string sortOrder = null,
            string filterColumn = null,
            string filterQuery = null)
        {
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
            {
                source = source.Where($"{filterColumn}.Contains(@0)",filterQuery);
            }
            var count = await source.CountAsync();

            if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder)
                    && sortOrder.ToUpper() == "ASC"
                    ? "ASC"
                    : "DESC";
                source = source.OrderBy($"{sortColumn} {sortOrder}");
            }
            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
            var data = await source.ToListAsync();

            return new ApiResult<T>(
                data,
                count,
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        private static bool IsValidProperty(string propertyName,
            bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
            {
                throw new NotSupportedException(
                    $"ERROR: Property '{propertyName} does not exist.");
            }
            return prop != null;
        }
    }
}
