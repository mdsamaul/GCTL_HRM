using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Loggers;
using GCTL.Data.Models;
using System.Linq.Dynamic.Core;

namespace GCTL.Service.Loggers
{
    public static class LogsExtensions
    {
        public static IQueryable<Logs> ApplyFilter(this IQueryable<Logs> query, DataTablesOptions<LogFilterRequest> options)
        {
            if (options.Filter == null)
            {
                return query;
            }

            query = query.Where(x => x.Level == options.Filter.LogLevel.ToString());
            return query;
        }
        public static IQueryable<Logs> ApplySearch(this IQueryable<Logs> query, DataTablesOptions options)
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
                             (!string.IsNullOrWhiteSpace(x.Message) && x.Message.ToLower().Contains(searchTerm))
                             || (!string.IsNullOrWhiteSpace(x.Exception) && x.Exception.Contains(searchTerm)));
            }

            return query;
        }
        public static IQueryable<Logs> ApplySort(this IQueryable<Logs> query, DataTablesOptions options)
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
                    SortBy = "Id",
                    SortDirection = "desc"
                });
            }

            return sortOptions.Aggregate(query, (current, option) => current.OrderBy($"{option.SortBy} {option.SortDirection}"));
        }
    }
}
