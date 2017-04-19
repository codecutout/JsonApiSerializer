using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace JsonApiSerializer.Util
{
    internal class JsonWriterCapture : JsonWriter
    {
        protected readonly List<Action<JsonWriter>> actions = new List<Action<JsonWriter>>();

        public JsonWriterCapture() : base()
        {

        }

        public override void Flush()
        {
            
        }

        public void ApplyCaptured(JsonWriter writer)
        {
            foreach (var action in actions)
                action(writer);
        }

        #region ("Public JsonWriter Methods")

        public override void Close()
        {
            base.Close();
            actions.Add(x => x.Close());
        }

        public override void WriteStartObject()
        {
            base.WriteStartObject();
            actions.Add(x => x.WriteStartObject());
        }

        public override void WriteEndObject()
        {
            base.WriteEndObject();
            actions.Add(x => x.WriteEndObject());
        }

        public override void WriteStartArray()
        {
            base.WriteStartArray();
            actions.Add(x => x.WriteStartArray());
        }

        public override void WriteEndArray()
        {
            base.WriteEndArray();
            actions.Add(x => x.WriteEndArray());
        }

        public override void WriteStartConstructor(string name)
        {
            base.WriteStartConstructor(name);
            actions.Add(x => x.WriteStartConstructor(name));
        }

        public override void WriteEndConstructor()
        {
            base.WriteEndConstructor();
            actions.Add(x => x.WriteEndConstructor());
        }

        public override void WritePropertyName(string name)
        {
            base.WritePropertyName(name);
            actions.Add(x => x.WritePropertyName(name));
        }

        public override void WriteEnd()
        {
            base.WriteEnd();
            actions.Add(x => x.WriteEnd());
        }

        public override void WriteNull()
        {
            base.WriteNull();
            actions.Add(x => x.WriteNull());
        }

        public override void WriteUndefined()
        {
            base.WriteUndefined();
            actions.Add(x => x.WriteUndefined());
        }

        public override void WriteRaw(string json)
        {
            base.WriteRaw(json);
            actions.Add(x => x.WriteRaw(json));
        }

        public override void WriteRawValue(string json)
        {
            base.WriteRawValue(json);
            actions.Add(x => x.WriteRawValue(json));
        }

        public override void WriteValue(string value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(int value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(uint value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(long value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(ulong value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(float value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(double value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(bool value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(short value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(ushort value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(char value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(byte value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(sbyte value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(decimal value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(DateTime value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(DateTimeOffset value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(Guid value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(TimeSpan value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(byte[] value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteValue(Uri value)
        {
            base.WriteValue(value);
            actions.Add(x => x.WriteValue(value));
        }

        public override void WriteComment(string text)
        {
            base.WriteComment(text);
            actions.Add(x => x.WriteComment(text));
        }

        public override void WriteWhitespace(string ws)
        {
            base.WriteWhitespace(ws);
            actions.Add(x => x.WriteWhitespace(ws));
        }

#endregion
    }
}
