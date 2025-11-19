namespace server.DAO.Base
{
    public class BaseDAO
    {
        #region contructor
        protected readonly IConfiguration _config;
        protected readonly string _connectionString;
        public BaseDAO(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        #endregion
    }
}
