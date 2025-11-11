using server.BO.Auth;
using server.DAO.Auth;
using server.Services;

namespace server.BLL.Auth
{
    public class AuthBLL
    {
        #region
        private readonly AuthDAO _authDAO;
        private readonly ITokenService _tokenService;

        public AuthBLL(ITokenService tokenService, AuthDAO authDAO)
        {
            _authDAO = authDAO;
            _tokenService= tokenService;
        }

        public async Task<LoginResponseBO> LoginAsync(LoginRequestBO request)
        {
            if (request.Username != "admin" || request.Password != "123") 
                return new LoginResponseBO { Message = "Tên tài khoản hoặc mật khẩu không đúng, vui lòng kiểm tra lại "};

            // Tạo access token 
            var accessToken = _tokenService.GenerateJwtToken(1, request.Username, "Admin");
            //Tạo refreshToken
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new LoginResponseBO { Success = true, AccessToken = accessToken, RefreshToken = refreshToken };
        }
        #endregion
    }
}
