// Controllers/RoleController.cs
using Microsoft.AspNetCore.Mvc;
using MyApp.Services;
using MyApp.DTOs.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace MyApp.Controllers.V1;

[ApiVersion(1.0)]
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public RoleController(RoleService roleService)
    {
        _roleService = roleService;
    }

    
    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(RoleDto dto)
    {
        var result = await _roleService.CreateRoleAsync(dto);

        if (result.Succeeded)
            return Ok($"Роль '{dto.RoleName}' створена");

        return BadRequest(result.Errors);
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var result = await _roleService.DeleteRoleAsync(roleName);

        if (result.Succeeded)
            return Ok($"Роль '{roleName}' видалена");

        return BadRequest(result.Errors);
    }

    [HttpGet]
    public IActionResult GetAllRoles()
    {
        return Ok(_roleService.GetAllRoles());
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(UserRoleDto dto)
    {
        var result = await _roleService.AssignRoleAsync(dto);

        if (result.Succeeded)
            return Ok($"Роль '{dto.RoleName}' призначена");

        return BadRequest(result.Errors);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole(UserRoleDto dto)
    {
        var result = await _roleService.RemoveRoleAsync(dto);

        if (result.Succeeded)
            return Ok($"Роль '{dto.RoleName}' забрана");

        return BadRequest(result.Errors);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}