using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;
using Services;

namespace MyApp.Services;


public class AuthService: IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;


    public AuthService(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        return await _userManager.CreateAsync(user, registerDto.Password);
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return null;

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenService.GenerateToken(user, roles);
    }



}