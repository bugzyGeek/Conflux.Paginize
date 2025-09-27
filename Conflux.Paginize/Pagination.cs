using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core; // Added for dynamic sorting
using System.Reflection; // For reflection-based sorting

namespace Conflux.Paginize
{
    /// <summary>
    /// Extension methods for implementing pagination with multi-column sorting support
    /// </summary>
    public static class Pagination
    {
        /// <summary>
        /// Paginates an IQueryable with multi-column sorting support using a Filter object
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="query">The IQueryable to paginate</param>
        /// <param name="filter">Filter object containing pagination and sorting parameters</param>
        /// <returns>PagedResult containing the paginated data</returns>
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, Filter filter) where T : class
        {
            if (filter == null)
                filter = new Filter();

            // Apply sorting if sortColumns are provided
            if (filter.SortColumns != null && filter.SortColumns.Count > 0)
            {
                var ordering = string.Join(", ", filter.SortColumns.Select(sc => $"{sc.Column} {sc.Order}"));
                query = query.OrderBy(ordering);
            }

            // Validate and adjust input parameters
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize; // Default page size
            var pageNumber = filter.Page <= 0 ? 1 : filter.Page; // Default to first page

            var result = new PagedResult<T>();
            result.TotalCount = query.Count();
            result.PageSize = pageSize;
            result.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            
            if (result.TotalCount == 0)
            {
                result.PageIndex = pageNumber;
                result.Results = new List<T>();
                return result;
            }
            
            if (pageNumber > result.TotalPages) pageNumber = result.TotalPages; // Adjust if page exceeds total pages
            result.PageIndex = pageNumber;
            result.Results = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// Paginates an IEnumerable with multi-column sorting support using a Filter object
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="source">The IEnumerable to paginate</param>
        /// <param name="filter">Filter object containing pagination and sorting parameters</param>
        /// <returns>PagedResult containing the paginated data</returns>
        public static PagedResult<T> GetPaged<T>(this IEnumerable<T> source, Filter filter) where T : class
        {
            if (filter == null)
                filter = new Filter();

            IEnumerable<T> sorted = source;
            if (filter.SortColumns != null && filter.SortColumns.Count > 0)
            {
                // Cache PropertyInfo lookups for efficiency
                var propertyCache = filter.SortColumns
                    .Select(sc => new { sc, Prop = typeof(T).GetProperty(sc.Column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) })
                    .Where(x => x.Prop != null)
                    .ToList();

                IOrderedEnumerable<T>? ordered = null;
                for (int i = 0; i < propertyCache.Count; i++)
                {
                    var sc = propertyCache[i].sc;
                    var prop = propertyCache[i].Prop!; // We know it's not null due to Where clause
                    var order = sc.Order?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true ? "desc" : "asc";
                    if (i == 0)
                    {
                        ordered = order == "desc"
                            ? sorted.OrderByDescending(x => prop.GetValue(x, null))
                            : sorted.OrderBy(x => prop.GetValue(x, null));
                    }
                    else if (ordered != null)
                    {
                        ordered = order == "desc"
                            ? ordered.ThenByDescending(x => prop.GetValue(x, null))
                            : ordered.ThenBy(x => prop.GetValue(x, null));
                    }
                }
                if (ordered != null)
                    sorted = ordered;
            }

            // Validate and adjust input parameters
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize; // Default page size
            var pageNumber = filter.Page <= 0 ? 1 : filter.Page; // Default to first page

            var result = new PagedResult<T>();
            result.TotalCount = sorted.Count();
            result.PageSize = pageSize;
            result.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            
            if (result.TotalCount == 0)
            {
                result.PageIndex = pageNumber;
                result.Results = new List<T>();
                return result;
            }
            
            if (pageNumber > result.TotalPages) pageNumber = result.TotalPages; // Adjust if page exceeds total pages
            result.PageIndex = pageNumber;
            result.Results = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}