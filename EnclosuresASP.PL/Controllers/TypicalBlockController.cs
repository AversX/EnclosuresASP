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
    public class TypicalBlockController : Controller
    {
        TypicalBlockService typicalBlockService = new TypicalBlockService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(typicalBlockService.Get());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TypicalBlock typicalBlock)
        {
            if (ModelState.IsValid)
            {
                typicalBlockService.Insert(typicalBlock);
                typicalBlockService.Save();
                return RedirectToAction("Index");
            }
            return View(typicalBlock);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypicalBlock typicalBlock = typicalBlockService.GetByID(id);
            if (typicalBlock == null)
            {
                return HttpNotFound();
            }
            return View(typicalBlock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TypicalBlock typicalBlock)
        {
            if (ModelState.IsValid)
            {
                typicalBlockService.Update(typicalBlock);
                typicalBlockService.Save();
                return RedirectToAction("Index");
            }
            return View(typicalBlock);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypicalBlock typicalBlock = typicalBlockService.GetByID(id);
            if (typicalBlock == null)
            {
                return HttpNotFound();
            }
            BlockService blockService = new BlockService(typicalBlockService.unitOfWork);
            TypicalBlockVM typicalBlockVM = new TypicalBlockVM()
            {
                Blocks = blockService.Get().Where(x => x.BlockName?.TypicalBlockID == typicalBlock.TypicalBlockID).ToList(),
                TypicalBlockID = typicalBlock.TypicalBlockID,
                BlockName = typicalBlock.BlockName
            };
            return View(typicalBlockVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlockService blockService = new BlockService(typicalBlockService.unitOfWork);
            List<Block> blocks = blockService.Get().Where(x => x.BlockName?.TypicalBlockID == id).ToList();
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].BlockName = null;
            }
            typicalBlockService.Delete(id);
            typicalBlockService.unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                typicalBlockService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
