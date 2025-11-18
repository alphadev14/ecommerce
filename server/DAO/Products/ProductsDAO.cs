using server.BO.Products;

namespace server.DAO.Products
{
    public class ProductsDAO
    {
        #region contructor
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public ProductsDAO(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<ProductBO> GetProductByIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductBO>> GetProductsByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductBO>> GetProductsBySearchKeywordAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        internal async Task CreateProductAsync(ProductBO productBO)
        {
            throw new NotImplementedException();
        }
    }
}
