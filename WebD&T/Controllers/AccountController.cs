using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebD_T.DAL;
using WebD_T.Helper;
using WebD_T.Models;

namespace WebD_T.Controllers
{
    public class AccountController : Controller
    {
        private readonly CustomerDAL _dal;

        public AccountController(CustomerDAL dal)
        {
            _dal = dal;
        }

        // ==========================================================
        // 1. REGISTER (GET)
        // ==========================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // ==========================================================
        // 2. REGISTER (POST)
        // ==========================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer model, IFormFile? ImgUpload)
        {
            // Kiểm tra email đã tồn tại chưa
            var existed = _dal.GetCustomerByEmail(model.Email);
            if (existed != null)
            {
                TempData["Error"] = "Email đã được sử dụng!";
                return View();
            }

            // Thời gian
            model.RegisterAt = DateTime.Now;
            model.UpdateAt = DateTime.Now;

            // Xử lý ảnh (tối giản)
            if (ImgUpload != null)
                model.Img = ImgUpload.FileName;
            else
                model.Img = "default-avatar.png";

            // Sinh RandomKey & Hash password (chuẩn bài hướng dẫn)
            model.RandomKey = PasswordHelper.GenerateRandomKey();
            model.Password = model.Password.ToSHA256Hash(model.RandomKey);

            // Role mặc định = User
            model.Role = 0;
            model.IsActive = true;

            bool created = _dal.SignUp(model);

            if (!created)
            {
                TempData["Error"] = "Đăng ký thất bại, vui lòng thử lại!";
                return View();
            }

            TempData["Success"] = "Đăng ký thành công. Mời bạn đăng nhập!";
            return RedirectToAction("Login");
        }

        // ==========================================================
        // 3. LOGIN (GET)
        // ==========================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ==========================================================
        // 4. LOGIN (POST)
        // ==========================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl)
        {
            var customer = _dal.GetCustomerByEmail(email);

            // Không tồn tại
            if (customer == null)
            {
                TempData["Error"] = "Sai email hoặc mật khẩu!";
                return View();
            }

            // Tài khoản bị khóa
            if (!customer.IsActive)
            {
                TempData["Error"] = "Tài khoản đang bị khóa!";
                return View();
            }

            // Hash password để so sánh
            string hashPass = password.ToSHA256Hash(customer.RandomKey);

            if (hashPass != customer.Password)
            {
                TempData["Error"] = "Sai email hoặc mật khẩu!";
                return View();
            }

            // Tạo danh sách claims cho cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.Email),
                new Claim("FullName", $"{customer.LastName} {customer.FirstName}"),
                new Claim(ClaimTypes.Role, customer.Role == 1 ? "Admin" : "Customer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // Điều hướng theo role
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Admin → chuyển vào khu vực Admin
            if (customer.Role == 1)
                return RedirectToAction("Index", "ProductAdmin", new { area = "Admin" });

            // User → về Home
            return RedirectToAction("Index", "Home");
        }

        // ==========================================================
        // 5. LOGOUT
        // ==========================================================
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ==========================================================
        // 6. ACCESS DENIED
        // ==========================================================
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập trang này!");
        }
    }
}
