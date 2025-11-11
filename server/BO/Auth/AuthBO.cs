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
    }
}
