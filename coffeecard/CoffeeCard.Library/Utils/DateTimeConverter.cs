using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoffeeCard.Library.Utils
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormatIso8601 = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            return DateTime.Parse(
                reader.GetString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStringValue(
                value.ToUniversalTime().ToString(DateFormatIso8601, CultureInfo.InvariantCulture)
            );
        }
    }
}
