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
    public class EmployeController : Controller
    {
        EmployeService employeService = new EmployeService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(employeService.Get());
        }

        [HttpGet]
        public ActionResult Create()
        {
            EmployeVM employeVM = new EmployeVM();
            PopulatePositionList(employeVM);
            return View(employeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeVM employeVM)
        {
            if (ModelState.IsValid)
            {
                PositionService positionService = new PositionService(employeService.unitOfWork);
                Employe employe = new Employe()
                {
                    FullName = employeVM.FullName,
                    EmpPosition = employeVM.PositionID == null ? null : positionService.GetByID(employeVM.PositionID)
                };
                employeService.Insert(employe);
                employeService.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            PopulatePositionList(employeVM);
            return View(employeVM);
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
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
            EmployeVM employeVM = new EmployeVM()
            {
                FullName = employe.FullName,
                EmployeID = employe.EmployeID,
            };
            if (employe.EmpPosition == null)
            {
                employeVM.PositionID = null;
                PopulatePositionList(employeVM);
            }
            else
            {
                employeVM.PositionID = employe.EmpPosition.PositionID;
                PopulatePositionList(employeVM, employe.EmpPosition.PositionID);
            }
            
            ViewBag.returnUrl = returnUrl;
            return View(employeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeVM employeVM, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                PositionService positionService = new PositionService(employeService.unitOfWork);
                Employe employeToUpdate = employeService.GetByID(employeVM.EmployeID);

                employeToUpdate.FullName = employeVM.FullName;
                employeToUpdate.EmpPosition = employeVM.PositionID == null ? null : positionService.GetByID(employeVM.PositionID);

                employeService.Update(employeToUpdate);
                employeService.unitOfWork.Save();
                return Redirect(returnUrl);
            }
            PopulatePositionList(employeVM, employeVM.PositionID);
            return View(employeVM);
        }

        [HttpGet]
        public ActionResult Delete(int? id, string returnUrl)
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
            EnclosureService enclosureService = new EnclosureService(employeService.unitOfWork);
            EmployeVM employeVM = new EmployeVM
            {
                Enclosures = enclosureService.Get().Where(x => x.Supervisor?.EmployeID == id).ToList(),
                EmployeID = employe.EmployeID,
                EmpPosition = employe.EmpPosition ?? null,
                FullName = employe.FullName
            };
            ViewBag.returnUrl = returnUrl;
            return View(employeVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            EnclosureService enclosureService = new EnclosureService(employeService.unitOfWork);
            List<Enclosure> enclosures = enclosureService.Get().Where(x => x.Supervisor?.EmployeID == id).ToList();
            for (int i=0; i<enclosures.Count; i++)
            {
                enclosures[i].Supervisor = null;
            }
            employeService.Delete(id);
            employeService.unitOfWork.Save();
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                employeService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Privates
        private void PopulatePositionList(EmployeVM empVM, object selectedPosition = null)
        {
            PositionService positionService = new PositionService(employeService.unitOfWork);
            SelectList posSelectList = new SelectList(positionService.Get().Select(pos => new SelectListItem { Text = pos.PosName, Value = pos.PositionID.ToString() }), selectedPosition);
            empVM.Positions = (IEnumerable<SelectListItem>)(posSelectList.Items);
        }
        #endregion
    }
}
