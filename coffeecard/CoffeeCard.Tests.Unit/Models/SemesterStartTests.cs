using CoffeeCard.WebApi.Models;
using System;
using Xunit;

namespace CoffeeCard.Tests.Unit.Models
{
    public class SemesterStartTests
    {
        [Theory(DisplayName = "When called in months Jan-Jun (inclusive), GetSemesterStart returns the last Monday of January")]
        [InlineData("2022/01/31", "2022/01/31")]
        [InlineData("2023/01/31", "2023/01/30")]
        [InlineData("2024/05/12", "2024/01/29")]
        [InlineData("2025/06/24", "2025/01/27")]
        [InlineData("2026/01/01", "2026/01/26")]
        [InlineData("2027/01/01", "2027/01/25")]
        [InlineData("2030/06/30", "2030/01/28")]
        public void TestSemesterStartGetsLastMondayOfJanuaryInSpring(string currentDateStr, string expectedDateStr)
        {
            var cd = currentDateStr.Split('/');
            var ed = expectedDateStr.Split('/');

            var currentDate = new DateTime(int.Parse(cd[0]), int.Parse(cd[1]), int.Parse(cd[2]));
            var expectedDate = new DateTime(int.Parse(ed[0]), int.Parse(ed[1]), int.Parse(ed[2]));

            var actualDate = Statistic.GetSemesterStart(currentDate);
            Assert.Equal(expectedDate, actualDate);
        }
    }
}
