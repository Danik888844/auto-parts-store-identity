using System.Net;

namespace AutoPartsIdentity.Core.Results;

public class SuccessDataResult<T>: DataResult<T>
{
    public SuccessDataResult(T data): base(data: data, result: true, statusCode: HttpStatusCode.OK){}
    
    public SuccessDataResult(T data, string message): base(data: data, result: true, message: message, statusCode: HttpStatusCode.OK){}
    
    public SuccessDataResult(string message): base(default!, result: false, message: message, HttpStatusCode.OK){}
}