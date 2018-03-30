using Newtonsoft.Json;
using System;

namespace JsonApiSerializer.Test.Models.Timer
{
    public class TimeSpanInSecondsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(TimeSpan))
                throw new ArgumentException();

            if (!(reader.Value is long spanSeconds))
                return null;

            return TimeSpan.FromSeconds(spanSeconds);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var duration = (TimeSpan)value;

            writer.WriteValue((int)duration.TotalSeconds);
        }
    }
}
