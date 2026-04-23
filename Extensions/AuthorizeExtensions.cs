using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MyApp.Extensions;
public static class AuthorizeExtensions
{
    public const string JwtScheme = JwtBearerDefaults.AuthenticationScheme;

    public static AuthorizeAttribute WithJwt(this AuthorizeAttribute attr)
    {
        attr.AuthenticationSchemes = JwtScheme;
        return attr;
    }
}