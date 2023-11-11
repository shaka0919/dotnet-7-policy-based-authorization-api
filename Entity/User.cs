using WebApi.Model;

namespace WebApi.Entity;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Password { get; set; }
    public Role Role { get; set; }
}
