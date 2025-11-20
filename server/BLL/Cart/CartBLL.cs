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
        public async Task<CartResponseBO> GetCartByCustomerAsync(string CartId)
        {
            int? customerId = 1;
            var result = new CartResponseBO();

            if (customerId.HasValue)
            {
                var cartDetails = await _cartDAO.GetCartByCustomerIdAsync(customerId);
               
                if (cartDetails == null || cartDetails?.Count == 0)
                {
                    return new CartResponseBO 
                    { 
                        Success = true, 
                        Message = "Giỏ hàng trống",
                    };
                }

                var cartData = cartDetails.GroupBy(x => new { x.UserId, x.CartId}).Select(g => new CartBO
                {
                    UserId = g.Key.UserId,
                    CartId = g.Key.CartId,
                    TotalPrice = g.Sum(i => i.Price * Convert.ToDecimal(i.Quantity)),
                    TotalDiscountAmount = g.Sum(i => i.DiscountAmount),
                    TotalFinalPrice = g.Sum(i => (i.Price * Convert.ToDecimal(i.Quantity)) - i.DiscountAmount),
                    CouponCode = g.FirstOrDefault().CouponCode,
                    Status = g.FirstOrDefault().Status,
                    ListCartDetail = g.Select(i => new CartDetailBO
                    {
                        CartDetailId = i.CartDetailId,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price,
                    }).ToList()
                }).FirstOrDefault();

                return new CartResponseBO
                {
                    Success = true,
                    Data = cartData
                };
            }
            else if (!string.IsNullOrEmpty(CartId))
            {
                // Lấy giỏ hàng từ Redis
                var cartRedis = _redis.Get<CartModel>(CartId) ?? new CartModel
                {
                    CartId = CartId,
                    Items = new List<CartItem>()
                };

                if (cartRedis.Items == null || cartRedis.Items.Count == 0)
                {
                    return new CartResponseBO
                    {
                        Success = true,
                        Message = "Giỏ hàng trống",
                    };
                }
                else
                {
                    var cartData = new CartBO                     {
                        CartId = CartId,
                        ListCartDetail = cartRedis.Items.Select(i => new CartDetailBO
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                        }).ToList()
                    };
                }
            }

            return new CartResponseBO
            {
                Message = "Giỏ hàng trống"
            };
        }

        public async Task<CartModel> AddToCartByCustomerAsync(AddToCartRequestBO request)
        {
            int? userId = 1;
            CartModel cart = new CartModel();

            // Khách hàng chưa có tài khoản hoặc chưa đăng nhập
            if(userId == 2)
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
            await _cartDAO.UpdateOrInsertToCartAsync(userId, cartData);
            return cartData;
        }

        public async Task<CartResponseBO> UpdateCartByCustomerAsync(AddToCartRequestBO request)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveCartItemByCustomerAsync(UpdateCartRequestBO request)
        {
            if (string.IsNullOrEmpty(request.CartId) || string.IsNullOrEmpty(request.ProductIds))
                throw new Exception("Thiếu thông tin xoá sản phẩm trong giỏ hàng");

            var cartResponse = new CartResponseBO();
            int? userId = 1; // Lấy userId từ token
            var productIds = request.ProductIds.Split(',').Select(id => int.Parse(id)).ToList();

            if (userId.HasValue)
            {
                // Xoá trong Redis
                var keyRedis = CartKeyBuilder.UserCart(userId.Value);
                var cartRedis = _redis.Get<CartModel>(keyRedis);
                if (cartRedis != null && cartRedis.Items != null)
                {
                    cartRedis.Items = cartRedis.Items.Where(i => !productIds.Contains(i.ProductId)).ToList();
                    _redis.Set(keyRedis, cartRedis, TimeSpan.FromDays(30));
                }

                // Xoá trong DB
                await _cartDAO.RemoveCartItemByUserIdAsync(userId.Value, productIds);
            }
            else
            {
                var keyRedis = CartKeyBuilder.GuestCart(request.CartId);
                var cartRedis = _redis.Get<CartModel>(keyRedis);
                if (cartRedis != null && cartRedis.Items != null)
                {
                    cartRedis.Items = cartRedis.Items.Where(i => !productIds.Contains(i.ProductId)).ToList();
                    _redis.Set(keyRedis, cartRedis, TimeSpan.FromDays(7));
                }
            }
        }

        public async Task ClearCartByCustomerAsync(UpdateCartRequestBO request)
        {
            int? userId = 1;

            if (userId.HasValue) 
            {
                // clear dữ liệu redis
                var keyRedis = CartKeyBuilder.UserCart(userId.Value);
                var cartRedis = _redis.Remove(keyRedis);

                // xóa dữ liệu database
                await _cartDAO.ClearCartByCustomerAsync(userId.Value);
            }
            else if (!string.IsNullOrEmpty(request.CartId))
            {
                // clear dữ liệu redis
                var keyRedis = CartKeyBuilder.GuestCart(request.CartId);
                var cartRedis = _redis.Remove(keyRedis);
            }
            else
            {
                throw new Exception("Khách hàng không tồn tại.");
            }
        }
        #endregion
    }
}
