namespace wppReservations.Areas.Api1.Models
{
    public class Api1UsersModels
    {
    }
    public class LoginModel
    {
        public LoginModel(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public RegisterModel(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
        
        public string? Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
