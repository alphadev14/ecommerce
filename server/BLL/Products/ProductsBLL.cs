using server.BO.Base;
using server.BO.Products;
using server.DAO.Products;
using System.Net.WebSockets;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace server.BLL.Products
{
    public class ProductsBLL
    {
        #region contructor
        private readonly ProductsDAO _productsDAO;
        public ProductsBLL(ProductsDAO productsDAO)
        {
            _productsDAO = productsDAO;
        }
        #endregion

        #region methods
        public async Task<ProductBO> GetProductByIdAsync(int productId)
        {
            var result = await _productsDAO.GetProductByIdAsync(productId);
            return result;
        }

        public async Task<List<ProductBO>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var result = await _productsDAO.GetProductsByCategoryIdAsync(categoryId);
            return result;
        }

        public async Task<List<ProductBO>> GetProductsBySearchKeywordAsync(string keyword)
        {
            var result = await _productsDAO.GetProductsBySearchKeywordAsync(keyword);
            return result;
        }

        public async Task<ProductResponseBO> CreateProductAsync(ProductBO product)
        {
            try 
            {
                await _productsDAO.CreateProductAsync(product);
                return new ProductResponseBO
                {
                    Success = true,
                    Message = "Thêm mới sản phẩm thành công"
                };
            }
            catch (Exception ex)
            {
                return new ProductResponseBO
                {
                    Success = false,
                    Message = $"Lỗi thêm mới sản phẩm: {ex.Message}"
                };
            }
        }

        public async Task<ProductResponseBO> CreateMutilProductsAsync(List<ProductBO> products)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> UpdateProductAsync(ProductBO product)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> UpdateMutilProductsAsync(List<ProductBO> products)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> DeleteProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> DeleteMutilProductsAsync(List<int> productIds)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> DeleteMultipleProductsAsync(List<int> productIds)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> CreateMultipleProductsAsync(List<ProductBO> products)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseBO> UpdateMultipleProductsAsync(List<ProductBO> products)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
