
namespace server.DAO.Auth
{
    public class AuthDAO
    {
        #region

        #endregion
        public async Task<string> LoginAsync(string username, string password)
        {
            return $"Hello {username}, bạn đã đăng nhập thành công";
        }
    }
}
