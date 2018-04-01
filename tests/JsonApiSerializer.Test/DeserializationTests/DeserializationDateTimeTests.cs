using Newtonsoft.Json;
using System;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DateTimes
    {
        public string Id { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public DateTime DateTime { get; set; }

        public DateTimeOffset? NullableDateTimeOffset { get; set; }

        public DateTime? NullableDateTime { get; set; }
    }

    public class DeserializationDateTimeTests
    {
        [Fact]
        public void When_DateTimeOffset_should_deserialize_with_offset()
        {
            var json = @"
{
    ""data"":{
        ""id"":""one"",
        ""attributes"":{
            ""DateTimeOffset"":""2017-01-01T12:00:00+02:00"",
            ""DateTime"":""2017-01-01T12:00:00+02:00"",
            ""NullableDateTimeOffset"":""2017-01-01T12:00:00+02:00"",
            ""NullableDateTime"":""2017-01-01T12:00:00+02:00""
        }
   }
}
";
            var settings = new JsonApiSerializerSettings();
            var dateTimes = JsonConvert.DeserializeObject<DateTimes>(json,settings);

            //Not happy with the behaviour to deserialize DateTime into DateTimeKind.Local, 
            //but it is the Json.NET default
            Assert.Equal("2017-01-01T12:00:00+02:00", dateTimes.DateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T10:00:00", dateTimes.DateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.Equal("2017-01-01T12:00:00+02:00", dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T10:00:00", dateTimes.NullableDateTime?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        [Fact]
        public void When_DateTime_should_deserialize_with_local_offset()
        {
            var json = @"
{
    ""data"":{
        ""id"":""one"",
        ""attributes"":{
            ""DateTimeOffset"":""2017-01-01T12:00:00"",
            ""DateTime"":""2017-01-01T12:00:00"",
            ""NullableDateTimeOffset"":""2017-01-01T12:00:00"",
            ""NullableDateTime"":""2017-01-01T12:00:00""
        }
   }
}
";
            var settings = new JsonApiSerializerSettings();
            var localOffset = new DateTimeOffset(new DateTime(0), TimeZoneInfo.Local.BaseUtcOffset).ToString("zzz");

            var dateTimes = JsonConvert.DeserializeObject<DateTimes>(json, settings);

            //Not happy with the behaviour to use local offset if an offset isnt defined
            //but it is the Json.NET default
            Assert.Equal($"2017-01-01T12:00:00{localOffset}", dateTimes.DateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T12:00:00", dateTimes.DateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.Equal($"2017-01-01T12:00:00{localOffset}", dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T12:00:00", dateTimes.NullableDateTime?.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        [Fact]
        public void When_nullable_are_null_should_deserialize_with_null()
        {
            var json = @"
{
    ""data"":{
        ""id"":""one"",
        ""attributes"":{
            ""DateTimeOffset"":""2017-01-01T12:00:00+02:00"",
            ""DateTime"":""2017-01-01T12:00:00+02:00"",
            ""NullableDateTimeOffset"":null,
            ""NullableDateTime"":null
        }
   }
}
";
            var settings = new JsonApiSerializerSettings();
            var dateTimes = JsonConvert.DeserializeObject<DateTimes>(json, settings);

            Assert.Null(dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Null(dateTimes.NullableDateTime?.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        [Fact]
        public void When_dates_are_null_should_error()
        {
            var json = @"
{
    ""data"":{
        ""id"":""one"",
        ""attributes"":{
            ""DateTimeOffset"":null,
            ""DateTime"":null,
            ""NullableDateTimeOffset"":null,
            ""NullableDateTime"":null
        }
   }
}
";
            var settings = new JsonApiSerializerSettings();
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<DateTimes>(json, settings));
        }

        [Fact]
        public void When_dates_non_date_strings()
        {
            var json = @"
{
    ""data"":{
        ""id"":""one"",
        ""attributes"":{
            ""DateTimeOffset"":""hello world"",
            ""DateTime"":""hello world"",
            ""NullableDateTimeOffset"":""hello world"",
            ""NullableDateTime"":""hello world""
        }
   }
}
";
            var settings = new JsonApiSerializerSettings();
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<DateTimes>(json, settings));
        }


    }
}
