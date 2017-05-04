using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Util
{
    internal class ForkableJsonReader : JsonReader
    {
        protected readonly JsonReader InnerReader;

        private JsonReaderState ReaderState;

        public ForkableJsonReader(JsonReader reader) 
            : this(reader, new JsonReaderState(reader.TokenType, reader.Value))
        {

        }

        private ForkableJsonReader(JsonReader reader, JsonReaderState state)
        {
            this.InnerReader = reader;
            this.SetToken(state.Token, state.Value);
            ReaderState = state;
        }



        public override bool Read()
        {
            if(ReaderState.Next != null)
            {
                ReaderState = ReaderState.Next;
                this.SetToken(ReaderState.Token, ReaderState.Value);
                return true;
            }
            else
            {
                var result = InnerReader.Read();
                this.SetToken(InnerReader.TokenType, InnerReader.Value);
                ReaderState.Next = new JsonReaderState(this.TokenType, this.Value);
                ReaderState = ReaderState.Next;
                return result;
            }
        }

        private JsonReader Fork()
        {
            return new ForkableJsonReader(this.InnerReader, this.ReaderState);
           
        }

        public static JsonReader LookAhead(ref JsonReader reader)
        {
            var peekJsonReader = (reader as ForkableJsonReader) ?? new ForkableJsonReader(reader);
            reader = peekJsonReader;
            return peekJsonReader.Fork();
        }

     
        private class JsonReaderState
        {
            public readonly JsonToken Token;
            public readonly object Value;

            public JsonReaderState Next;

            public JsonReaderState(JsonToken token, object value)
            {
                this.Token = token;
                this.Value = value;
            }
        }
    }
}
