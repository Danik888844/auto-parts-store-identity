using System.Net;
using System.Text.Json.Serialization;

namespace AutoPartsIdentity.Core.Results;

public class DataResult<T> : ResultModel, IDataResult<T>
{
    [JsonPropertyName("data")]
    public T Data { get; }

    public DataResult(T data, bool result, HttpStatusCode statusCode) : base(result, statusCode)
    {
        Data = data;
    }
    
    public DataResult(T data, bool result, string message, HttpStatusCode statusCode) : base(result, message, statusCode)
    {
        Data = data;
    }
    
    public DataResult(T data, bool result, string message, HttpStatusCode statusCode, List<string> errorMessages) : base(result, message, statusCode, errorMessages)
    {
        Data = data;
    }
}