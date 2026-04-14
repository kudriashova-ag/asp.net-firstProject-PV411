using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Services;


public class AuthService: IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    public async Task<SignInResult> LoginAsync(LoginDto loginDto)
    {
        return await _signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );
    }



}