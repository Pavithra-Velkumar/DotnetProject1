using Domain;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserById(string userId);
    Task<string> DeleteUserAsync(string userId);
}