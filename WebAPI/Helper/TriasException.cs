namespace TRIAS.NET.WebAPI.Helper
{
    public class TriasException : Exception
    {
        public int StatusCode { get; init; }
        public TriasException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
