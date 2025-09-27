using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Conflux.Paginize;

namespace Conflux.Paginize.Tests
{
    public class FilterTests
    {
        [Fact]
        public void Filter_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var filter = new Filter();

            // Assert
            Assert.Null(filter.Search);
            Assert.Equal(1, filter.Page);
            Assert.Equal(10, filter.PageSize);
            Assert.Null(filter.SortColumns);
        }

        [Fact]
        public void Filter_SetProperties_WorksCorrectly()
        {
            // Arrange
            var filter = new Filter();
            var sortColumns = new List<SortColumn>
            {
                new SortColumn { Column = "Name", Order = "asc" },
                new SortColumn { Column = "Date", Order = "desc" }
            };

            // Act
            filter.Search = "test";
            filter.Page = 2;
            filter.PageSize = 25;
            filter.SortColumns = sortColumns;

            // Assert
            Assert.Equal("test", filter.Search);
            Assert.Equal(2, filter.Page);
            Assert.Equal(25, filter.PageSize);
            Assert.NotNull(filter.SortColumns);
            Assert.Equal(2, filter.SortColumns.Count);
            Assert.Equal("Name", filter.SortColumns[0].Column);
            Assert.Equal("asc", filter.SortColumns[0].Order);
        }

        [Fact]
        public void Filter_PageValidation_WithInvalidPage_FailsValidation()
        {
            // Arrange
            var filter = new Filter { Page = 0 };
            var context = new ValidationContext(filter);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(filter, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Filter.Page)));
        }

        [Fact]
        public void Filter_PageSizeValidation_WithInvalidPageSize_FailsValidation()
        {
            // Arrange
            var filter = new Filter { PageSize = 0 };
            var context = new ValidationContext(filter);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(filter, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Filter.PageSize)));
        }

        [Fact]
        public void Filter_WithValidValues_PassesValidation()
        {
            // Arrange
            var filter = new Filter { Page = 1, PageSize = 10 };
            var context = new ValidationContext(filter);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(filter, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }

    public class SortColumnTests
    {
        [Fact]
        public void SortColumn_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var sortColumn = new SortColumn();

            // Assert
            Assert.Equal(string.Empty, sortColumn.Column);
            Assert.Equal("asc", sortColumn.Order);
        }

        [Fact]
        public void SortColumn_SetProperties_WorksCorrectly()
        {
            // Arrange
            var sortColumn = new SortColumn();

            // Act
            sortColumn.Column = "TestColumn";
            sortColumn.Order = "desc";

            // Assert
            Assert.Equal("TestColumn", sortColumn.Column);
            Assert.Equal("desc", sortColumn.Order);
        }

        [Fact]
        public void SortColumn_WithAscendingOrder_IsValid()
        {
            // Arrange & Act
            var sortColumn = new SortColumn { Column = "Name", Order = "asc" };

            // Assert
            Assert.Equal("Name", sortColumn.Column);
            Assert.Equal("asc", sortColumn.Order);
        }

        [Fact]
        public void SortColumn_WithDescendingOrder_IsValid()
        {
            // Arrange & Act
            var sortColumn = new SortColumn { Column = "Date", Order = "desc" };

            // Assert
            Assert.Equal("Date", sortColumn.Column);
            Assert.Equal("desc", sortColumn.Order);
        }

        [Fact]
        public void SortColumn_WithCustomOrder_IsValid()
        {
            // Arrange & Act
            var sortColumn = new SortColumn { Column = "Custom", Order = "custom" };

            // Assert
            Assert.Equal("Custom", sortColumn.Column);
            Assert.Equal("custom", sortColumn.Order);
        }
    }
}