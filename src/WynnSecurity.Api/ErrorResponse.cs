namespace WynnSecurity.Api
{
    public class ErrorResponse
    {
        public ErrorResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }
    }
}
