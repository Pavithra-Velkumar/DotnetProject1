using Database;
using Domain;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly MyDbContext _context;
    private readonly ILogger<UserService> _logger;
    public UserService(MyDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _logger.LogInformation($"Received request to create a new user and request data: {user}");
        // Validate user
        if (string.IsNullOrEmpty(user.UserName))
            throw new ArgumentException("UserName is required");
        else if (!IsAlphabetic(user.UserName))
            throw new ArgumentException("UserName must contain only alphabetic characters");
        else if (string.IsNullOrEmpty(user.EmailAddress) || !IsValidEmail(user.EmailAddress))
            throw new ArgumentException("A valid EmailAddress is required");
        else if (string.IsNullOrEmpty(user.AccountNumber.ToString()) || !IsValidAccountNumber(user.AccountNumber.ToString()))
            throw new ArgumentException("AccountNumber is required and must be a positive integer containing only digits & > 0");
        else if (!string.IsNullOrEmpty(user.PhoneNumber) && !IsValidPhoneNumber(user.PhoneNumber))
            throw new ArgumentException("A valid PhoneNumber is required");
        else if (user.Date != default && !IsValidDate(user.Date))
            throw new ArgumentException("A valid Date is required");

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
    public async Task<User> GetUserById(string userId)
    {
        //  try
        //     {
        var user = await _context.User.FindAsync(userId) ?? throw new KeyNotFoundException("User not found");
        return user!;
        // }
        // catch (Exception ex)
        // {
        //     throw  Exception(new {"An error occurred",  ex.Message} );
        // }
    }
    public async Task<string> DeleteUserAsync(string userId)
    {
        _logger.LogInformation("Attempting to delete user with ID: {UserId}", userId);
        var user = await _context.User.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with ID: {UserId} not found", userId);
            throw new KeyNotFoundException("User not found");
        }
        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        string message = "User deleted successfully";
        _logger.LogInformation("User with ID: {UserId} deleted successfully", userId);
        return message;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    private bool IsAlphabetic(string input)
    {
        return input.All(char.IsLetter);
    }
    private bool IsValidAccountNumber(string accountNumber)
    {
        // Example validation: account number must be a positive integer containing only digits
        return accountNumber.All(char.IsDigit) && int.Parse(accountNumber) > 0;
    }
    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Example validation: phone number must be 10 digits
        return phoneNumber.All(char.IsDigit) && phoneNumber.Length == 10;
    }
    private bool IsValidDate(DateTime date)
    {
        // Example validation: date should be a valid DateTime and not default
        return date != default(DateTime);
    }
}