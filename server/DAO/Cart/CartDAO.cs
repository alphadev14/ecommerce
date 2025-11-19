
using Npgsql;
using server.DAO.Base;

namespace server.DAO.Cart
{
    public class CartDAO : BaseDAO
    {
        public CartDAO(IConfiguration config) : base(config)
        {
        }

        public async Task<string> GetCartAsync(string cartId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateOrInsertToCartAsync()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"
                INSERT INTO masterdata.pm_users (username, passwordhash, email)
                VALUES (@username, @passwordhash, @email);
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
