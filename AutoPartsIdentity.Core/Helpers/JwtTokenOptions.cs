namespace AutoPartsIdentity.Core.Helpers;

public class JwtTokenOptions
{
    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int LifeTimeHours { get; set; } = 12;
}