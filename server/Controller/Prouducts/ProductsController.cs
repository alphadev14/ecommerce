using Microsoft.AspNetCore.Mvc;
using server.BLL.Products;
using server.BO.Products;

namespace server.Controller.Prouduct
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        #region contructor
        private readonly ProductsBLL _productBLL;
        public ProductsController(ProductsBLL productBLL)
        {
            _productBLL = productBLL;
        }
        #endregion

        #region methods

        [HttpGet]
        public async Task<ProductBO> GetProductByProductId([FromQuery] int productId)
        {
            return await _productBLL.GetProductByIdAsync(productId);
        }

        [HttpGet]
        public async Task<List<ProductBO>> GetProductsByCategoryId([FromQuery] int categoryId)
        {
            return await _productBLL.GetProductsByCategoryIdAsync(categoryId);
        }

        [HttpGet]
        public async Task<List<ProductBO>> GetProductsBySearchKeyword([FromQuery] string keyword)
        {
            return await _productBLL.GetProductsBySearchKeywordAsync(keyword);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductBO product)
        {
            var result = await _productBLL.CreateProductAsync(product);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMultipleProducts([FromBody] List<ProductBO> products)
        {
            var result = await _productBLL.CreateMultipleProductsAsync(products);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductBO product)
        {
            var result = await _productBLL.UpdateProductAsync(product);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMultipleProducts([FromBody] List<ProductBO> products)
        {
            var result = await _productBLL.UpdateMultipleProductsAsync(products);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromQuery] int productId)
        {
            var result = await _productBLL.DeleteProductAsync(productId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMultipleProducts([FromBody] List<int> productIds)
        {
            var result = await _productBLL.DeleteMultipleProductsAsync(productIds);
            return Ok(result);
        }

        #endregion
    }
}
