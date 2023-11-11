using System.ComponentModel.DataAnnotations;

namespace WebApi.Model;

public class AuthRequest
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
}
