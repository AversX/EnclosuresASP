using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace EnclosuresASP.PL.Controllers
{
    public class BlockController : Controller
    {
        // GET: Block
        public ActionResult Index()
        {
            return View();
        }

        // GET: Block/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Block/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Block/Create
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

        // GET: Block/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Block/Edit/5
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

        // GET: Block/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Block/Delete/5
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
