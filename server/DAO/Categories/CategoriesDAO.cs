using Microsoft.EntityFrameworkCore;
using Npgsql;
using server.BO.Categories;

namespace server.DAO.Categories
{
    public class CategoriesDAO
    {
        #region contructor
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public CategoriesDAO(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        #endregion

        #region method
        public async Task<List<CategoriesBO>> GetAllCategoriesAsync()
        {
            var result = new List<CategoriesBO>();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT categoryid, categoryname, description
                FROM masterdata.pm_categories 
                WHERE isdelete = false";

            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var category = new CategoriesBO
                {
                    CategoryId = reader.GetInt32(0),
                    CategoryName = reader.GetString(1),
                    Description = reader.GetString(2),
                };
                result.Add(category);
            }

            return result;
        }

        #endregion
    }
}
