namespace NdfcAPIsMongoDB.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ErrorResponse
    {
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
