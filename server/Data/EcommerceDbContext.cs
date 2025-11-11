using Microsoft.EntityFrameworkCore;
using server.BO.Categories;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options) { }

    public DbSet<CategoriesBO> Categories { get; set; }
}
