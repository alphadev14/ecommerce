using System.ComponentModel.DataAnnotations.Schema;

namespace server.BO.Categories
{
    public class CategoriesBO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}

