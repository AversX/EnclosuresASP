using EnclosuresASP.BLL;
using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using EnclosuresASP.PL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnclosuresASP.PL.ActivityTrack;

namespace EnclosuresASP.PL.Controllers
{
    [Authorize]
    [TraceFilter]
    public class EnclosureController : Controller
    {
        EnclosureService enclosureService = new EnclosureService();

        // GET: Enclosure
        [HttpGet]
        public ActionResult Index()
        {
            return View(enclosureService.Get(x => x.Temporary==false));
        }

        // GET: Enclosure/Details/?
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(enclosureService.GetByID(id));
        }

        // GET: Enclosure/Create
        [HttpGet]
        public ActionResult Create()
        {
            Enclosure enclosure = new Enclosure()
            {
                Temporary = true,
                Number = "temp",
                Username = HttpContext.User.Identity.Name,
                Blocks = new List<Block>(),
                ACSs = new List<ACS>()
            };
            enclosureService.Insert(enclosure);
            enclosureService.Save();
            EnclosureVM enclosureVM = new EnclosureVM()
            {
                EnclosureID = enclosure.EnclosureID,
                Username = enclosure.Username,
                BlocksJSON = JsonConvert.SerializeObject(enclosure.Blocks),
                ACSsJSON = JsonConvert.SerializeObject(enclosure.ACSs)
            };
            PopulateEmployeList(enclosureVM);
            return View(enclosureVM);
        }

