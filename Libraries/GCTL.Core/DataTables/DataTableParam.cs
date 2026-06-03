using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.DataTables
{
    public class DataTableParams
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<Column2> Columns { get; set; }
        public List<Order2> Order { get; set; }
        public Search2 Search { get; set; }
    }

    public class Column2
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
    }

    public class Order2
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class Search2
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
