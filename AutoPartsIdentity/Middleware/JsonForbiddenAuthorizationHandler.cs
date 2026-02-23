using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace AutoPartsIdentity.Middleware;

/// <summary>
/// При 403 (Forbidden) возвращает тело ответа в формате JSON вместо пустого ответа.
/// </summary>
public class JsonForbiddenAuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly IAuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json; charset=utf-8";

            var body = new
            {
                result = false,
                message = "Access denied. Insufficient permissions.",
                statusCode = 403,
                errorMessages = Array.Empty<string>(),
                data = (object?)null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
