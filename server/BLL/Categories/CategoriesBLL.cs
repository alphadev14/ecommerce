using server.BO.Categories;
using server.DAO.Categories;

namespace server.BLL.Categories
{
    public class CategoriesBLL
    {
        #region contructor
        private readonly CategoriesDAO _categoriesDAO;

        public CategoriesBLL(CategoriesDAO categoriesDAO)
        {
            _categoriesDAO = categoriesDAO;
        }
        #endregion 
        public async Task<List<CategoriesBO>> GetAllCategoriesAsync()
        {
            var categories = await _categoriesDAO.GetAllCategoriesAsync();
            return categories;
        }
    }
}
