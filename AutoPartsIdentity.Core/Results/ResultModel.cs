using System.Net;
using System.Text.Json.Serialization;

namespace AutoPartsIdentity.Core.Results;

public class ResultModel : IResult
{
    [JsonPropertyName("result")]
    public bool Result { get; }

    [JsonPropertyName("message")]
    public string Message { get; }

    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("errorMessages")]
    public List<string> ErrorMessages { get; set; }

    public ResultModel(bool result, HttpStatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
        Message = "";
        ErrorMessages = new List<string>();
    }
    
    public ResultModel(bool result, string message, HttpStatusCode statusCode) : this(result, statusCode)
    {
        Result = result;
        Message = message;
        StatusCode = statusCode;
    }
    
    public ResultModel(bool result, string message, HttpStatusCode statusCode, List<string> errorMessages)
    {
        Result = result;
        Message = message;
        StatusCode = statusCode;
        ErrorMessages = errorMessages;
    }
}