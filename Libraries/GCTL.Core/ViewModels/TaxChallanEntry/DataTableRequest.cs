using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{

    //public class DataTableRequest
    //{
    //    public int Draw { get; set; }
    //    public int Start { get; set; }
    //    public int Length { get; set; }
    //    public DataTableSearch Search { get; set; }
    //}

    //public class DataTableSearch
    //{
    //    public string Value { get; set; }
    //    public string Regex { get; set; }
    //}

    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearch Search { get; set; } = new DataTableSearch();
        public List<DataTableColumn> Columns { get; set; } = new List<DataTableColumn>();
        public List<DataTableOrder> Order { get; set; } = new List<DataTableOrder>();
    }

    public class DataTableSearch
    {
        public string Value { get; set; } = string.Empty;
        public bool Regex { get; set; } = false;
    }

    public class DataTableColumn
    {
        public string Data { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Searchable { get; set; } = true;
        public bool Orderable { get; set; } = true;
        public DataTableSearch Search { get; set; } = new DataTableSearch();
    }

    public class DataTableOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }

    //public class HrmPayMonthlyTaxDepositEntryDto
    //{
    //    public long TaxDepositCode { get; set; }
    //    public string TaxDepositId { get; set; } = string.Empty;
    //    public string EmployeeId { get; set; } = string.Empty;
    //    public string EmpName { get; set; } = string.Empty;
    //    public string DesignationName { get; set; } = string.Empty;
    //    public string TaxChallanNo { get; set; } = string.Empty;
    //    public decimal? TaxDepositAmount { get; set; }
    //    public DateTime? TaxChallanDate { get; set; }
    //    public string ShowTaxChallanDate { get; set; } = string.Empty;
    //    public string SalaryMonthName { get; set; } = string.Empty;
    //    public int? SalaryYear { get; set; }
    //    public string ShowFinancialCodeNo { get; set; } = string.Empty;
    //    public string Remark { get; set; } = string.Empty;
    //}

}
