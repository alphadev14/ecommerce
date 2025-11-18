using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using server.BLL.Auth;
using server.BLL.Categories;
using server.BLL.User;
using server.DAO.Auth;
using server.DAO.Categories;
using server.DAO.User;
using server.Services;
using System.Text;
using server.BLL.Products;
using server.DAO.Products;

AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressSameSiteNone", true);

var builder = WebApplication.CreateBuilder(args);

// ================== Controllers ==================
builder.Services.AddControllers();

// ================== Swagger ==================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce API",
        Version = "v1",
        Description = "API cho hệ thống thương mại điện tử"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Nhập token: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// ================== Database ==================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(connectionString));

// ================== Redis ==================
var redisConnection = builder.Configuration["Redis:ConnectionStrings"];
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "Ecommerce_";
});

// ================== CORS ==================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // port FE bạn đang dùng
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // quan trọng để cookie OAuth gửi về
    });
});

// ================== JWT + Google OAuth ==================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Dùng constant GoogleDefaults để tránh lỗi typo
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // MUST: SameSite=None to allow cross-site OAuth redirects (Google)
    //options.Cookie.SameSite = SameSiteMode.None;

    //// Tốt nhất là chạy HTTPS và đặt Always.
    //// Nếu dev trên HTTP và muốn tạm test, đổi thành SameAsRequest (không khuyến nghị).
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
})
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    options.CallbackPath = "/api/v1/google/Callback"; // trùng Google Cloud
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");

    
    // Thêm dòng này vào phần Google options
    options.UsePkce = true;
});

// ================== Dependency Injection ==================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<CategoriesDAO>();
builder.Services.AddScoped<CategoriesBLL>();
builder.Services.AddScoped<AuthBLL>();
builder.Services.AddScoped<AuthDAO>();
builder.Services.AddScoped<UserBLL>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<ProductsBLL>();
builder.Services.AddScoped<ProductsDAO>();

// ================== Build App ==================
var app = builder.Build();

// ================== Middleware ==================
// Đặt CORS trước Authentication để preflight và credentials hoạt động
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Nếu bạn muốn chạy HTTPS locally, giữ dòng này (dotnet dev-certs)
//app.UseHttpsRedirection();

// Authentication phải trước Authorization, và sau CORS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