        // POST: Enclosure/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnclosureVM enclosureVM)
        {
            try
            {
                EmployeService employeService = new EmployeService(enclosureService.unitOfWork);

                Enclosure enclosure;
                try
                {
                    enclosure = enclosureService.GetByID(enclosureVM.EnclosureID);
                }
                catch (Exception ex)
                {
                    return View("EnclosureError", new HandleErrorInfo(ex, "Enclosure", "Index"));
                }
                
                enclosure.Number = enclosureVM.Number;
                enclosure.RootLogin = enclosureVM.RootLogin;
                enclosure.RootPassword = enclosureVM.RootPassword;

                try
                {
                    enclosure.Supervisor = employeService.GetByID(enclosureVM.EmployeID);
                }
                catch (Exception ex)
                {
                    return View("SupervisorError", new HandleErrorInfo(ex, "Enclosure", "Index"));
                }

                List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(enclosureVM.BlocksJSON);
                if (blocks != null) enclosure.Blocks = blocks;

                List<ACS> acss = JsonConvert.DeserializeObject<List<ACS>>(enclosureVM.ACSsJSON);
                enclosure.ACSs = acss;

                if (enclosure.Files != null)
                {
                    for (int i = 0; i < enclosure.Files.Count; i++)
                    {
                        enclosure.Files.ToList()[i].Temporary = false;
                    }
                }

                enclosure.Temporary = false;
                enclosureService.Update(enclosure);
                enclosureService.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Невозможно сохранить данные. Попробуйте снова, и если проблема останется, обратиатесь к вашему системному администратору.");
            }
            PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            return View(enclosureVM);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enclosure enclosure = enclosureService.GetByID(id);
            if (enclosure == null)
            {
                return HttpNotFound();
            }
            EnclosureVM enclosureVM = new EnclosureVM()
            {
                EnclosureID = enclosure.EnclosureID,
                Number = enclosure.Number,
                EmployeID = enclosure.Supervisor.EmployeID,
                RootLogin = enclosure.RootLogin,
                RootPassword = enclosure.RootPassword,
                BlocksJSON = JsonConvert.SerializeObject(enclosure.Blocks),
                ACSsJSON = JsonConvert.SerializeObject(enclosure.ACSs),
                FilesJSON = JsonConvert.SerializeObject(enclosure.Files.Select(x => new { name = x.Filename, extension = Path.GetExtension(x.Filename), size = x.Bytes.Length }).ToList())
            };
            PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            return View(enclosureVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EnclosureVM enclosureVM)
        {
            if (enclosureVM == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
            BlockService blockService = new BlockService(enclosureService.unitOfWork);
            ACSService aCSService = new ACSService(enclosureService.unitOfWork);

            Enclosure enclosureToUpdate;
            try
            {
                enclosureToUpdate = enclosureService.GetByID(enclosureVM.EnclosureID);
            }
            catch (Exception ex)
            {
                return View("EnclosureError", new HandleErrorInfo(ex, "Enclosure", "Index"));
            }

            enclosureToUpdate.Number = enclosureVM.Number;
            enclosureToUpdate.RootLogin = enclosureVM.RootLogin;
            enclosureToUpdate.RootPassword = enclosureVM.RootPassword;

            try
            {
                enclosureToUpdate.Supervisor = employeService.GetByID(enclosureVM.EmployeID);
            }
            catch (Exception ex)
            {
                return View("SupervisorError", new HandleErrorInfo(ex, "Enclosure", "Index"));
            }

            List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(enclosureVM.BlocksJSON);
            if (blocks != null) enclosureToUpdate.Blocks = blocks;

            List<ACS> acss = JsonConvert.DeserializeObject<List<ACS>>(enclosureVM.ACSsJSON);
            if (acss != null) enclosureToUpdate.ACSs = acss;

            if (enclosureToUpdate.Files != null)
            {
                for (int i = 0; i < enclosureToUpdate.Files.Count; i++)
                {
                    enclosureToUpdate.Files.ToList()[i].Temporary = false;
                }
            }

            try
            {
                enclosureService.Update(enclosureToUpdate);
                enclosureService.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Невозможно сохранить данные. Попробуйте снова, и если проблема останется, обратиатесь к вашему системному администратору.");
            }
            PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            return View(enclosureVM);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enclosure enclosure = enclosureService.GetByID(id);
            if (enclosure == null)
            {
                return HttpNotFound();
            }
            EnclosureVM enclosureVM = new EnclosureVM()
            {
                EnclosureID = enclosure.EnclosureID,
                Number = enclosure.Number,
                EmployeID = enclosure.Supervisor.EmployeID,
                RootLogin = enclosure.RootLogin,
                RootPassword = enclosure.RootPassword,
                //Blocks = enclosure.Blocks.ToList(),
                //ACSs = enclosure.ACSs.ToList(),
                FilesJSON = JsonConvert.SerializeObject(enclosure.Files.Select(x => new { name = x.Filename, extension = Path.GetExtension(x.Filename), size = x.Bytes.Length }).ToList())
            };
            PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            return View(enclosure);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enclosure enclosureToDelete = enclosureService.GetByID(id);
            enclosureToDelete.Supervisor = null;
            enclosureService.Delete(id);
            enclosureService.unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                enclosureService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region BlockPartial
        [HttpGet]
        public ActionResult GetBlocks(string blocks)
        {
            List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blocks);
            return PartialView("~/Views/Block/BlockTablePartial.cshtml", Blocks);
        }

        [HttpGet]
        public ActionResult CreateBlock(string blocks)
        {
            BlockVM blockVM = new BlockVM() { Blocks = blocks };
            PopulateBlockList(blockVM);
            return PartialView("~/Views/Block/CreateBlockPartial.cshtml", blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBlock(BlockVM blockVM)
        {
            if (ModelState.IsValid)
            {
                List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blockVM.Blocks);
                TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);
                Block block = new Block()
                {
                    UID = blockVM.UID,
                    BlockName = typicalBlockService.GetByID(blockVM.TypicalBlockID),
                };
                Blocks.Add(block);
                return Json(new { success = true, data = JsonConvert.SerializeObject(Blocks) }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView("~/Views/Block/CreateBlockPartial.cshtml", blockVM);
        }

        [HttpGet]
        public ActionResult EditBlock(string blocks, string blockToEdit)
        {
            Block BlockToEdit = JsonConvert.DeserializeObject<Block>(blockToEdit);

            BlockVM blockVM = new BlockVM()
            {
                TypicalBlockID = BlockToEdit.BlockName.TypicalBlockID,
                UID = BlockToEdit.UID,
                Blocks = blocks,
                OriginBlock = blockToEdit
            };
            PopulateBlockList(blockVM, blockVM.TypicalBlockID);
            return PartialView("~/Views/Block/EditBlockPartial.cshtml", blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBlock(BlockVM blockVM)
        {
            if (ModelState.IsValid)
            {
                List<Block> Blocks = JsonConvert.DeserializeObject<List<Block>>(blockVM.Blocks);

                TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);
                Block block = new Block()
                {
                    UID = blockVM.UID,
                    BlockName = typicalBlockService.GetByID(blockVM.TypicalBlockID),
                };
                Block originBlock = JsonConvert.DeserializeObject<Block>(blockVM.OriginBlock);
                int index = Blocks.IndexOf(originBlock);
                if (index >= 0)
                    Blocks[index] = block;
                return Json(new { success = true, data = JsonConvert.SerializeObject(Blocks) }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView("~/Views/Block/EditBlockPartial.cshtml", blockVM);
        }
        #endregion

        #region ACSPartial
        [HttpGet]
        public ActionResult GetACSs(string acssJSON)
        {
            List<ACS> acss = JsonConvert.DeserializeObject<List<ACS>>(acssJSON);
            return PartialView("~/Views/ACS/ACSTablePartial.cshtml", acss);
        }

        [HttpGet]
        public ActionResult CreateACS(string acssJSON)
        {
            AcsVM acsVM = new AcsVM { JsonACSs = acssJSON };
            return PartialView("~/Views/ACS/ACSPartial.cshtml", acsVM);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateACS(AcsVM acsVM)
        {
            if (ModelState.IsValid)
            {
                List<ACS> ACSs = JsonConvert.DeserializeObject<List<ACS>>(acsVM.JsonACSs);
                ACS acs = new ACS()
                {
                    Code = acsVM.Code
                };
                ACSs.Add(acs);
                return Json(new { success = true, data = JsonConvert.SerializeObject(ACSs) }, JsonRequestBehavior.AllowGet);
            }
            return PartialView("~/Views/ACS/ACSPartial.cshtml", acsVM);
        }

        [HttpGet]
        public ActionResult EditACS(string acssJSON)
        {
            //BlockService blockService = new BlockService(enclosureService.unitOfWork);
            //Block block = blockService.GetByID(id);
            BlockVM blockVM = new BlockVM();
            //{
            //    BlockID = block.BlockID,
            //    EnclosureID = block.EnclosureID,
            //    TypicalBlockID = block.BlockName.TypicalBlockID,
            //    UID = block.UID
            //};
            //PopulateBlockList(blockVM, blockVM.TypicalBlockID);
            return PartialView("~/Views/Block/BlockPartial.cshtml", blockVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditBlock(BlockVM blockVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (blockVM == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        BlockService blockService = new BlockService(enclosureService.unitOfWork);

        //        Block block = blockService.GetByID(blockVM.BlockID);

        //        TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);
        //        block.UID = blockVM.UID;
        //        block.BlockName = typicalBlockService.GetByID(blockVM.TypicalBlockID);

        //        if (enclosure.Blocks == null) enclosure.Blocks = new List<Block>();
        //        enclosure.Blocks.Add(block);
        //        enclosureService.Update(enclosure);
        //        enclosureService.Save();
        //        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    PopulateBlockList(blockVM);
        //    return PartialView("~/Views/Block/BlockPartial.cshtml", blockVM);
        //}
        #endregion

        #region Privates
        private void PopulateEmployeList(EnclosureVM encVM, object selectedEmployes = null)
        {
            EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
            SelectList empSelectList = new SelectList(employeService.Get().Select(emp => new SelectListItem { Text = emp.FullName + (emp.EmpPosition == null ? string.Empty : ", " + emp.EmpPosition.PosName), Value = emp.EmployeID.ToString() }), selectedEmployes);
            encVM.Employes = (IEnumerable<SelectListItem>)(empSelectList.Items);
        }

        private void PopulateBlockList(BlockVM blockVM, object selectedBlocks = null)
        {
            TypicalBlockService typicalBlock = new TypicalBlockService(enclosureService.unitOfWork);
            SelectList tBlockSelectList = new SelectList(typicalBlock.Get().Select(emp => new SelectListItem { Text = emp.BlockName, Value = emp.TypicalBlockID.ToString() }), selectedBlocks);
            blockVM.TypicalBlocks = (IEnumerable<SelectListItem>)(tBlockSelectList.Items);
        }
        #endregion

        #region File
        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> files, int id)
        {
            Enclosure enclosure = enclosureService.GetByID(id);
            if (files != null)
            {
                if (enclosure.Files == null) enclosure.Files = new List<EnclosureFile>();
                MemoryStream memoryStream = new MemoryStream();
                foreach (HttpPostedFileBase file in files)
                {
                    file.InputStream.CopyTo(memoryStream);
                    EnclosureFile enclosureFile = new EnclosureFile()
                    {
                        Filename = Path.GetFileName(file.FileName),
                        Bytes = memoryStream.ToArray(),
                        MimeType = MIME.GetMimeType(Path.GetFileName(file.FileName)),
                        Temporary = true
                    };

                    if (!enclosure.Files.Contains(enclosureFile)) enclosure.Files.Add(enclosureFile);
                    else return Content("Ошибка загрузки");
                }
            }
            enclosureService.Update(enclosure);
            enclosureService.Save();
            return Content("");
        }

        public ActionResult RemoveFile(string[] fileNames, int id)
        {
            if (fileNames != null)
            {
                FileService fileService = new FileService(enclosureService.unitOfWork);
                foreach (string fileName in fileNames)
                {
                    string filename = Path.GetFileName(fileName);
                    List<EnclosureFile> enclosureFiles = fileService.Get(x => x.EnclosureID == id && x.Filename == filename).ToList();
                    for (int i = 0; i < enclosureFiles.Count; i++)
                        fileService.Delete(enclosureFiles[i]);
                }
                fileService.Save();
                return Content("");
            }
            else return Content("Ошибка удаления");
        }

        public ActionResult DownloadFile(int fileID)
        {
            FileService fileService = new FileService(enclosureService.unitOfWork);
            EnclosureFile eFile = fileService.GetByID(fileID);
            if (eFile == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return File(eFile.Bytes, eFile.MimeType, eFile.Filename);
        }
        #endregion
    }
}