using WebApi.Entity;
using WebApi.Model;

namespace WebApi.Service;

public interface IUserService
{
    User? GetByName(string name);
    User? GetById(int id);
    IEnumerable<User> GetAll();
}


public class UserService : IUserService
{
    private readonly List<User> _mockData = new()
    {
        new User { Id = 1, Name = "tom", Age = 1, Password="tom", Role = Role.User },
        new User { Id = 2, Name = "jerry", Age = 2, Password="jerry", Role = Role.User },
        new User { Id = 3, Name = "spike", Age = 3, Password="spike", Role = Role.Admin }
    };

    public IEnumerable<User> GetAll()
    {
        return _mockData;
    }

    public User? GetById(int id)
    {
        return _mockData.FirstOrDefault(x => x.Id == id);
    }

    public User? GetByName(string name)
    {
        return _mockData.FirstOrDefault(x => x.Name == name);
    }
}
