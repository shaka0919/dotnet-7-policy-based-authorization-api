using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Config;

namespace WebApi.Service;

public interface IAuthService
{
    string Authenticate(string userName, string password);

}

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly AppSettings _options;

    public AuthService(IUserService userService, IOptions<AppSettings> options)
    {
        _userService = userService;
        _options = options.Value;
    }


    public string Authenticate(string userName, string password)
    {
        var user = _userService.GetByName(userName);

        if (user == null) return string.Empty;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
