using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.login, user.password))
                {
                    FormsAuthentication.SetAuthCookie(user.login,(bool)user.remberMe);
                    return RedirectToAction("Index", "Home", new {id = user.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAvatar(int id)
        {
            using(DatabaseEntities2 context =new DatabaseEntities2())
            {
                return File(context.Users.FirstOrDefault(i => i.Id == id).avatar, "image/png");
            }
        }
    }
}