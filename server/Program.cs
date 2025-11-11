using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using server.BLL.Categories;
using server.DAO.Categories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce API",
        Version = "v1",
        Description = "API cho hệ thống thương mại điện tử"
    });
});

// Connect PostgreSQL 
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ========== Dependency Injection ==========
builder.Services.AddScoped<CategoriesDAO>();
builder.Services.AddScoped<CategoriesBLL>();

var app = builder.Build();


// Swagger UI (cho cả production luôn, tiện test trên Render)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
