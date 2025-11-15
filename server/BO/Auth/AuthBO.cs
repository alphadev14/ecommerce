using server.BO.Base;

namespace server.BO.Auth
{
    public class LoginRequestBO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseBO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class RegisterRequestBO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
    }

    public class RegisterResponseBO 
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class RefreshTokenBO : BaseBO
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public string JwtId { get; set; }
        public DateTime ExpiresDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime RevokedDate { get; set; }
        public string ReplacedByToken { get; set; }
    }

    public class LogoutRequesBO
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutResponseBO : BaseResponseBO
    {

    }
}
