using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoPartsIdentity.DataAccess.Models.Authentification;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using AutoPartsIdentity.Business.Services.Interfaces;
using AutoPartsIdentity.Core.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutoPartsIdentity.Business.Services;

public class TokenCacheService : ITokenCacheService
{
    private object _lock = new();
    private List<Token> Tokens { get; set; } = new();
    private readonly ILogger _logger;
    private readonly JwtTokenOptions _options;

    public TokenCacheService(ILogger<TokenCacheService> logger, IOptions<JwtTokenOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Token? RegisterUser(UserDto user)
    {
        try
        {
            return CreateOrRefreshToken(user);
        }
        catch(Exception ex)
        {
            _logger.LogError("{ex}",ex.Message);
            return null;
        }
    }

    private Token? CreateOrRefreshToken(UserDto user)
    {
        try
        {
            Token? userToken = Tokens.FirstOrDefault(t => t.User.Id == user.Id);

            if (userToken != null)
            {
                Tokens.Remove(userToken);
                userToken.ExpirationDate = DateTime.Now.AddDays(1);
                userToken.Value = CreateToken(user);
            }
            else
            {
                userToken = new()
                {
                    User = user, ExpirationDate = DateTime.Now.AddDays(1), Id = Guid.NewGuid().ToString(),
                    Value = CreateToken(user)
                };

                lock (_lock)
                {
                    Tokens.Add(userToken);
                }
            }

            return userToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    private string CreateToken(UserDto user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName.ToLower().Trim()),
                new Claim(ClaimTypes.Name, user.FirstName +" "+ user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            // add roles
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
            
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            var now = DateTime.Now;
            
            // create JWT-token
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.AddHours(_options.LifeTimeHours),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SigningKey)), 
                    SecurityAlgorithms.HmacSha256Signature)
            );
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return encodedJwt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return "";
        }
    }
}