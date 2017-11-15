using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnclosuresASP.PL.Controllers
{
    public class TypicalBlockController : Controller
    {
        // GET: TypicalBlock
        public ActionResult Index()
        {
            return View();
        }

        // GET: TypicalBlock/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TypicalBlock/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypicalBlock/Create
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

        // GET: TypicalBlock/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TypicalBlock/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: TypicalBlock/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TypicalBlock/Delete/5
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
