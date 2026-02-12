using AutoPartsIdentity.Core.DataAccess;
using AutoPartsIdentity.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.DataAccess.Dals;

public interface IUserDal
{
    Task<List<UserWithRoles>> GetUsersWithRolesAsync(int page = 1, int pageSize = 50, CancellationToken ct = default);
}

public class UserDal : IUserDal
{
    private readonly IdentityDb _context;
    
    public UserDal(IdentityDb context)
    {
        _context = context;
    }

    public async Task<List<UserWithRoles>> GetUsersWithRolesAsync(
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        // 1) Пейдж пользователей
        var users = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new { u.Id, u.UserName, u.Email })
            .ToListAsync(ct);

        if (users.Count == 0)
            return new List<UserWithRoles>();

        var userIds = users.Select(u => u.Id).ToArray();

        // 2) Связи user->role + сами роли (join UserRoles -> Roles)
        // Если у контекста нет свойства UserRoles, см. вариант ниже через Set<IdentityUserRole<Guid>>()
        var links = await (
            from ur in _context.UserRoles.AsNoTracking()
            join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
            where userIds.Contains(ur.UserId)
            select new
            {
                ur.UserId,
                RoleId = r.Id,
                RoleName = r.Name!
            }
        ).ToListAsync(ct);

        // 3) Группировка: UserId -> List<RoleDto>
        var rolesByUser = links
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => g
                    .GroupBy(x => x.RoleId) // убираем дубли
                    .Select(x => new Role {Id = x.Key, Name = x.First().RoleName})
                    .ToList()
            );

        // 4) Сборка DTO
        var result = new List<UserWithRoles>(users.Count);
        foreach (var u in users)
        {
            result.Add(new UserWithRoles() {
                Id = u.Id,
                Roles = rolesByUser.TryGetValue(u.Id, out var roles) ? roles : new List<Role>()
            });
        }

        return result;
    }
}
