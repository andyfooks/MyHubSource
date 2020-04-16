using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AJG.VirtualTrainer.MVC.Attributes;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [AuthorizeWith401RedirectAttribute(Roles = "ClaimsHandler,ProjectAdmin,SystemAdmin")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult About()
        {
            ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            ViewBag.Environment = AdminService.GetEnvironment();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            ViewBag.Environment = AdminService.GetEnvironment();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}