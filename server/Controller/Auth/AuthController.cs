using Microsoft.AspNetCore.Mvc;
using server.BLL.Auth;
using server.BO.Auth;

namespace server.Controller.Auth
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        #region contructor
        private readonly AuthBLL _authBLL;

        public AuthController(AuthBLL authBLL)
        {
            _authBLL = authBLL;
        }
        #endregion

        #region method
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestBO request)
        {
            var result = await _authBLL.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
            
        }

        [HttpPost]
        public async Task<RegisterResponseBO> Register([FromBody] RegisterRequestBO request)
        {
            var result = await _authBLL.RegisterAsync(request);
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] LogoutRequesBO request)
        {
            var result = await _authBLL.LogoutAsync(request.RefreshToken);
            return Ok(result);
        }
        #endregion
    }
}
