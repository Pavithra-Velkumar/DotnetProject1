namespace Domain;

public class User
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int AccountNumber { get; set; }
    public string EmailAddress { get; set; }
    public int? PhoneNumber { get; set; }
    public DateTime Date { get; set; }
}