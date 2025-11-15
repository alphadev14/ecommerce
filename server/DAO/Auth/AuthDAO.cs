
using Npgsql;
using server.BO.Auth;

namespace server.DAO.Auth
{
    public class AuthDAO
    {
        #region contructor
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthDAO(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task InsertRefreshTokenAsync(RefreshTokenBO request)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"
                        INSERT INTO operation.om_refresh_tokens (
                            userid, 
                            refreshtoken, 
                            jwtid, 
                            expiresdate
                        )
                        VALUES (@userid, @refreshtoken, @jwtid, @expiresdate);
                    ";


            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userid", request.UserId);
            cmd.Parameters.AddWithValue("refreshtoken", request.RefreshToken);
            cmd.Parameters.AddWithValue("jwtid", request.JwtId);
            cmd.Parameters.AddWithValue("expiresdate", request.ExpiresDate);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
