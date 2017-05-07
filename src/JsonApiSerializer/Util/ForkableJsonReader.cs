using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Util
{
    internal class ForkableJsonReader : JsonReader
    {
        protected readonly JsonReader InnerReader;

        protected readonly string ParentPath;

        private JsonReaderState ReaderState;

        public string FullPath {
            get
            {
                return (ParentPath + "." + Path).Trim('.');
            }
        }

        public ForkableJsonReader(JsonReader reader) 
            : this(reader, new JsonReaderState(reader.TokenType, reader.Value))
        {

        }

        private ForkableJsonReader(JsonReader reader, JsonReaderState state)
        {
            this.InnerReader = reader;
            this.ParentPath = reader.Path;
            this.SetToken(state.Token, state.Value);
            this.ReaderState = state;
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

        public ForkableJsonReader Fork()
        {
            return new ForkableJsonReader(this.InnerReader, this.ReaderState);
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
