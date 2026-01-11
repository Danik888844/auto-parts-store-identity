using Microsoft.Extensions.Configuration;

namespace AutoPartsIdentity.Business;

public static class AppConfig
{
    public static IConfiguration? Configuration { get; set; }
}