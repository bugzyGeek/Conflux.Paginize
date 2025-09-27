using System.Collections.Generic;
using Xunit;
using Conflux.Paginize;

namespace Conflux.Paginize.Tests
{
    public class PagedResultTests
    {
        [Fact]
        public void PagedResult_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var pagedResult = new PagedResult<string>();

            // Assert
            Assert.Equal(0, pagedResult.PageIndex);
            Assert.Equal(0, pagedResult.TotalPages);
            Assert.Equal(0, pagedResult.PageSize);
            Assert.Equal(0, pagedResult.TotalCount);
            Assert.Null(pagedResult.Results);
        }

        [Fact]
        public void PagedResult_SetProperties_WorksCorrectly()
        {
            // Arrange
            var pagedResult = new PagedResult<string>();
            var results = new List<string> { "item1", "item2", "item3" };

            // Act
            pagedResult.PageIndex = 1;
            pagedResult.TotalPages = 5;
            pagedResult.PageSize = 10;
            pagedResult.TotalCount = 50;
            pagedResult.Results = results;

            // Assert
            Assert.Equal(1, pagedResult.PageIndex);
            Assert.Equal(5, pagedResult.TotalPages);
            Assert.Equal(10, pagedResult.PageSize);
            Assert.Equal(50, pagedResult.TotalCount);
            Assert.Equal(3, pagedResult.Results.Count);
            Assert.Equal("item1", pagedResult.Results[0]);
        }

        [Fact]
        public void PagedResult_WithComplexType_WorksCorrectly()
        {
            // Arrange
            var testItems = new List<TestItem>
            {
                new TestItem { Id = 1, Name = "Test1" },
                new TestItem { Id = 2, Name = "Test2" }
            };

            // Act
            var pagedResult = new PagedResult<TestItem>
            {
                PageIndex = 1,
                TotalPages = 1,
                PageSize = 2,
                TotalCount = 2,
                Results = testItems
            };

            // Assert
            Assert.Equal(2, pagedResult.Results.Count);
            Assert.Equal(1, pagedResult.Results[0].Id);
            Assert.Equal("Test1", pagedResult.Results[0].Name);
        }

        [Fact]
        public void PagedResult_WithEmptyResults_WorksCorrectly()
        {
            // Arrange & Act
            var pagedResult = new PagedResult<string>
            {
                PageIndex = 1,
                TotalPages = 0,
                PageSize = 10,
                TotalCount = 0,
                Results = new List<string>()
            };

            // Assert
            Assert.Equal(1, pagedResult.PageIndex);
            Assert.Equal(0, pagedResult.TotalPages);
            Assert.Equal(10, pagedResult.PageSize);
            Assert.Equal(0, pagedResult.TotalCount);
            Assert.Empty(pagedResult.Results);
        }

        private class TestItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}