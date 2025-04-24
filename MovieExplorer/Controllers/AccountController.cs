using Microsoft.AspNetCore.Mvc;
using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services.Interfaces;
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
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            try
            {
                var user = await userService.RegisterAsync(registerViewModel);
                UpdateSessionWithUserInfo(user);
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        private void UpdateSessionWithUserInfo(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            try
            {
                var user = await userService.LoginAsync(loginViewModel.Email, loginViewModel.Password);
                if (null != user)
                {
                    UpdateSessionWithUserInfo(user);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", "Email or password are incorect!");
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
