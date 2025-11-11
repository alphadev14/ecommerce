using Microsoft.AspNetCore.Mvc;
using server.BLL.Categories;
using server.BO.Categories;
using System.Net.WebSockets;

namespace server.Controller.Categories
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class CategoriesController : ControllerBase
    {
        #region contructor
        public readonly CategoriesBLL _categoriesBLL;   

        public CategoriesController(CategoriesBLL categoriesBLL)
        {
            _categoriesBLL = categoriesBLL;
        }
        #endregion

        #region method
        [HttpGet]
        public async Task<List<CategoriesBO>> GetAllCategories()
        {
            return await _categoriesBLL.GetAllCategoriesAsync();
        }
        #endregion
    }
}
