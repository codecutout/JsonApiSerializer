using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.Models.Timer
{
    public class Timer
    {
        public string Id { get; set; }

        [JsonConverter(typeof(TimeSpanInSecondsConverter))]
        public TimeSpan Duration { get; set; }

        public List<Timer> SubTimer { get; set; }
    }
}
