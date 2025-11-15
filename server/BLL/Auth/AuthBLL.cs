using server.BO.Auth;
using server.BO.Base;
using server.BO.User;
using server.DAO.Auth;
using server.DAO.User;
using server.Services;

namespace server.BLL.Auth
{
    public class AuthBLL
    {
        #region contructor
        private readonly AuthDAO _authDAO;
        private readonly ITokenService _tokenService;
        private readonly UserDAO _userDAO;
        private readonly IConfigService _configService;

        public AuthBLL(ITokenService tokenService, AuthDAO authDAO, UserDAO userDAO, IConfigService configService)
        {
            _authDAO = authDAO;
            _tokenService = tokenService;
            _userDAO = userDAO;
            _configService = configService;
        }
        #endregion

        #region method
        public async Task<RegisterResponseBO> RegisterAsync(RegisterRequestBO request)
        {
            // Base Valadate
            if (string.IsNullOrEmpty(request.Username))
                return new RegisterResponseBO { Message = "Họ và tên không được để trống." };

            if (string.IsNullOrEmpty(request.Password))
                return new RegisterResponseBO { Message = "Mật khẩu không được để trống." };

            if (request.Password.Trim() != request.PasswordAgain.Trim())
                return new RegisterResponseBO { Message = "Mật khẩu không trùng nhau." };

            // Check if exist
            var existRequest = new UserRequestBO
            {
                Username = request.Username
            };
            var systemUser = await _userDAO.GetUsersAsync(existRequest);

            if (!string.IsNullOrEmpty(systemUser.Username) && systemUser.Username.ToUpper().Trim() == request.Username.ToUpper().Trim())
                return new RegisterResponseBO { Message = "Người dùng đã tồn tại trong hệ thống." };

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new UserRegisterBO
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Email,
            };

            await _userDAO.InsertUserAsync(newUser);

            return new RegisterResponseBO { Success = true, Message = "Tài khoản đã được tạo thành công." };
        }

        public async Task<LoginResponseBO> LoginAsync(LoginRequestBO request)
        {
            if (string.IsNullOrEmpty(request.Username))
                return new LoginResponseBO { Message = "Tên đăng nhập không được để trống." };

            // Check exits system user
            var existRequest = new UserRequestBO
            {
                Username = request.Username
            };
            var systemUser = await _userDAO.GetUsersAsync(existRequest);
            if (systemUser == null || string.IsNullOrEmpty(systemUser.Username))
                return new LoginResponseBO { Message = "Tài khoản không tồn tại trong hệ thống, vui lòng kiểm tra lại." };

            // Compare password hash
            bool valid = BCrypt.Net.BCrypt.Verify(request.Password, systemUser.PasswordHash);
            if (!valid)
                return new LoginResponseBO { Message = "Tên đăng nhập hoặc mật khẩu không đúng, vui lòng kiểm tra lại." };


            // access token 
            var jwtId = Guid.NewGuid().ToString();
            var accessToken = _tokenService.GenerateJwtToken(systemUser.UserId, systemUser.Username, systemUser.Role, jwtId);
            // refreshToken
            var refreshToken = _tokenService.GenerateRefreshToken();

            // insert refresh token
            var refreshTokenExpireDays = int.Parse(_configService.GetJwtRefreshTokenExpireDays());
            var refreshTokenRequest = new RefreshTokenBO
            {
                UserId = systemUser.UserId,
                RefreshToken = refreshToken,
                JwtId = jwtId,
                ExpiresDate = DateTime.Now.AddDays(refreshTokenExpireDays).Date

            };
            await _authDAO.InsertRefreshTokenAsync(refreshTokenRequest);

            return new LoginResponseBO { 
                Success = true, 
                AccessToken = accessToken, 
                RefreshToken = 
                refreshToken, 
                Role = systemUser.Role, 
                Username = 
                systemUser.Username 
            };
        }

        public async Task<LogoutResponseBO> LogoutAsync(string refreshToken)
        {
            var tokenInfor = await _authDAO.GetRefreshTokenAsync(refreshToken);

            if (tokenInfor == null || string.IsNullOrEmpty(tokenInfor.RefreshToken))
                return new LogoutResponseBO { Message = "Refresh token không tồn tại." };

            var updatedDate = DateTime.Now;
            await _authDAO.RevokeRefreshTokenAsync(tokenInfor.UserId, tokenInfor.RefreshToken, updatedDate);

            return new LogoutResponseBO { Success = true, Message = "Đăng xuất thành công." };
        }
        #endregion
    }
}
