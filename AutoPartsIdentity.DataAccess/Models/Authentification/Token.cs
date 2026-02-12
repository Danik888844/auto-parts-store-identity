using AutoPartsIdentity.DataAccess.Models.DtoModels.User;

namespace AutoPartsIdentity.DataAccess.Models.Authentification;

public class Token
{
    public string Id { get; set; } = "";
    public string Value { get; set; } = "";
    public UserDto User { get; set; } = new UserDto();
    public DateTime ExpirationDate { get; set; }
}