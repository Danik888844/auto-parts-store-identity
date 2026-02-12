using AutoPartsIdentity.DataAccess.Models.Authentification;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;

namespace AutoPartsIdentity.Business.Services.Interfaces;

public interface ITokenCacheService
{
    public Token? RegisterUser(UserDto user);
}