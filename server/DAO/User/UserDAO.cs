using Npgsql;
using server.BO.Auth;
using server.BO.User;

namespace server.DAO.User
{
    public class UserDAO
    {
        #region contructor
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public UserDAO(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<UserBO> GetUsersAsync(UserRequestBO request)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"
                        SELECT userid, username, passwordhash, email, role
                        FROM masterdata.pm_users
                        WHERE (username ILIKE @usernamePattern OR email = @username)
                        AND isdelete = false;
                    ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("usernamePattern", $"%{request.Username}%");
            cmd.Parameters.AddWithValue("username", request.Username);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserBO
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Email = reader.GetString(3),
                    Role = reader.GetString(4),
                };
            }

            return new UserBO();
        }

        public async Task InsertUserAsync(UserRegisterBO request)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"
                        INSERT INTO masterdata.pm_users (username, passwordhash, email)
                        VALUES (@username, @passwordhash, @email);
                    ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", request.Username);
            cmd.Parameters.AddWithValue("passwordhash", request.PasswordHash);
            cmd.Parameters.AddWithValue("email", request.Email);

            await cmd.ExecuteNonQueryAsync();
        }

    }
}
