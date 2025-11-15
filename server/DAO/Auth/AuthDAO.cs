
using Npgsql;
using server.BO.Auth;
using server.BO.User;

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

        public async Task<RefreshTokenBO> GetRefreshTokenAsync(string refreshToken)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = @"
                        SELECT userid, refreshtoken 
                        FROM operation.om_refresh_tokens
                        WHERE refreshtoken = @refreshtoken AND isrevoked = false
                    ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("refreshtoken", refreshToken);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new RefreshTokenBO
                {
                    UserId = reader.GetInt32(0),
                    RefreshToken = reader.GetString(1),
                };
            }

            return new RefreshTokenBO();
        }

        public async Task RevokeRefreshTokenAsync(int userId, string refreshToken, DateTime updatedDate)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            var sql = @"UPDATE operation.om_refresh_tokens
                SET updateduser = @userid,
                    isrevoked = true,
                    revokeddate = @revokeddate,
                    updateddate = @updateddate
                WHERE refreshtoken = @refreshToken";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userid", userId);
            cmd.Parameters.AddWithValue("revokeddate", updatedDate);
            cmd.Parameters.AddWithValue("updateddate", updatedDate);
            cmd.Parameters.AddWithValue("refreshtoken", refreshToken);

            await cmd.ExecuteReaderAsync();
        }
    }
}
