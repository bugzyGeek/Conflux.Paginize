# Conflux.Paginize

A comprehensive pagination library for .NET applications that provides advanced multi-column sorting capabilities for both `IQueryable<T>` and `IEnumerable<T>` data sources.

## Features

✅ **Multi-Column Sorting**: Sort by multiple columns with customizable order (ascending/descending)  
✅ **Filter-Based API**: Clean, object-oriented API using Filter objects for better parameter management  
✅ **Dual Data Source Support**: Works with both `IQueryable<T>` (Entity Framework, databases) and `IEnumerable<T>` (in-memory collections)  
✅ **Dynamic Column Sorting**: Specify column names as strings with reflection-based sorting  
✅ **Comprehensive Filtering**: Built-in `Filter` class with validation attributes  
✅ **Edge Case Handling**: Robust handling of invalid page numbers, page sizes, and column names  
✅ **Performance Optimized**: Efficient property caching and optimized query execution  
✅ **Type Safety**: Generic implementation with compile-time type checking  

## Installation

### From Azure DevOps Artifacts
dotnet add package Conflux.Paginize --source "https://pkgs.dev.azure.com/[YourOrganization]/_packaging/[YourFeedName]/nuget/v3/index.json"
### Package Manager Console
Install-Package Conflux.Paginize -Source "https://pkgs.dev.azure.com/[YourOrganization]/_packaging/[YourFeedName]/nuget/v3/index.json"
## Quick Start

### Basic Pagination with Filter Object
using Conflux.Paginize;

// With IQueryable (Entity Framework)
var filter = new Filter
{
    Page = 1,
    PageSize = 10,
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "LastName", Order = "asc" },
        new SortColumn { Column = "FirstName", Order = "asc" }
    }
};

var pagedUsers = dbContext.Users.GetPaged(filter);

// With IEnumerable (in-memory)
var filter2 = new Filter
{
    Page = 1,
    PageSize = 10,
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "Name", Order = "desc" }
    }
};

var pagedItems = myList.GetPaged(filter2);
### Advanced Filter Usage
var filter = new Filter
{
    Page = 2,
    PageSize = 25,
    Search = "john", // Available for custom search implementations
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "CreatedDate", Order = "desc" },
        new SortColumn { Column = "Priority", Order = "asc" },
        new SortColumn { Column = "Name", Order = "asc" }
    }
};

var result = data.GetPaged(filter);
## API Reference

### Extension Methods

#### `GetPaged<T>(Filter filter)`
Paginates an `IQueryable<T>` or `IEnumerable<T>` using a Filter object for better parameter management.

**Parameters:**
- `filter`: Filter object containing pagination and sorting parameters

**Returns:** `PagedResult<T>` with paginated results

### Classes

#### `PagedResult<T>`public class PagedResult<T>
{
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IList<T>? Results { get; set; }
}
#### `Filter`public class Filter
{
    public string? Search { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; } = 10;
    public List<SortColumn>? SortColumns { get; set; }
}
#### `SortColumn`public class SortColumn
{
    public string Column { get; set; } = string.Empty;
    public string Order { get; set; } = "asc"; // "asc" or "desc"
}
## Advanced Examples

### Complex Multi-Column Sorting
var filter = new Filter
{
    Page = 1,
    PageSize = 20,
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "IsActive", Order = "desc" },    // Active items first
        new SortColumn { Column = "Priority", Order = "asc" },     // Then by priority
        new SortColumn { Column = "CreatedDate", Order = "desc" }, // Then by newest
        new SortColumn { Column = "Name", Order = "asc" }          // Finally by name
    }
};

var result = data.GetPaged(filter);
### Filter Validation
var filter = new Filter
{
    Page = 0,     // Will be adjusted to 1
    PageSize = -5, // Will be adjusted to 10 (default)
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "Name", Order = "asc" }
    }
};

// Automatic parameter correction
var result = data.GetPaged(filter);
// result.PageIndex will be 1, result.PageSize will be 10
### Null Handling
// Passing null filter creates default filter
var result1 = data.GetPaged(null); // Uses default: Page=1, PageSize=10, no sorting

// Empty filter object
var emptyFilter = new Filter(); // Uses defaults
var result2 = data.GetPaged(emptyFilter);
### Case-Insensitive Column Names
var filter = new Filter
{
    Page = 1,
    PageSize = 10,
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "lastname", Order = "asc" },  // lowercase
        new SortColumn { Column = "FIRSTNAME", Order = "asc" }  // uppercase
    }
};
// Works with different casing
## Supported Data Types

- `string`
- `int`, `long`, `decimal`, `double`, `float`
- `DateTime`, `DateTimeOffset`
- `bool`
- `Guid`
- `enum` types
- Nullable versions of all above types

## Requirements

- .NET 8.0 or later
- System.Linq.Dynamic.Core 1.6.7+

## Performance Considerations

- **IQueryable**: Sorting and pagination are applied at the database level for optimal performance
- **IEnumerable**: Uses reflection-based sorting with property caching for improved performance
- **Large Datasets**: Tested with 1000+ items with excellent performance characteristics
- **Filter Objects**: Minimal overhead, reusable across multiple queries

## Error Handling
// Invalid column names are gracefully ignored
var filter = new Filter
{
    SortColumns = new List<SortColumn>
    {
        new SortColumn { Column = "InvalidColumn", Order = "asc" }, // Ignored
        new SortColumn { Column = "Name", Order = "asc" }           // Applied
    }
};

// Invalid page numbers and sizes are automatically corrected
var filter2 = new Filter { Page = -1, PageSize = 0 }; // Becomes Page=1, PageSize=10
## License

MIT License

## Changelog

### v1.0.0
- Initial release with Filter-based API
- Multi-column sorting support for IQueryable<T> and IEnumerable<T>
- GetPaged extension methods for both data source types
- Filter class with built-in validation attributes
- SortColumn class for defining sorting criteria
- PagedResult<T> class for pagination results
- Case-insensitive column name support
- Automatic handling of invalid parameters (page numbers, page sizes)
- Comprehensive test suite
- Performance optimizations with property caching
- Support for all common data types (string, numeric, DateTime, bool, etc.)
- Documentation and usage examples