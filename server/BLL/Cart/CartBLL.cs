using server.BO.Cart;
using server.Common.Redis;
using server.DAO.Cart;
using server.Helper;

namespace server.BLL.Cart
{
    public class CartBLL
    {
        #region contructor
        private readonly CartDAO _cartDAO;
        private readonly RedisHelper _redis;

        public CartBLL(CartDAO cartDAO)
        {
            _cartDAO = cartDAO;
            _redis = RedisHelper.Instance;
        }
        #endregion

        #region methods
        public async Task<string> GetCartByCustomerAsync(string cartId)
        {
            return await _cartDAO.GetCartAsync(cartId);
        }

        public async Task<CartModel> AddToCartByCustomerAsync(AddToCartRequestBO request)
        {
            int? userId = 1;
            CartModel cart = new CartModel();

            // Khách hàng chưa có tài khoản hoặc chưa đăng nhập
            if(userId == 1)
            {
                cart = await this.AddToCartGuestAsync(request);
            }
            // Khách hàng đã đăng nhập
            else
            {
                cart = await this.AddToCartUserAsync(userId.Value, request);
            }

            return cart;
        }

        public async Task<CartModel> AddToCartGuestAsync(AddToCartRequestBO request)
        {
            string cartId = string.IsNullOrEmpty(request.CartId) ? Guid.NewGuid().ToString() : request.CartId;

            string key = CartKeyBuilder.GuestCart(cartId);

            // Lấy giỏ hàng từ Redis
            var cartData = _redis.Get<CartModel>(key) ?? new CartModel
            {
                CartId = cartId,
                Items = new List<CartItem>()
            };

            // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa
            var item = cartData.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item != null)
            {
                // Cập nhật số lượng sản phẩm
                item.Quantity += request.Quantity;
            }
            else
            {
                // Thêm sản phẩm mới vào giỏ hàng
                cartData.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            // 4. Save lại Redis với TTL 7 ngày
            _redis.Set(key, cartData, TimeSpan.FromDays(7));

            return cartData;
        }

        public async Task<CartModel> AddToCartUserAsync(int userId, AddToCartRequestBO request)
        {
            string key = CartKeyBuilder.UserCart(userId);

            // Load cart user từ redis
            var cartData = _redis.Get<CartModel>(key) ?? new CartModel
            {
                UserId = userId,
                Items = new List<CartItem>()
            };

            // Add or update item
            var itemCart = cartData.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (itemCart == null)
            {
                cartData.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }
            else
            {
                itemCart.Quantity += request.Quantity;
            }

            // Save Redis
            _redis.Set(key, cartData, TimeSpan.FromDays(30));

            // 4. TODO: Save vào DB (optional - khi checkout hoặc sync)
            return cartData;
        }

        public async Task<CartResponseBO> UpdateCartByCustomerAsync(AddToCartRequestBO request)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
