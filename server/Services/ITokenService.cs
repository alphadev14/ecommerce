using System.Security.Claims;

namespace server.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, string username, string roleName, string jwtId, IEnumerable<Claim>? extraClaims = null);

        string GenerateRefreshToken();
    }
}

//Một trong những nguyên lý cơ bản trong C# OOP và clean architecture.
//Việc tạo ITokenService (interface) và TokenService(class implement) có nhiều mục đích quan trọng:

/*
    🎯 1. Tách biệt giao diện và triển khai (Abstraction)
        + ITokenService chỉ định nghĩa hợp đồng (contract): "Bất kỳ ai implement interface này phải có hàm GenerateJwtToken".
        + TokenService là một triển khai cụ thể các contract đó.
        + Nhờ vậy, code của bạn phụ thuộc vào interface thay vì phụ thuộc vào implementation -> dễ thay thế/mở rộng.

        Ví dụ: Hôm nay bạn genarate token bằng JWT, ngày mai bạn muốn đổi sang một loại token khác (ví dụ OAuth2).
            ==> Chi cần viết OAuthTokenService : ITokenService mà không cần sửa code ở Controller.

       
    🎯 2. Dễ dàng Unit Test / Mock
        + Khi viết test cho AuthController, bạn không muốn generate JWT thật.
        + Bạn có thể mock ITokenService:    
                var mockTokenService = new Mock<ITokenService>();
                mockTokenService.Setup(s => s.GenerateJwtToken("admin", null)).Returns("fake-token");

        → Nhờ có interface, test controller rất dễ, không phụ thuộc vào logic thật của TokenService.


    🎯 3. Dependency Injection (DI) hoạt động dựa trên interface
        + Trong ASP.NET Core, DI container hoạt động theo pattern: builder.Services.AddScoped<ITokenService, TokenService>();
        + Nghĩa là: 
            * Khi có class nào đó cần ITokenService, DI sẽ tự tạo inject một instance TokenService.
            * Container không cần biết TokenService tạo thế nào.
            ==> Đây là nguyên lý DIP (Denpendecy Inversion Principle) trong SOLID.


    🎯 4. Code clean & dễ bảo trì    
        + Controller chỉ biết dùng ITokenService, không quan tâm implementation.
        + Sau này bạn muốn log thêm audit, hay đổi các mã hóa token -> chỉ cần sửa trong TokenService, không ảnh hưởng tới Controller.

    ==> Tóm lại:
        + Interface: định nghĩa hợp đồng.
        + Class: triển khai cụ thể.
 */