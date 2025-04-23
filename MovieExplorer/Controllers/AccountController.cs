using Microsoft.AspNetCore.Mvc;
using MovieExplorer.Services;

namespace MovieExplorer.Controllers
{
    public class AccountController(IUserService userService) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string username, string password, string confirmPassword)
        {
            if (string.CompareOrdinal(password, confirmPassword) != 0)
            {
                ModelState.AddModelError("", "Passwords dont match!");
                return View();
            }
            try
            {
                var user = await userService.Register(username, password, email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.UserName);
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await userService.Login(email, password);
                if(null == user)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View();
                }
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.UserName);
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
