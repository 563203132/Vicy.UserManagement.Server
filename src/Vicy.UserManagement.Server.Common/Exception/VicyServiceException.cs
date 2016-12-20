using System;
using System.Runtime.Serialization;

namespace Vicy.UserManagement.Server.Common
{
    public class VicyServiceException : Exception
    {
        public string ErrorCode;
        public string ErrorMessage;
        public object[] Args;

        public VicyServiceException(string message)
            : base(message)
        {
        }

        public VicyServiceException(string code, string message)
            : base(message)
        {
            ErrorCode = code;
            ErrorMessage = message;
        }

        public VicyServiceException(string code, string message, params object[] args)
            : base(message)
        {
            ErrorCode = code;
            ErrorMessage = message;
            Args = args;
        }

        public VicyServiceException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public VicyServiceException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = code;
            ErrorMessage = message;
        }

        public VicyServiceException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected VicyServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
