using server.BO.Base;

namespace server.BO.Products
{
   public class ProductBO : BaseBO
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string ProductName { get; set; }
        public string ProductShortName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public int QuantityUnitId { get; set; }
        public decimal Vat { get; set; }
        public string OptionInfor { get; set; }
        public decimal ProductLength { get; set; }
        public decimal ProductWidth { get; set; }
        public decimal ProductSize { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductResponseBO : BaseResponseBO
    {
    }
}
