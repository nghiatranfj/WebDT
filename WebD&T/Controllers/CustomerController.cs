using Microsoft.AspNetCore.Mvc;
using WebD_T.DAL;

namespace WebD_T.Controllers
{
    public class CustomerController : Controller
    {
        CustomerDAL customerDAL = new CustomerDAL();
        public IActionResult Index()
        {
            return View();
        }
    }
}
