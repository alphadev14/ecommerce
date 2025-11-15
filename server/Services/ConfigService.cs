namespace server.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfiguration _config;

        public ConfigService(IConfiguration config)
        {
            _config = config;
        }

        public string GetJwtRefreshTokenExpireDays()
        {
            return _config["Jwt:RefreshTokenExpireDays"];
        }
    }
}
