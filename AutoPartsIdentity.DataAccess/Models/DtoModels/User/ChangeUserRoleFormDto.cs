namespace AutoPartsIdentity.DataAccess.Models.DtoModels.User;

public class ChangeUserRoleFormDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
