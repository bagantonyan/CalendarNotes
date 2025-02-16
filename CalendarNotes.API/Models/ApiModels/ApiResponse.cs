namespace CalendarNotes.API.Models.ApiModels
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Fail(string message, int statusCode)
        {
            return new ApiResponse<T> { Succeeded = false, Message = message, StatusCode = statusCode };
        }

        public static ApiResponse<T> Success(T data, int statusCode)
        {
            return new ApiResponse<T> { Succeeded = true, Data = data, StatusCode = statusCode };
        }
    }
}