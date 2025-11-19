namespace server.Common.Redis
{
    public class CartKeyBuilder
    {
        public static string GuestCart(string cartId) => $"cart:guest:{cartId}";
        public static string UserCart(int userId) => $"cart:user:{userId}";
    }
}
