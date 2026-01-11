using System.Net;

namespace AutoPartsIdentity.Core.Results;

public class ErrorDataResult<T>: DataResult<T>
{
    public ErrorDataResult(T data, HttpStatusCode statusCode): base(data: data, result: false, statusCode: statusCode){}
    
    public ErrorDataResult(T data, string message, HttpStatusCode statusCode): base(data: data, result: false, message: message, statusCode: statusCode){}
    
    public ErrorDataResult(string message, HttpStatusCode statusCode): base(default!, result: false, message: message, statusCode: statusCode){}
    
    public ErrorDataResult(string message, HttpStatusCode statusCode, List<string> errorMessages): base(default!, result: false, message: message, statusCode: statusCode, errorMessages: errorMessages){}
}