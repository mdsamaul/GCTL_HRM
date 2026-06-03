using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EachGcFilterRequest
{
    public class GcItemDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }

    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public bool More { get; set; }
    }
}
