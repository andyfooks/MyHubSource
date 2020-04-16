using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}