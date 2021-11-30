using System;
using System.Runtime.Serialization;

namespace OneNet.PubSub.Client.Exceptions
{
    public class PubSubException : Exception
    {
        public PubSubException()
        {
        }

        public PubSubException(string message) : base(message)
        {
        }

        public PubSubException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PubSubException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}