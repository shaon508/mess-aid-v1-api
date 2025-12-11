public class Result<T>
{

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
   
    public Dictionary<string, object> MetaData { get; set; }
    private Result()
    {
        MetaData = new Dictionary<string, object>();
    }

    public static Result<T> Success(T data, Dictionary<string, object>? metaData = null)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data,
            MetaData = metaData ?? new Dictionary<string, object>()
        };
    }

    public static Result<T> Failure(string errorMessage, Dictionary<string, object>? metaData = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            MetaData = metaData ?? new Dictionary<string, object>()
        };
    }
}
