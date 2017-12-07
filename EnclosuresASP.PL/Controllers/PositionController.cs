using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using EnclosuresASP.PL.ActivityTrack;
using EnclosuresASP.PL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EnclosuresASP.PL.Controllers
{
    [Authorize]
    [TraceFilter]
    public class PositionController : Controller
    {
        PositionService positionService = new PositionService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(positionService.Get());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Position position)
        {
            if (ModelState.IsValid)
            {
                positionService.Insert(position);
                positionService.Save();
                return RedirectToAction("Index");
            }
            return View(position);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = positionService.GetByID(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Position position)
        {
            if (ModelState.IsValid)
            {
                positionService.Update(position);
                positionService.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(position);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = positionService.GetByID(id);
            if (position == null)
            {
                return HttpNotFound();
            }
            EmployeService employeService = new EmployeService(positionService.unitOfWork);
            PositionVM positionVM = new PositionVM()
            {
                Employes = employeService.Get().Where(x => x.EmpPosition?.PositionID == position.PositionID).ToList(),
                PositionID = position.PositionID,
                PosName = position.PosName
            };
            return View(positionVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeService employeService = new EmployeService(positionService.unitOfWork);
            List<Employe> employes = employeService.Get().Where(x => x.EmpPosition?.PositionID == id).ToList();
            for (int i = 0; i < employes.Count; i++)
            {
                employes[i].EmpPosition = null;
            }
            positionService.Delete(id);
            positionService.unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                positionService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
