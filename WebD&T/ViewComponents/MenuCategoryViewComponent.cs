using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebD_T.ViewComponents
{
    public class MenuCategoryViewComponent : ViewComponent
    {
        // query SQL 
        CategoryDAL categoryDAL = new CategoryDAL();
        public IViewComponentResult Invoke()
        { 
            List<CategoryMenu> categoryMenus = new List<CategoryMenu>();

            categoryMenus = categoryDAL.getAllWithCount(); 
            return View("Default", categoryMenus);
        }
    }
}
