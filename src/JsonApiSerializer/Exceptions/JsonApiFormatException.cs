using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonApiSerializer.Exceptions
{
    public class JsonApiFormatException : JsonSerializationException
    {
        public readonly string Path;

        public readonly string SpecificationInformation;

        public JsonApiFormatException(string path, string message, string specificationInformation) 
            : base(ConcatSentence(message, specificationInformation))
        {
            this.Path = path;
            this.SpecificationInformation = specificationInformation;
        }

        public JsonApiFormatException(string path, string message) 
            : this(path, message, null)
        {

        }

        private static string ConcatSentence(params string[] sentences)
        {
            return string.Join(". ", sentences
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => FirstLetterToUpper(x.Trim('.', ' '))));
        }

        private static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
