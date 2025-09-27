using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Conflux.Paginize;

namespace Conflux.Paginize.Tests
{
    public class PaginationTests
    {
        // Test entity for pagination tests
        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public decimal Price { get; set; }
            public bool IsActive { get; set; }
        }

        private List<TestEntity> GetTestData()
        {
            return new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Apple", CreatedDate = new DateTime(2023, 1, 1), Price = 1.50m, IsActive = true },
                new TestEntity { Id = 2, Name = "Banana", CreatedDate = new DateTime(2023, 1, 2), Price = 0.75m, IsActive = true },
                new TestEntity { Id = 3, Name = "Cherry", CreatedDate = new DateTime(2023, 1, 3), Price = 2.25m, IsActive = false },
                new TestEntity { Id = 4, Name = "Date", CreatedDate = new DateTime(2023, 1, 4), Price = 3.00m, IsActive = true },
                new TestEntity { Id = 5, Name = "Elderberry", CreatedDate = new DateTime(2023, 1, 5), Price = 4.50m, IsActive = false },
                new TestEntity { Id = 6, Name = "Fig", CreatedDate = new DateTime(2023, 1, 6), Price = 2.75m, IsActive = true },
                new TestEntity { Id = 7, Name = "Grape", CreatedDate = new DateTime(2023, 1, 7), Price = 1.25m, IsActive = true },
                new TestEntity { Id = 8, Name = "Honeydew", CreatedDate = new DateTime(2023, 1, 8), Price = 3.75m, IsActive = false },
                new TestEntity { Id = 9, Name = "Kiwi", CreatedDate = new DateTime(2023, 1, 9), Price = 2.00m, IsActive = true },
                new TestEntity { Id = 10, Name = "Lemon", CreatedDate = new DateTime(2023, 1, 10), Price = 1.00m, IsActive = true }
            };
        }

        #region IQueryable Tests

        [Fact]
        public void GetPaged_IQueryable_WithValidInput_ReturnsCorrectPagedResult()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(1, result.PageIndex);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(10, result.TotalCount);
            Assert.Equal(4, result.TotalPages);
            Assert.NotNull(result.Results);
            Assert.Equal(3, result.Results.Count);
            Assert.Equal("Apple", result.Results.First().Name);
        }

        [Fact]
        public void GetPaged_IQueryable_WithMultipleSortColumns_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 5,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "IsActive", Order = "desc" },
                    new SortColumn { Column = "Price", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(5, result.Results.Count);
            // First should be active items sorted by price ascending
            Assert.True(result.Results.First().IsActive);
            // Banana (0.75m) should be first active item with lowest price
            Assert.Equal(0.75m, result.Results.First().Price);
        }

        [Fact]
        public void GetPaged_IQueryable_WithDescendingSort_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Price", Order = "desc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Elderberry", result.Results.First().Name); // Highest price
            Assert.Equal(4.50m, result.Results.First().Price);
        }

        [Fact]
        public void GetPaged_IQueryable_WithEmptyData_ReturnsEmptyResult()
        {
            // Arrange
            var data = new List<TestEntity>().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 10,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);
            Assert.NotNull(result.Results);
            Assert.Empty(result.Results);
        }

        [Fact]
        public void GetPaged_IQueryable_WithPageExceedingTotal_ReturnsLastPage()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 10, // Page 10 when only 4 pages exist
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Id", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(4, result.PageIndex); // Should be adjusted to last page
            Assert.NotNull(result.Results);
            Assert.Single(result.Results); // Last page has only 1 item
        }

        [Fact]
        public void GetPaged_IQueryable_WithInvalidPageSize_UsesDefaultPageSize()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 0, // Invalid page size
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Id", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(10, result.PageSize); // Should use default
            Assert.Equal(1, result.TotalPages);
            Assert.NotNull(result.Results);
            Assert.Equal(10, result.Results.Count);
        }

        [Fact]
        public void GetPaged_IQueryable_WithNullFilter_ReturnsUnsortedData()
        {
            // Arrange
            var data = GetTestData().AsQueryable();

            // Act
            var result = data.GetPaged(null!);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(10, result.Results.Count);
            Assert.Equal(10, result.TotalCount);
            Assert.Equal(1, result.PageIndex); // Default page
            Assert.Equal(10, result.PageSize); // Default page size
        }

        [Fact]
        public void GetPaged_IQueryable_WithNullSortColumns_ReturnsUnsortedData()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 5,
                SortColumns = null
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(5, result.Results.Count);
            Assert.Equal(10, result.TotalCount);
        }

        #endregion

        #region IEnumerable Tests

        [Fact]
        public void GetPaged_IEnumerable_WithValidInput_ReturnsCorrectPagedResult()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(1, result.PageIndex);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(10, result.TotalCount);
            Assert.Equal(4, result.TotalPages);
            Assert.NotNull(result.Results);
            Assert.Equal(3, result.Results.Count);
            Assert.Equal("Apple", result.Results.First().Name);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithMultipleSortColumns_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 5,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "IsActive", Order = "desc" },
                    new SortColumn { Column = "Price", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(5, result.Results.Count);
            // First should be active items sorted by price ascending
            Assert.True(result.Results.First().IsActive);
            // Banana (0.75m) should be first active item with lowest price
            Assert.Equal(0.75m, result.Results.First().Price);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithCaseInsensitiveColumnName_WorksCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "name", Order = "asc" } // lowercase
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Apple", result.Results.First().Name);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithInvalidColumnName_IgnoresInvalidColumn()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "InvalidColumn", Order = "asc" },
                    new SortColumn { Column = "Name", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(3, result.Results.Count);
            Assert.Equal("Apple", result.Results.First().Name); // Should still sort by Name
        }

        [Fact]
        public void GetPaged_IEnumerable_WithDateSorting_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "CreatedDate", Order = "desc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Lemon", result.Results.First().Name); // Latest date
            Assert.Equal(new DateTime(2023, 1, 10), result.Results.First().CreatedDate);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithDecimalSorting_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Price", Order = "desc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Elderberry", result.Results.First().Name); // Highest price
            Assert.Equal(4.50m, result.Results.First().Price);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithBooleanSorting_SortsCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 10,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "IsActive", Order = "desc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            // True values should come first when sorted descending
            Assert.True(result.Results.First().IsActive);
            // Count active items (should be first 7 items)
            var activeCount = result.Results.Take(7).Count(x => x.IsActive);
            Assert.Equal(7, activeCount);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GetPaged_WithNegativePageNumber_UsesPageOne()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = -1,
                PageSize = 5,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Id", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(1, result.PageIndex);
        }

        [Fact]
        public void GetPaged_WithZeroPageNumber_UsesPageOne()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 0,
                PageSize = 5,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Id", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(1, result.PageIndex);
        }

        [Fact]
        public void GetPaged_WithNegativePageSize_UsesDefaultPageSize()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 1,
                PageSize = -5,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Id", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public void GetPaged_WithEmptySortColumns_ReturnsData()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 5,
                SortColumns = new List<SortColumn>()
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(5, result.Results.Count);
            Assert.Equal(10, result.TotalCount);
        }

        [Fact]
        public void GetPaged_IEnumerable_WithNullOrder_UsesAscendingOrder()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = null! }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Apple", result.Results.First().Name); // Should sort ascending
        }

        [Fact]
        public void GetPaged_IEnumerable_WithEmptyOrder_UsesAscendingOrder()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = "" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("Apple", result.Results.First().Name); // Should sort ascending
        }

        #endregion

        #region Integration Tests with Filter Class

        [Fact]
        public void GetPaged_WithFilterObject_WorksCorrectly()
        {
            // Arrange
            var data = GetTestData().AsQueryable();
            var filter = new Filter
            {
                Page = 2,
                PageSize = 3,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Price", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.Equal(2, result.PageIndex);
            Assert.Equal(3, result.PageSize);
            Assert.NotNull(result.Results);
            Assert.Equal(3, result.Results.Count);
            // Should contain items sorted by price, starting from 4th cheapest
        }

        [Fact]
        public void GetPaged_WithComplexMultiColumnSort_WorksCorrectly()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 10,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "IsActive", Order = "desc" },
                    new SortColumn { Column = "Name", Order = "asc" },
                    new SortColumn { Column = "Price", Order = "desc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal(10, result.Results.Count);
            // First item should be active with alphabetically first name among active items
            Assert.True(result.Results.First().IsActive);
            Assert.Equal("Apple", result.Results.First().Name);
        }

        [Fact]
        public void GetPaged_WithSearchProperty_FilterObjectContainsSearch()
        {
            // Arrange
            var data = GetTestData();
            var filter = new Filter
            {
                Page = 1,
                PageSize = 10,
                Search = "test-search-term", // Testing that Search property is available
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "Name", Order = "asc" }
                }
            };

            // Act
            var result = data.GetPaged(filter);

            // Assert
            Assert.NotNull(result.Results);
            Assert.Equal("test-search-term", filter.Search); // Verify Search property is maintained
        }

        #endregion

        #region Performance and Memory Tests

        [Fact]
        public void GetPaged_WithLargeDataSet_PerformsReasonably()
        {
            // Arrange
            var largeDataSet = Enumerable.Range(1, 1000)
                .Select(i => new TestEntity
                {
                    Id = i,
                    Name = $"Item{i:D4}",
                    Price = (decimal)(i * 0.1),
                    IsActive = i % 2 == 0,
                    CreatedDate = DateTime.Now.AddDays(-i)
                })
                .ToList();

            var filter = new Filter
            {
                Page = 10,
                PageSize = 25,
                SortColumns = new List<SortColumn>
                {
                    new SortColumn { Column = "IsActive", Order = "desc" },
                    new SortColumn { Column = "Price", Order = "asc" }
                }
            };

            // Act
            var result = largeDataSet.GetPaged(filter);

            // Assert
            Assert.Equal(10, result.PageIndex);
            Assert.Equal(25, result.PageSize);
            Assert.Equal(1000, result.TotalCount);
            Assert.Equal(40, result.TotalPages);
            Assert.NotNull(result.Results);
            Assert.Equal(25, result.Results.Count);
        }

        #endregion
    }
}