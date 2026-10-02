using System;
using System.Collections.Generic;

namespace OneFitness.ViewModel
{
    public class PagedResultViewModel<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
