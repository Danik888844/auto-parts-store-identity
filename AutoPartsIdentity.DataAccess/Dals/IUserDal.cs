using AutoPartsIdentity.DataAccess.Contexts;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsIdentity.DataAccess.Dals;

public interface IUserDal
{
    Task<(List<UserListItemDto> Items, int TotalCount)> GetUserListAsync(int page = 1, int pageSize = 50, CancellationToken ct = default);
    Task<Dictionary<string, string>> GetDisplayNamesByIdsAsync(IEnumerable<string> userIds, CancellationToken ct = default);
}

public class UserDal : IUserDal
{
    private readonly IdentityDb _context;

    public UserDal(IdentityDb context)
    {
        _context = context;
    }

    public async Task<(List<UserListItemDto> Items, int TotalCount)> GetUserListAsync(
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = _context.Users.AsNoTracking().OrderBy(u => u.UserName);
        var totalCount = await query.CountAsync(ct);

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.IsActive
            })
            .ToListAsync(ct);

        if (users.Count == 0)
            return (new List<UserListItemDto>(), totalCount);

        var userIds = users.Select(u => u.Id).ToArray();

        var links = await (
            from ur in _context.UserRoles.AsNoTracking()
            join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
            where userIds.Contains(ur.UserId)
            select new { ur.UserId, RoleName = r.Name! }
        ).ToListAsync(ct);

        var rolesByUser = links
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.RoleName).Distinct().ToArray()
            );

        var result = new List<UserListItemDto>(users.Count);
        foreach (var u in users)
        {
            result.Add(new UserListItemDto
            {
                Id = u.Id.ToString(),
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                IsActive = u.IsActive,
                Roles = rolesByUser.TryGetValue(u.Id, out var roles) ? roles : Array.Empty<string>()
            });
        }

        return (result, totalCount);
    }

    public async Task<Dictionary<string, string>> GetDisplayNamesByIdsAsync(IEnumerable<string> userIds, CancellationToken ct = default)
    {
        var ids = new List<Guid>();
        foreach (var s in userIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(s)) continue;
            if (Guid.TryParse(s, out var g)) ids.Add(g);
        }
        if (ids.Count == 0) return new Dictionary<string, string>();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.UserName })
            .ToListAsync(ct);

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var u in users)
        {
            var displayName = string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName)
                ? (u.UserName ?? u.Id.ToString())
                : $"{u.FirstName} {u.LastName}".Trim();
            result[u.Id.ToString()] = displayName;
        }
        return result;
    }
}
