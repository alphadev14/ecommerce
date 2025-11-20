
using Npgsql;
using server.BO.Cart;
using server.BO.Categories;
using server.DAO.Base;

namespace server.DAO.Cart
{
    public class CartDAO : BaseDAO
    {
        public CartDAO(IConfiguration config) : base(config)
        {
        }

        public async Task<List<CartDetailBO>> GetCartByCustomerIdAsync(int? customerId)
        {
            var result = new List<CartDetailBO>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"
                    SELECT 
                        cd.cartdetailid,
                        c.cartid,
                        c.userid,
                        cd.productid,
                        cd.quantity,
                        cd.price,
                        cd.promoid,
                        cd.discountid,
                        cd.discountamount,
                        c.couponcode,
                        c.status
                    FROM operation.om_carts c
                    JOIN operation.om_cart_details cd ON c.cartid = cd.cartid
                    WHERE c.userid = @userid AND c.isdelete = FALSE AND cd.isdelete = FALSE;
                ";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userid", customerId.HasValue ? customerId.Value : DBNull.Value);

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var category = new CartDetailBO
                {
                    CartDetailId = reader.GetInt32(0),
                    CartId = reader.GetInt32(1).ToString(),
                    UserId = reader.GetInt32(2),
                    ProductId = reader.GetInt32(3),
                    Quantity = reader.GetInt64(4),
                    Price = reader.GetDecimal(5),
                    PromoId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    DiscountId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                    DiscountAmount = reader.GetDecimal(8),
                    CouponCode = reader.GetString(9),
                    Status = reader.GetString(10),
                };
                result.Add(category);
            }

            return result;
        }

        public async Task UpdateOrInsertToCartAsync(int userId, CartModel cartData)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var tran = await conn.BeginTransactionAsync();

            try
            {
                // 1. Check if user already has cart
                var getCartSql = @"
                        SELECT cartid 
                        FROM operation.om_carts 
                        WHERE userid = @userid
                        LIMIT 1;
                    ";

                int? existingCartId = null;

                await using (var checkCmd = new NpgsqlCommand(getCartSql, conn, tran))
                {
                    checkCmd.Parameters.AddWithValue("@userid", userId);
                    var result = await checkCmd.ExecuteScalarAsync();
                    existingCartId = result != null ? Convert.ToInt32(result) : null;
                }

                int cartId;

                if (existingCartId == null)
                {
                    // 2. INSERT new cart
                    var insertCartSql = @"
                        INSERT INTO operation.om_carts
                        (userid, storeid, totalprice, discountamount, finalprice, couponcode, status, createduser)
                        VALUES
                        (@userid, @storeid, @totalprice, @discountamount, @finalprice, @couponcode, 0, @createduser)
                        RETURNING cartid;
                    ";

                    await using var insertCmd = new NpgsqlCommand(insertCartSql, conn, tran);
                    insertCmd.Parameters.AddWithValue("@userid", userId);
                    insertCmd.Parameters.AddWithValue("@storeid", 116);
                    insertCmd.Parameters.AddWithValue("@totalprice", 20000);
                    insertCmd.Parameters.AddWithValue("@discountamount", 5000);
                    insertCmd.Parameters.AddWithValue("@finalprice", 15000);
                    insertCmd.Parameters.AddWithValue("@couponcode", "");
                    insertCmd.Parameters.AddWithValue("@createduser", userId);

                    cartId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                }
                else
                {
                    // 3. UPDATE existing cart
                    cartId = existingCartId.Value;

                    var updateCartSql = @"
                        UPDATE operation.om_carts
                        SET totalprice = @totalprice,
                            discountamount = @discountamount,
                            finalprice = @finalprice,
                            couponcode = @couponcode,
                            updateduser = @updateduser,
                            updateddate = NOW()
                        WHERE cartid = @cartid;
                    ";

                    await using var updateCmd = new NpgsqlCommand(updateCartSql, conn, tran);
                    updateCmd.Parameters.AddWithValue("@cartid", cartId);
                    updateCmd.Parameters.AddWithValue("@totalprice", 100000);
                    updateCmd.Parameters.AddWithValue("@discountamount", 10000);
                    updateCmd.Parameters.AddWithValue("@finalprice", 90000);
                    updateCmd.Parameters.AddWithValue("@couponcode", "");
                    updateCmd.Parameters.AddWithValue("@updateduser", userId);

                    await updateCmd.ExecuteNonQueryAsync();

                    // 4. Soft delete old details
                    var deleteDetailsSql = @"
                        UPDATE operation.om_cart_details 
                        SET isdelete = TRUE, 
                            deleteduser = @deleteduser, 
                            deleteddate = NOW()
                        WHERE cartid = @cartid;
                    ";

                    await using var delCmd = new NpgsqlCommand(deleteDetailsSql, conn, tran);
                    delCmd.Parameters.AddWithValue("@deleteduser", userId);
                    delCmd.Parameters.AddWithValue("@cartid", cartId);
                    await delCmd.ExecuteNonQueryAsync();
                }

                // 5. INSERT new details
                var insertDetailSql = @"
                    INSERT INTO operation.om_cart_details
                    (cartid, productid, quantity, price, promoid, discountid, discountamount)
                    VALUES
                    (@cartid, @productid, @quantity, @price, @promoid, @discountid, @discountamount);
                ";

                foreach (var item in cartData.Items)
                {
                    await using var detailCmd = new NpgsqlCommand(insertDetailSql, conn, tran);
                    detailCmd.Parameters.AddWithValue("@cartid", cartId);
                    detailCmd.Parameters.AddWithValue("@productid", item.ProductId);
                    detailCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                    detailCmd.Parameters.AddWithValue("@price", 10000);
                    detailCmd.Parameters.AddWithValue("@promoid", 1);
                    detailCmd.Parameters.AddWithValue("@discountid", 1);
                    detailCmd.Parameters.AddWithValue("@discountamount", 2000);

                    await detailCmd.ExecuteNonQueryAsync();
                }

                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw new Exception($"Lỗi lưu dữ liệu giỏ hàng khách hàng đã định danh: {ex}");
            }
        }

    }
}
