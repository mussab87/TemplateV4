using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve CAPTCHA from session
            var storedCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (string.IsNullOrEmpty(storedCaptcha) || model.CaptchaInput != storedCaptcha)
            {
                ViewBag.captchError = "رمز التحقق غير صحيح!";
                ModelState.AddModelError("", "رمز التحقق غير صحيح!");
                return View(model);
            }

            // TODO: Validate email and password from the database
            //bool isUserValid = (model.Email == "test@example.com" && model.Password == "password123");

            //if (!isUserValid)
            //{
            //    ModelState.AddModelError("", "Invalid email or password.");
            //    return View(model);
            //}

            // Login successful, redirect to dashboard
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Captcha()
        {
            var captchaBytes = CaptchaService.GenerateCaptcha(out string captchaText);

            // Store CAPTCHA in session
            HttpContext.Session.SetString("CaptchaCode", captchaText);

            return File(captchaBytes, "image/png");
        }
    }
}
