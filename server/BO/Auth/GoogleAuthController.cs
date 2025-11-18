using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using System;
using System.Security.Claims;

[ApiController]
[Route("api/v1/google/[action]")]
public class GoogleAuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public GoogleAuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpGet]
    public IActionResult Login([FromQuery] string returnUrl)
    {
        var redirectUrl = Url.Action(nameof(Callback), "GoogleAuth", new { returnUrl });

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        // => redirect to Google
        return Challenge(properties, "Google");
    }

    [HttpGet]
    public async Task<IActionResult> Callback([FromQuery] string returnUrl = "/")
    {
        // Lấy thông tin đã được Google trả về thông qua CookieAuthentication
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest("Google login failed");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        if (string.IsNullOrEmpty(email))
            return BadRequest("Google did not return email");

        // Bạn tự mapping user ở đây
        var token = _tokenService.GenerateJwtToken(
            1,          // userId
            name,       // userName
            "user",     // role
            Guid.NewGuid().ToString() // jti
        );

        // Xóa cookie auth sau khi lấy info
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect($"{returnUrl}?token={token}");
    }
}
