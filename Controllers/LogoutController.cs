using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            // ask client to remove these cookies so that
            // they won't be sent over next time
            Response.Cookies.Delete("SessionId"); //Delete cookie and send back to login as a clean slate
            Response.Cookies.Delete("Username");
            return RedirectToAction("LoginIndex", "Login");
        }
    }
}
