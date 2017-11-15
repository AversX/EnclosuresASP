using System.Web.Mvc;
using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace EnclosuresASP.PL.Controllers
{
    public class EmployeController : Controller
    {
        EmployeService employeService = new EmployeService();

        // GET: Employe
        public ActionResult Index()
        {
            return View(employeService.Get());
        }

        // GET: Employe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employe/Create
        [HttpPost]
        public ActionResult Create(Employe employe)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    employeService.Insert(employe);
                    employeService.unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Невозможно сохранить данные. Попробуйте снова, и если проблема останется, обратиатесь к вашему системному администратору.");
            }
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(employe);
        }

        // GET: Enclosure/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = employeService.GetByID(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(employe);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employeToUpdate = employeService.GetByID(id);
            if (TryUpdateModel(employeToUpdate))
            {
                try
                {
                    employeService.unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Невозможно сохранить данные. Попробуйте снова, и если проблема останется, обратиатесь к вашему системному администратору.");
                }
            }
            //PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(employeToUpdate);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = employeService.GetByID(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            return View(employe);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            employeService.Delete(id);
            employeService.unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                employeService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
