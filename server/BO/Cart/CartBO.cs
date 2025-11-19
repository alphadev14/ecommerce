using server.BO.Base;

namespace server.BO.Cart
{
    public class CartBO : BaseBO
    {
        public Guid CartId { get; set; }  
        public int? UserId { get; set; }  
        public int StoreId { get; set; } 
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? CouponCode { get; set; }
        public string Status { get; set; } = "Pending"; 

    }

    public class CartDetailBO : CartBO
    {
        public int CartDetailId { get; set; }     
        public Guid CartId { get; set; }          
        public int ProductId { get; set; }        
        public int Quantity { get; set; }         
        public decimal Price { get; set; }        
        public int? PromoId { get; set; }         
        public int? DiscountId { get; set; }     
        public decimal DiscountAmount { get; set; } 
        public decimal FinalPrice { get; set; }     
    }

    public class CartResponseBO
    {
        public string CartId { get; set; }
        public int? UserId { get; set; }
        public int StoreId { get; set; }
        public List<CartDetailBO> listProducts { get; set; }
    }

    public class AddToCartRequestBO
    {
        public string? CartId { get; set; }   // Guest only
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartModel
    {
        public string CartId { get; set; }
        public int? UserId { get; set; }  
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
