using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchService.Model
{
    public class Image
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}