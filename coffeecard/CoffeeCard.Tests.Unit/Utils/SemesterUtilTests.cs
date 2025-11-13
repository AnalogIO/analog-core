using System;
using CoffeeCard.Library.Utils;
using Xunit;

namespace CoffeeCard.Tests.Unit.Utils
{
    public class SemesterUtilTests
    {
        [Theory(
            DisplayName = "GetSemesterStart returns the correct semester start date based on currentDate"
        )]
        [InlineData("2022/01/31", "2022/01/31")]
        [InlineData("2023/01/31", "2023/01/30")]
        [InlineData("2024/05/12", "2024/01/29")]
        [InlineData("2025/06/24", "2025/01/27")]
        [InlineData("2026/01/01", "2026/01/26")]
        [InlineData("2027/01/01", "2027/01/25")]
        [InlineData("2030/06/30", "2030/01/28")]
        [InlineData("2000/07/01", "2000/07/01")]
        [InlineData("2020/12/31", "2020/07/01")]
        public void TestGetSemesterStart(string currentDateStr, string expectedDateStr)
        {
            var currentDate = DateTime.Parse(currentDateStr);
            var expectedDate = DateTime.Parse(expectedDateStr);

            var actualDate = SemesterUtils.GetSemesterStart(currentDate);
            Assert.Equal(expectedDate, actualDate);
        }

        [Theory(
            DisplayName = "GetSemesterEnd returns the correct semester start date based on currentDate"
        )]
        [InlineData("2022/01/01", "2022/06/30")]
        [InlineData("2023/01/31", "2023/06/30")]
        [InlineData("2025/06/30", "2025/06/30")]
        [InlineData("2030/07/01", "2030/12/23")]
        [InlineData("2000/12/23", "2000/12/23")]
        [InlineData("2020/12/31", "2020/12/23")]
        public void TestGetSemesterEnd(string currentDateStr, string expectedDateStr)
        {
            var currentDate = DateTime.Parse(currentDateStr);
            var expectedDate = DateTime.Parse(expectedDateStr);

            var actualDate = SemesterUtils.GetSemesterEnd(currentDate);
            Assert.Equal(expectedDate, actualDate);
        }
    }
}
