using System.Net;

namespace AutoPartsIdentity.Core.Results;

public class ResultModel : IResult
{
    public bool Result { get; }
    public string Message { get; }
    public HttpStatusCode StatusCode { get; set; }
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