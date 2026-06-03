using GCTL.Core.DataTables;
using GCTL.Data.Models;
using System.Linq.Dynamic.Core;

namespace GCTL.Service.UnitTypes
{
    public static class UnitTypeExtensions
    {
        public static IQueryable<RmgProdDefUnitType> ApplySearch(this IQueryable<RmgProdDefUnitType> query, DataTablesOptions options)
        {
            var searchableColumns = options.GetSearchableColums();
            if (!searchableColumns.Any())
            {
                return query;
            }

            var searchTerm = options.Search.Value;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                             (!string.IsNullOrWhiteSpace(x.UnitTypeName) && x.UnitTypeName.ToLower().Contains(searchTerm))
                             || (!string.IsNullOrWhiteSpace(x.UnitTypId) && x.UnitTypId.Contains(searchTerm)));
            }

            return query;
        }
        public static IQueryable<RmgProdDefUnitType> ApplySort(this IQueryable<RmgProdDefUnitType> query, DataTablesOptions options)
        {
            var sortableColumns = options.GetSortableColumns().ToArray();
            if (!sortableColumns.Any())
            {
                return query;
            }

            IEnumerable<SortOptions> sortOptions;

            if (options.Order != null)
            {
                sortOptions = options.Order.Select(order => new SortOptions
                {
                    SortBy = options.Columns[options.Order[0].Column].Data,
                    SortDirection = options.Order[0].GetSortDirection().ToString()
                });
            }
            else
            {
                sortOptions = options.Order.Select(order => new SortOptions
                {
                    SortBy = "UnitTypId",
                    SortDirection = "desc"
                });
            }

            return sortOptions.Aggregate(query, (current, option) => current.OrderBy($"{option.SortBy} {option.SortDirection}"));
        }
    }
}