namespace AutoPartsIdentity.Core.DataAccess;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UserWithRoles
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public List<Role> Roles { get; set; } = new();
}

