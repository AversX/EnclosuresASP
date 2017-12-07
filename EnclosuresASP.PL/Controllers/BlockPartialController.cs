using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using EnclosuresASP.PL.ActivityTrack;
using EnclosuresASP.PL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EnclosuresASP.PL.Controllers
{
    [Authorize]
    [TraceFilter]
    public class BlockPartialController : Controller
    {
        [HttpGet]
        public ActionResult Index(string blocks)
        {
            List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blocks);
            return PartialView(Blocks);
        }

        [HttpGet]
        public ActionResult Create(string blocks)
        {
            BlockVM blockVM = new BlockVM() { Blocks = blocks };
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlockVM blockVM)
        {
            if (ModelState.IsValid)
            {
                List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blockVM.Blocks);
                TypicalBlockService typicalBlockService = new TypicalBlockService();
                Block block = new Block()
                {
                    UID = blockVM.UID,
                    EnclosureID = blockVM.EnclosureID,
                    BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID),
                };
                Blocks.Add(block);
                return Json(new { success = true, data = JsonConvert.SerializeObject(Blocks) }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpGet]
        public ActionResult Edit(string blocks, string blockGuid, string returnUrl)
        {
            List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blocks);
            Guid BlockGuid = JsonConvert.DeserializeObject<Guid>(blockGuid);

            Block block = Blocks.Find(x => x.BlockGuid == BlockGuid);

            BlockVM blockVM = new BlockVM()
            {
                UID = block.UID,
                BlockGuid = block.BlockGuid,
                Blocks = blocks
            };
            if (block.BlockName == null)
                blockVM.TypicalBlockID = null;
            else
                blockVM.TypicalBlockID = block.BlockName.TypicalBlockID;

            PopulateBlockList(blockVM, blockVM.TypicalBlockID);
            return PartialView(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BlockVM blockVM, string BlockGuid)
        {
            if (ModelState.IsValid)
            {
                TypicalBlockService typicalBlockService = new TypicalBlockService();
                blockVM.BlockGuid = Guid.Parse(BlockGuid);
                List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blockVM.Blocks);
                int index = Blocks.FindIndex(x => x.BlockGuid == blockVM.BlockGuid);
                if (index >= 0)
                {
                    Blocks[index].UID = blockVM.UID;
                    Blocks[index].BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID);
                }
                return Json(new { success = true, data = JsonConvert.SerializeObject(Blocks) }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpGet]
        public ActionResult Delete(string blocks, string blockGuid)
        {
            List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blocks);
            Guid BlockGuid = JsonConvert.DeserializeObject<Guid>(blockGuid);

            Block block = Blocks.Find(x => x.BlockGuid == BlockGuid);

            BlockVM blockVM = new BlockVM()
            {
                UID = block.UID,
                BlockGuid = block.BlockGuid,
                Blocks = blocks,
                BlockName = block.BlockName
            };
            return PartialView(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(BlockVM blockVM, string BlockGuid)
        {
            blockVM.BlockGuid = Guid.Parse(BlockGuid);
            List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blockVM.Blocks);
            int index = Blocks.FindIndex(x => x.BlockGuid == blockVM.BlockGuid);
            if (index >= 0)
            {
                Blocks.RemoveAt(index);
            }
            return Json(new { success = true, data = JsonConvert.SerializeObject(Blocks) }, JsonRequestBehavior.AllowGet);
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