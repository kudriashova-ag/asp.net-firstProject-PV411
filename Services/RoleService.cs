using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;


namespace MyApp.Services;

public class RoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager,
                       UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // Створити роль
    public async Task<IdentityResult> CreateRoleAsync(RoleDto dto)
    {
        if (await _roleManager.RoleExistsAsync(dto.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{dto.RoleName}' вже існує"
            });

        return await _roleManager.CreateAsync(new IdentityRole(dto.RoleName));
    }

    // Видалити роль
    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{roleName}' не знайдена"
            });

        return await _roleManager.DeleteAsync(role);
    }

    // Отримати всі ролі
    public List<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name!).ToList();
    }

    // Призначити роль користувачу
    public async Task<IdentityResult> AssignRoleAsync(UserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{dto.RoleName}' не існує"
            });

        return await _userManager.AddToRoleAsync(user, dto.RoleName);
    }

    // Забрати роль у користувача
    public async Task<IdentityResult> RemoveRoleAsync(UserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        return await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
    }

    // Отримати ролі конкретного користувача
    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }
}