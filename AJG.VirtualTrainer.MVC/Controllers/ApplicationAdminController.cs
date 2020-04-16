using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    public class ApplicationAdminController : Controller
    {
        private VirtualTrainerContext ctx = new VirtualTrainerContext();
    
        // GET: ApplicationAdmin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Projects()
        {
            return View(ctx.Project.ToList());
        }
        // GET: ApplicationAdmin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ApplicationAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationAdmin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ApplicationAdmin/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View();
        }

        // POST: ApplicationAdmin/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ApplicationAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApplicationAdmin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
