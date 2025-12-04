using System.Net;
using System.Text.Json.Serialization;

public class ApiResponse<T>
{
    [JsonIgnore]
    public HttpStatusCode HttpStatusCode { get; set; }
    public int StatusCode => (int)HttpStatusCode;
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 301;
    public string Message { get; set; } = null!;
    public T? Data { get; set; }


}
