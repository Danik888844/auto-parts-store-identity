namespace AutoPartsIdentity.DataAccess.Models.DtoModels.User;

public class CreateUserFormDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginFormDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}