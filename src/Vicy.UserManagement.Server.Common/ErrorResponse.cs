namespace Vicy.UserManagement.Server.Common
{
    public class ErrorResponse
    {
        public ErrorResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorResponse(int code, string errorCode, string message)
        {
            Code = code;
            ErrorCode = errorCode;
            Message = message;
        }

        public ErrorResponse(int code, string errorCode, string message, params object[] args)
        {
            Code = code;
            ErrorCode = errorCode;
            Message = message;
            Args = args;
        }

        public int Code { get; }
        public string ErrorCode { get; set; }
        public string Message { get; }
        public object[] Args { get; }
    }
}
