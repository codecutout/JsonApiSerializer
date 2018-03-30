using Newtonsoft.Json;

namespace JsonApiSerializer.Util
{
    internal class ForkableJsonReader : JsonReader
    {
        public readonly object SerializationDataToken;

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
            : this(reader, new JsonReaderState(reader.TokenType, reader.Value), reader)
        {

        }

        public ForkableJsonReader(JsonReader reader, object serializationDataToken)
          : this(reader, new JsonReaderState(reader.TokenType, reader.Value), serializationDataToken)
        {

        }

        private ForkableJsonReader(JsonReader reader, JsonReaderState state, object serializationDataToken)
        {
            this.InnerReader = reader;
            this.ParentPath = reader.Path;
            this.SetToken(state.Token, state.Value);
            this.ReaderState = state;
            this.SerializationDataToken = serializationDataToken;
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
            return new ForkableJsonReader(this.InnerReader, this.ReaderState, this.SerializationDataToken);
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
