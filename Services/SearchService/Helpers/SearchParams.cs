using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchService.Helpers
{
    public class SearchParams
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public string Sex { get; set; }
        public string Type { get; set; }
        public string OrderBy { get; set; }
        public string FilterBy { get; set; }
    }
}