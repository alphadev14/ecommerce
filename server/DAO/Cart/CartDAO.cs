
using Npgsql;
using server.BO.Cart;
using server.DAO.Base;

namespace server.DAO.Cart
{
    public class CartDAO : BaseDAO
    {
        public CartDAO(IConfiguration config) : base(config)
        {
        }

        public async Task<string> GetCartAsync(string cartId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateOrInsertToCartAsync(int userId, CartModel cartData)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var tran = await conn.BeginTransactionAsync();

            try
            {
                // 1. Check if user already has cart order (status = 0)
                var getOrderSql = @"
                    SELECT cartid 
                    FROM operation.om_carts 
                    WHERE userid = @userid
                    LIMIT 1;
                ";

                int? existingCartId = null;

                await using (var checkCmd = new NpgsqlCommand(getOrderSql, conn, tran))
                {
                    checkCmd.Parameters.AddWithValue("@userid", userId);
                    var result = await checkCmd.ExecuteScalarAsync();
                    existingCartId = result != null ? Convert.ToInt32(result) : null;
                }

                int cartId;

                if (existingCartId == null)
                {
                    // 2. INSERT new order
                    var insertOrderSql = @"
                        INSERT INTO operation.om_carts
                        (userid, storeid, totalprice, discountamount, finalprice, couponcode, status, createduser)
                        VALUES
                        (@userid, @storeid, @totalprice, @discountamount, @finalprice, @couponcode, 0)
                        RETURNING cartid;
                    ";

                    await using var insertCmd = new NpgsqlCommand(insertOrderSql, conn, tran);
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
                    // 3. UPDATE existing order
                    cartId = existingCartId.Value;

                    var updateOrderSql = @"
                        UPDATE operation.om_carts
                        SET totalprice = @totalprice,
                            discountamount = @discountamount,
                            finalprice = @finalprice,
                            couponcode = @couponcode
                            updateduser = @userid,
                            updateddate = NOW()
                        WHERE cartid = @cartid;
                    ";

                    await using var updateCmd = new NpgsqlCommand(updateOrderSql, conn, tran);
                    updateCmd.Parameters.AddWithValue("@cartid", cartId);
                    updateCmd.Parameters.AddWithValue("@totalprice", 100000);
                    updateCmd.Parameters.AddWithValue("@discountamount", 10000);
                    updateCmd.Parameters.AddWithValue("@finalprice", 90000);
                    updateCmd.Parameters.AddWithValue("@coupon_code", "");
                    updateCmd.Parameters.AddWithValue("@updateduser", userId);

                    await updateCmd.ExecuteNonQueryAsync();

                    // 4. Delete existing order details
                    var deleteDetailsSql = @"
                        UPDATE operation.om_cart_details 
                        SET isdelete = TRUE, 
                            deleteduser = @deleteduser, 
                            deleteddate = NOW()
                        WHERE cartid = @cartid;";

                    await using var delCmd = new NpgsqlCommand(deleteDetailsSql, conn, tran);
                    delCmd.Parameters.AddWithValue("@deleteduser", userId);
                    delCmd.Parameters.AddWithValue("@cartid", cartId);
                    await delCmd.ExecuteNonQueryAsync();
                }

                // 5. INSERT new order details
                var insertDetailSql = @"
                    INSERT INTO operation.om_cart_details
                    (cartid, productid, quantity, price, promoid, discountid, discountamount, finalprice)
                    VALUES
                    (@cartid, @productid, @quantity, @price, @promoid, @discountid, @discountamount, @finalprice);
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
                    detailCmd.Parameters.AddWithValue("@finalprice", 8000);

                    await detailCmd.ExecuteNonQueryAsync();
                }

                // Commit transaction
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
