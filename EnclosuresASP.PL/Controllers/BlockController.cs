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
    public class BlockController : Controller
    {
        BlockService blockService = new BlockService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(blockService.Get());
        }

        [HttpGet]
        public ActionResult Create()
        {
            BlockVM blockVM = new BlockVM();
            PopulateBlockList(blockVM);
            return View(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlockVM blockVM)
        {
            if (ModelState.IsValid)
            {
                TypicalBlockService typicalBlockService = new TypicalBlockService();
                Block block = new Block()
                {
                    UID = blockVM.UID,
                    EnclosureID = blockVM.EnclosureID,
                    BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID)
                };

                blockService.Insert(block);
                blockService.Save();
                return RedirectToAction("Index");
            }
            PopulateBlockList(blockVM);
            return View(blockVM);
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Block block = blockService.GetByID(id);
            if (block == null)
            {
                return HttpNotFound();
            }
            BlockVM blockVM = new BlockVM()
            {
                BlockID = block.BlockID,
                UID = block.UID
            };
            if (block.BlockName == null)
            {
                blockVM.TypicalBlockID = null;
                PopulateBlockList(blockVM);
            }
            else
            {
                blockVM.TypicalBlockID = block.BlockName.TypicalBlockID;
                PopulateBlockList(blockVM, blockVM.TypicalBlockID);
            }
            ViewBag.returnUrl = returnUrl;
            return View(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlockVM blockVM, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                TypicalBlockService typicalBlockService = new TypicalBlockService();
                Block blockToUpdate = blockService.GetByID(blockVM.BlockID);

                blockToUpdate.UID = blockVM.UID;
                blockToUpdate.BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID);

                blockService.Update(blockToUpdate);
                blockService.Save();
                return Redirect(returnUrl);
            }
            PopulateBlockList(blockVM, blockVM.TypicalBlockID);
            return View(blockVM);
        }

        [HttpGet]
        public ActionResult Delete(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Block block = blockService.GetByID(id);
            if (block == null)
            {
                return HttpNotFound();
            }
            BlockVM blockVM = new BlockVM()
            {
                UID = block.UID,
                BlockGuid = block.BlockGuid,
                BlockName = block.BlockName
            };
            ViewBag.returnUrl = returnUrl;
            return View(blockVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
           // blockService.Delete(id);
            blockService.Save();
            return Redirect(returnUrl);
        }

        #region Privates
        private void PopulateBlockList(BlockVM blockVM, object selectedBlocks = null)
        {
            TypicalBlockService typicalBlock = new TypicalBlockService();
            SelectList tBlockSelectList = new SelectList(typicalBlock.Get().Select(emp => new SelectListItem { Text = emp.BlockName, Value = emp.TypicalBlockID.ToString() }), selectedBlocks);
            blockVM.TypicalBlocks = (IEnumerable<SelectListItem>)(tBlockSelectList.Items);
        }
        #endregion
    }
}