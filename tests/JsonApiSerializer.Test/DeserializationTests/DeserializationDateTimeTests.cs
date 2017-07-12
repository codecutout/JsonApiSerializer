using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
     
            Assert.Equal("2017-01-01T12:00:00+02:00", dateTimes.DateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T10:00:00", dateTimes.DateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.Equal("2017-01-01T12:00:00+02:00", dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T10:00:00", dateTimes.NullableDateTime?.ToString("yyyy-MM-ddTHH:mm:ss"));
        }

        [Fact]
        public void When_DateTime_should_deserialize_with_zero_offset()
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
            var dateTimes = JsonConvert.DeserializeObject<DateTimes>(json, settings);

            Assert.Equal("2017-01-01T12:00:00+00:00", dateTimes.DateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal("2017-01-01T12:00:00", dateTimes.DateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.Equal("2017-01-01T12:00:00+00:00", dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
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

            Assert.Equal(null, dateTimes.NullableDateTimeOffset?.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            Assert.Equal(null, dateTimes.NullableDateTime?.ToString("yyyy-MM-ddTHH:mm:ss"));
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
