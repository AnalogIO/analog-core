using System;
using System.Text.Json;
using CoffeeCard.Library.Utils;
using Xunit;

namespace CoffeeCard.Tests.Unit.Utils
{
    public class DateTimeConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public DateTimeConverterTests()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new DateTimeConverter());
        }

        [Fact]
        public void Serialize_ShouldFormatDateTimeCorrectly()
        {
            // Arrange
            var dateTime = new DateTime(2022, 1, 9, 21, 3, 52, millisecond: 123, DateTimeKind.Utc);
            var expectedJson = "\"2022-01-09T21:03:52.123Z\"";

            // Act
            var json = JsonSerializer.Serialize(dateTime, _options);

            // Assert
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize_ShouldParseDateTimeCorrectly()
        {
            // Arrange
            var json = "\"2022-01-09T21:03:52Z\"";
            var expectedDateTime = new DateTime(2022, 1, 9, 21, 3, 52, DateTimeKind.Utc);

            // Act
            var dateTime = JsonSerializer.Deserialize<DateTime>(json, _options);

            // Assert
            Assert.Equal(expectedDateTime, dateTime);
        }

        [Fact]
        public void Serialize_Deserialize_RoundTrip_ShouldMatchOriginalDateTime()
        {
            // Arrange
            var originalDateTime = new DateTime(2022, 1, 9, 21, 3, 52, DateTimeKind.Utc);

            // Act
            var json = JsonSerializer.Serialize(originalDateTime, _options);
            var deserializedDateTime = JsonSerializer.Deserialize<DateTime>(json, _options);

            // Assert
            Assert.Equal(originalDateTime, deserializedDateTime);
        }
    }
}
