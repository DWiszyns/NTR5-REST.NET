using System;
using System.Collections.Generic;
using System.Linq;

namespace NTR5
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
            if (TotalPages == 0) 
            {
                TotalPages = 1;
            }
            PageIndex = PageIndex <= TotalPages ? PageIndex : TotalPages;
            items = items.Count != 0 ? items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList() : new List<T>();
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}