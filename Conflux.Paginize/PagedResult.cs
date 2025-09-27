using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conflux.Paginize
{
    /// <summary>
    /// Result object containing paginated data and pagination metadata
    /// </summary>
    /// <typeparam name="T">The type of data being paginated</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageIndex { get; set; }
        
        /// <summary>
        /// Total number of pages available
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// List of items for the current page
        /// </summary>
        public IList<T>? Results { get; set; }
    }
}
