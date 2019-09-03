using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Exceptions
{
    public class AsteriskNotConnectedException : Exception
    {
        public AsteriskNotConnectedException() { }
        public AsteriskNotConnectedException(string message) : base(message) { }
        public AsteriskNotConnectedException(string message, Exception inner) : base(message, inner) { }
        protected AsteriskNotConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
