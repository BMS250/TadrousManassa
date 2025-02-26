namespace TadrousManassa.Models
{
    public class OperationResult<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T? Data { get; }

        private OperationResult(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static OperationResult<T> Ok(T data, string message = "Operation successful") =>
            new OperationResult<T>(true, message, data);

        public static OperationResult<T> Fail(string message) =>
            new OperationResult<T>(false, message);

        // Void-like Ok/Fail for operations without return data
        public static OperationResult<object> Ok(string message = "Operation successful") =>
            new OperationResult<object>(true, message, null);
    }
}
