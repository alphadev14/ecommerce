using Microsoft.AspNetCore.Mvc;
using server.BLL.Cart;
using server.BO.Cart;

namespace server.Controller.Cart
{
    [ApiController]
    [Route("api/v1/[Controller]/[Action]")]
    public class CartController : ControllerBase
    {
        #region Constructor
        private readonly CartBLL _cartBLL;

        public CartController(CartBLL cartBLL)
        {
            _cartBLL = cartBLL;
        }
        #endregion

        #region methods
        [HttpGet]
        public async Task<IActionResult> GetCartByCustomer([FromQuery] string CartId)
        {
            var result = await _cartBLL.GetCartByCustomerAsync(CartId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<CartModel> AddToCartByCustomer([FromBody] AddToCartRequestBO request)
        {
            var result = await _cartBLL.AddToCartByCustomerAsync(request);
            return result;
        }

        [HttpPut]
        public async Task<CartResponseBO> UpdateCartByCustomer([FromBody] AddToCartRequestBO request)
        {
            var result = await _cartBLL.UpdateCartByCustomerAsync(request);
            return result;
        }

        [HttpDelete]
        public async Task RemoveCartItemByCustomer([FromBody] AddToCartRequestBO request)
        {
            await Task.CompletedTask;
        }

        [HttpPost]
        public async Task ClearCartByCustomer([FromBody] AddToCartRequestBO request)
        {
            await Task.CompletedTask;
        }

        [HttpPost]
        public async Task MergeCartByCustomer([FromBody] AddToCartRequestBO request)
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}
