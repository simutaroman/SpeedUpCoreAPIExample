using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IEnumerable<T> source, int pageSize, int pageIndex = 1)
        {
            TotalCount = source.Count();

            PageIndex = pageIndex;
            PageSize = pageSize == 0 ? TotalCount : pageSize;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.AddRange(source.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private PaginatedList(IEnumerable<T> source, int pageSize, int pageIndex, int totalCount) : base(source)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        }

        public static async Task<PaginatedList<T>> FromIQueryable(IQueryable<T> source, int pageSize, int pageIndex = 1)
        {
            int totalCount = await source.CountAsync();
            pageSize = pageSize == 0 ? totalCount : pageSize;

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (pageIndex > totalPages)
            {
                //return empty list
                return new PaginatedList<T>(new List<T>(), pageSize, pageIndex, totalCount);
            }

            if (pageIndex == 1 && pageSize == totalCount)
            {
                //no paging needed
            }
            else
            {
                source = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            };

            List<T> sourceList = await source.ToListAsync();
            return new PaginatedList<T>(sourceList, pageSize, pageIndex, totalCount);
        }
    }
}