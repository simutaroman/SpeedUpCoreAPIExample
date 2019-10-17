using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.ViewModels
{
    public class PageViewModel
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PageViewModel(int pageIndex, int pageSize, int totalPages, int totalCount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalCount = totalCount;
        }
    }
}
