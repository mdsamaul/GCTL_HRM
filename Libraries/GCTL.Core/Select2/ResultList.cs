using System.Text.Json.Serialization;

namespace GCTL.Core.Select2
{
    public class ResultList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }
}
