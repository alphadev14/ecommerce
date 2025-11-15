using server.BO.Base;

namespace server.BO.User
{
    public class UserBO : BaseBO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string  Role { get; set; }
    }

    public class UserRequestBO
    {
        public string Username { get; set; }
    }

    public class UserRegisterBO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}
