using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
