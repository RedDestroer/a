using System;
using System.Runtime.Serialization;

namespace FileCreator
{
    [Serializable]
    public class ArgsException
        : Exception
    {
        public ArgsException()
        {
        }

        public ArgsException(string message) 
            : base(message)
        {
        }

        public ArgsException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected ArgsException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}