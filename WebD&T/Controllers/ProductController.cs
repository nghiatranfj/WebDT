using Microsoft.AspNetCore.Mvc;
using WebDT.DAL;
using WebDT.Models;

namespace WebD_T.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDAL _productDAL = new ProductDAL();

        public IActionResult Index(int? categoryId, int page = 1, string sortOrder = "")
        {
            // Lấy URL hiện tại
            var currentUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            ViewData["CurrentUrl"] = currentUrl;

            // Số lượng sản phẩm trên mỗi trang
            int pageSize = 6;

            // Lưu Id Category và sort order
            ViewData["CategoryId"] = categoryId;
            ViewData["SortColumn"] = sortOrder;

            // Lấy danh sách sản phẩm sau khi phân trang
            List<Product> products = _productDAL.GetProducts_Pagination(categoryId, page, pageSize, sortOrder);

            // Lấy tổng số lượng sản phẩm
            int rowCount = _productDAL.GetTotalProducts(categoryId);

            // Tính số lượng trang
            double pageCount = (double)rowCount / pageSize;
            int maxPage = (int)Math.Ceiling(pageCount);

            // Tạo model để hiển thị
            ProductPagination model = new ProductPagination
            {
                Products = products,
                CurrentPageIndex = page,
                PageCount = maxPage
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            Product product = _productDAL.GetProductById(id);

            if (product == null || product.Id == 0)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.Message = "Vui lòng nhập từ khóa để tìm kiếm.";
                return View(new List<Product>());
            }

            List<Product> products = _productDAL.SearchProducts(keyword);
            ViewData["SearchKeyword"] = keyword;

            return View(products);
        }

        public IActionResult Category(int id, int page = 1, string sortOrder = "")
        {
            return RedirectToAction("Index", new { categoryId = id, page, sortOrder });
        }

        public IActionResult Featured()
        {
            var featuredProducts = _productDAL.GetFeaturedProducts(8);
            return View(featuredProducts);
        }

        public IActionResult BestSellers()
        {
            var bestSellers = _productDAL.GetBestSellerProducts(10);
            return View(bestSellers);
        }
    }
}