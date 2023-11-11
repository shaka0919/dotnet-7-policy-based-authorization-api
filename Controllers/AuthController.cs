using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Config;
using WebApi.Model;
using WebApi.Service;

namespace WebApi.Controllers;

[Route("[controller]")]
public class AuthController : Controller
{
    private readonly AppSettings _options;
    private readonly IAuthService _authService;

    public AuthController(IOptions<AppSettings> options, IAuthService authService)
    {
        _options = options.Value;
        _authService = authService;
    }

    [HttpPost]
    public ActionResult<string> Auth([FromBody] AuthRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = _authService.Authenticate(model.UserName, model.Password);
        if (string.IsNullOrEmpty(result))
        {
            return NotFound("Cannot find any matched user.");
        }

        return Ok(result);
    }
}
