using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Conflux.Paginize
{
    /// <summary>
    /// Filter object for pagination and sorting operations
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Search term for filtering data. This property is available for custom search implementations.
        /// </summary>
        public string? Search { get; set; }
        
        /// <summary>
        /// Page number (1-based). Must be 1 or greater.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        
        /// <summary>
        /// Number of items per page. Must be 1 or greater.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
        
        /// <summary>
        /// List of columns and their sort order, in the order to be applied.
        /// </summary>
        public List<SortColumn>? SortColumns { get; set; }
    }

    /// <summary>
    /// Represents a column and its sort order for multi-column sorting
    /// </summary>
    public class SortColumn
    {
        /// <summary>
        /// Name of the column to sort by
        /// </summary>
        public string Column { get; set; } = string.Empty;
        
        /// <summary>
        /// Sort order: "asc" for ascending or "desc" for descending
        /// </summary>
        public string Order { get; set; } = "asc"; // "asc" or "desc"
    }
}