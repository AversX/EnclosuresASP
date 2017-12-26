using EnclosuresASP.BLL;
using EnclosuresASP.BLL.Services;
using EnclosuresASP.DAL.Entities;
using EnclosuresASP.PL.ActivityTrack;
using EnclosuresASP.PL.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System;

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
                Blocks = new List<Block>()
            };
            enclosureService.Insert(enclosure);
            enclosureService.Save();
            EnclosureVM enclosureVM = new EnclosureVM()
            {
                EnclosureID = enclosure.EnclosureID,
                Username = enclosure.Username,
                AcceptanceDate = DateTime.Now
            };
            PopulateEmployeList(enclosureVM);
            return View(enclosureVM);
        }

        // POST: Enclosure/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnclosureVM enclosureVM)
        {
            if (ModelState.IsValid)
            {
                EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
                BlockService blockService = new BlockService(enclosureService.unitOfWork);
                TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);

                Enclosure enclosure;
                enclosure = enclosureService.GetByID(enclosureVM.EnclosureID);
                enclosure.Number = enclosureVM.Number;
                enclosure.ElisNumber = enclosureVM.ElisNumber;
                enclosure.AcceptanceDate = enclosureVM.AcceptanceDate?.ToShortDateString();
                enclosure.Lvl1Password = enclosureVM.Lvl1Password;
                enclosure.Lvl2Password = enclosureVM.Lvl2Password;
                enclosure.Lvl3Password = enclosureVM.Lvl3Password;
                enclosure.Lvl4Password = enclosureVM.Lvl4Password;
                enclosure.Lvl5Password = enclosureVM.Lvl5Password;
                enclosure.Object = enclosureVM.Object;
                enclosure.Comment = enclosureVM.Comment;
                enclosure.Supervisor = enclosureVM.EmployeID == null ? null : employeService.GetByID(enclosureVM.EmployeID);


                List<Block> blocks = blockService.Get(x => x.EnclosureID == enclosure.EnclosureID).ToList();
                if (enclosure.Blocks == null) enclosure.Blocks = new List<Block>();
                if (blocks != null)
                {
                    for (int i = 0; i < enclosure.Blocks.Count(); i++)
                    {
                        int index = blocks.FindIndex(x => x.BlockGuid == enclosure.Blocks.ToList()[i].BlockGuid);
                        if (index >= 0)
                        {
                            enclosure.Blocks.ToList()[i].BlockName = blocks[index].BlockName == null ? null : typicalBlockService.GetByID(blocks[index].BlockName.TypicalBlockID);
                            enclosure.Blocks.ToList()[i].UID = blocks[index].UID;
                            enclosure.Blocks.ToList()[i].Number = blocks[index].Number;
                            enclosure.Blocks.ToList()[i].SoftwareVersion = blocks[index].SoftwareVersion;
                            enclosure.Blocks.ToList()[i].Comment = blocks[index].Comment;
                            blocks.RemoveAt(index);
                        }
                        else
                        {
                            blockService.Delete(enclosure.Blocks.ToList()[i]);
                            i--;
                        }
                    }
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        Block block = new Block()
                        {
                            BlockGuid = blocks[i].BlockGuid,
                            UID = blocks[i].UID,
                            EnclosureID = blocks[i].EnclosureID,
                            Number = blocks[i].Number,
                            SoftwareVersion = blocks[i].SoftwareVersion,
                            Comment = blocks[i].Comment,
                            BlockName = blocks[i].BlockName == null ? null : typicalBlockService.GetByID(blocks[i].BlockName.TypicalBlockID)
                        };
                        enclosure.Blocks.Add(block);
                    }
                }

                FileService fileService = new FileService(enclosureService.unitOfWork);
                List<EnclosureFile> files = fileService.Get(x => x.EnclosureID == enclosure.EnclosureID).ToList();
                if (enclosure.Files == null) enclosure.Files = new List<EnclosureFile>();
                if (files?.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].Temporary = false;
                        enclosure.Files.Add(files[i]);
                    }
                }

                enclosure.Temporary = false;
                enclosureService.Update(enclosure);
                enclosureService.Save();
                return RedirectToAction("Index");
            }
            if (enclosureVM.EmployeID != null)
                PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            else
                PopulateEmployeList(enclosureVM);
            return View(enclosureVM);
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
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
                ElisNumber = enclosure.ElisNumber,
                AcceptanceDate = DateTime.Parse(enclosure.AcceptanceDate),
                Lvl1Password = enclosure.Lvl1Password,
                Lvl2Password = enclosure.Lvl2Password,
                Lvl3Password = enclosure.Lvl3Password,
                Lvl4Password = enclosure.Lvl4Password,
                Lvl5Password = enclosure.Lvl5Password,
                Object = enclosure.Object,
                Comment = enclosure.Comment,
                FilesJSON = JsonConvert.SerializeObject(enclosure.Files.Select(x => new { name = x.Filename, extension = Path.GetExtension(x.Filename), size = x.Bytes.Length }).ToList()),
                Version = enclosure.Version
            };
            if (enclosure.Supervisor == null)
            {
                enclosureVM.EmployeID = null;
                PopulateEmployeList(enclosureVM);
            }
            else
            {
                enclosureVM.EmployeID = enclosure.Supervisor.EmployeID;
                PopulateEmployeList(enclosureVM, enclosure.Supervisor.EmployeID);
            }
            ViewBag.returnUrl = returnUrl;
            return View(enclosureVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EnclosureVM enclosureVM, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
                BlockService blockService = new BlockService(enclosureService.unitOfWork);
                TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);

                Enclosure enclosureToUpdate = enclosureService.GetByID(enclosureVM.EnclosureID);
                enclosureToUpdate = enclosureService.GetByID(enclosureVM.EnclosureID);
                enclosureToUpdate.Number = enclosureVM.Number;
                enclosureToUpdate.ElisNumber = enclosureVM.ElisNumber;
                enclosureToUpdate.AcceptanceDate = enclosureVM.AcceptanceDate?.ToShortDateString();
                enclosureToUpdate.Lvl1Password = enclosureVM.Lvl1Password;
                enclosureToUpdate.Lvl2Password = enclosureVM.Lvl2Password;
                enclosureToUpdate.Lvl3Password = enclosureVM.Lvl3Password;
                enclosureToUpdate.Lvl4Password = enclosureVM.Lvl4Password;
                enclosureToUpdate.Lvl5Password = enclosureVM.Lvl5Password;
                enclosureToUpdate.Object = enclosureVM.Object;
                enclosureToUpdate.Comment = enclosureVM.Comment;
                enclosureToUpdate.Supervisor = enclosureVM.EmployeID == null ? null : employeService.GetByID(enclosureVM.EmployeID);
                enclosureToUpdate.Version = enclosureVM.Version;

                List<Block> blocks = blockService.Get(x => x.EnclosureID == enclosureToUpdate.EnclosureID).ToList();
                if (enclosureToUpdate.Blocks == null) enclosureToUpdate.Blocks = new List<Block>();
                if (blocks != null)
                {
                    for (int i = 0; i < enclosureToUpdate.Blocks.Count(); i++)
                    {
                        int index = blocks.FindIndex(x => x.BlockGuid == enclosureToUpdate.Blocks.ToList()[i].BlockGuid);
                        if (index >= 0)
                        {
                            enclosureToUpdate.Blocks.ToList()[i].BlockName = blocks[index].BlockName == null ? null : typicalBlockService.GetByID(blocks[index].BlockName.TypicalBlockID);
                            enclosureToUpdate.Blocks.ToList()[i].UID = blocks[index].UID;
                            enclosureToUpdate.Blocks.ToList()[i].Number = blocks[index].Number;
                            enclosureToUpdate.Blocks.ToList()[i].SoftwareVersion = blocks[index].SoftwareVersion;
                            enclosureToUpdate.Blocks.ToList()[i].Comment = blocks[index].Comment;
                            blocks.RemoveAt(index);
                        }
                        else
                        {
                            blockService.Delete(enclosureToUpdate.Blocks.ToList()[i]);
                            i--;
                        }
                    }
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        Block block = new Block()
                        {
                            BlockGuid = blocks[i].BlockGuid,
                            UID = blocks[i].UID,
                            EnclosureID = blocks[i].EnclosureID,
                            BlockName = blocks[i].BlockName == null ? null : typicalBlockService.GetByID(blocks[i].BlockName.TypicalBlockID),
                            Number = blocks[i].Number,
                            SoftwareVersion = blocks[i].SoftwareVersion,
                            Comment = blocks[i].Comment
                        };
                        enclosureToUpdate.Blocks.Add(block);
                    }
                }

                FileService fileService = new FileService(enclosureService.unitOfWork);
                List<EnclosureFile> files = fileService.Get(x => x.EnclosureID == enclosureToUpdate.EnclosureID).ToList();
                if (enclosureToUpdate.Files == null) enclosureToUpdate.Files = new List<EnclosureFile>();
                if (files?.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].Temporary = false;
                        enclosureToUpdate.Files.Add(files[i]);

                    }
                }

                try
                {
                    enclosureService.Update(enclosureToUpdate);
                    enclosureService.Save();
                    return Redirect(returnUrl);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", "Объект был изменён другим пользователем. Внесённые вами изменения сохранены не будут. Откройте объект заново, чтобы отобразить актуальные данные.");
                }

            }
            if (enclosureVM.EmployeID != null)
                PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            else
                PopulateEmployeList(enclosureVM);
            return View("Edit", enclosureVM);
        }

        [HttpGet]
        public ActionResult Delete(int? id, string returnUrl)
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
            ViewBag.returnUrl = returnUrl;
            return View(enclosure);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Enclosure enclosure, string returnUrl)
        {
            try
            {
                enclosureService.Delete(enclosure.EnclosureID, enclosure.Version);
                enclosureService.Save();
                return Redirect(returnUrl);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError("", "Объект был изменён другим пользователем. Удаление не произведено. Ниже представлены актуализированные данные.");
            }
            enclosure = enclosureService.GetByID(enclosure.EnclosureID);
            if (enclosure == null)
            {
                return HttpNotFound();
            }
            ViewBag.returnUrl = returnUrl;
            return View(enclosure);


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                enclosureService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Blocks
        [HttpGet]
        public ActionResult BlocksIndex(int? id)
        {
            BlockService blockService = new BlockService(enclosureService.unitOfWork);
            List<Block> blocks = blockService.Get(x => x.EnclosureID == id).ToList();
            return PartialView(blocks);
        }

        [HttpGet]
        public ActionResult CreateBlock(int id)
        {
            BlockVM blockVM = new BlockVM() { EnclosureID = id };
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBlock(BlockVM blockVM)
        {
            if (ModelState.IsValid)
            {
                BlockService blockService = new BlockService(enclosureService.unitOfWork);
                TypicalBlockService typicalBlockService = new TypicalBlockService(enclosureService.unitOfWork);
                Block block = new Block()
                {
                    UID = blockVM.UID,
                    EnclosureID = blockVM.EnclosureID,
                    BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID),
                    Number = blockVM.Number,
                    SoftwareVersion = blockVM.SoftwareVersion,
                    Comment = blockVM.Comment
                };
                blockService.Insert(block);
                blockService.Save();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpGet]
        public ActionResult EditBlock(int? id, string blockGuid)
        {
            BlockService blockService = new BlockService(enclosureService.unitOfWork);
            Guid BlockGuid = JsonConvert.DeserializeObject<Guid>(blockGuid);
            Block block = blockService.Get(x => x.BlockGuid == BlockGuid).FirstOrDefault();
            if (block != null)
            {
                BlockVM blockVM = new BlockVM()
                {
                    UID = block.UID,
                    BlockGuid = block.BlockGuid,
                    BlockName = block.BlockName,
                    Number = block.Number,
                    SoftwareVersion = block.SoftwareVersion,
                    Comment = block.Comment,
                    EnclosureID = block.EnclosureID,
                    Version = block.Version
                };
                if (block.BlockName == null)
                    blockVM.TypicalBlockID = null;
                else
                    blockVM.TypicalBlockID = block.BlockName.TypicalBlockID;

                PopulateBlockList(blockVM, blockVM.TypicalBlockID);
                return PartialView(blockVM);
            }
            else return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBlock(BlockVM blockVM, string blockGuid)
        {
            if (ModelState.IsValid)
            {
                TypicalBlockService typicalBlockService = new TypicalBlockService();
                BlockService blockService = new BlockService(enclosureService.unitOfWork);
                Guid BlockGuid = Guid.Parse(blockGuid);
                Block block = blockService.Get(x => x.BlockGuid == BlockGuid).FirstOrDefault();
                if (block!=null)
                {
                    block.UID = blockVM.UID;
                    block.BlockName = blockVM.TypicalBlockID == null ? null : typicalBlockService.GetByID(blockVM.TypicalBlockID);
                    block.Number = blockVM.Number;
                    block.SoftwareVersion = blockVM.SoftwareVersion;
                    block.Comment = blockVM.Comment;
                    block.Version = blockVM.Version;
                    blockService.Update(block);
                    blockService.Save();
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            PopulateBlockList(blockVM);
            return PartialView(blockVM);
        }

        [HttpGet]
        public ActionResult DeleteBlock(int? id, string blockGuid)
        {
            BlockService blockService = new BlockService(enclosureService.unitOfWork);
            Guid BlockGuid = JsonConvert.DeserializeObject<Guid>(blockGuid);
            Block block = blockService.Get(x => x.BlockGuid == BlockGuid).FirstOrDefault();
            if (block != null)
            {
                BlockVM blockVM = new BlockVM()
                {
                    UID = block.UID,
                    BlockGuid = block.BlockGuid,
                    BlockName = block.BlockName,
                    Number = block.Number,
                    SoftwareVersion = block.SoftwareVersion,
                    Comment = block.Comment,
                    EnclosureID = block.EnclosureID,
                    Version = block.Version
                };
                return PartialView(blockVM);
            }
            else return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBlock(BlockVM blockVM, string blockGuid)
        { //VERSION!
            BlockService blockService = new BlockService(enclosureService.unitOfWork);
            Guid BlockGuid = JsonConvert.DeserializeObject<Guid>(blockGuid);
            Block block = blockService.Get(x => x.BlockGuid == BlockGuid).FirstOrDefault();
            try
            {
                blockService.Delete(block);
                blockService.Save();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError("", "Объект был изменён другим пользователем. Удаление не произведено. Ниже представлены актуализированные данные.");
            }
            blockVM = new BlockVM()
            {
                UID = block.UID,
                BlockGuid = block.BlockGuid,
                BlockName = block.BlockName,
                Number = block.Number,
                SoftwareVersion = block.SoftwareVersion,
                Comment = block.Comment,
                EnclosureID = block.EnclosureID,
                Version = block.Version
            };
            return PartialView(blockVM);
        }
        #endregion

        #region Privates
        private void PopulateEmployeList(EnclosureVM encVM, object selectedEmploye = null)
        {
            EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
            SelectList empSelectList = new SelectList(employeService.Get().Select(emp => new SelectListItem { Text = emp.FullName + (emp.EmpPosition == null ? string.Empty : ", " + emp.EmpPosition.PosName), Value = emp.EmployeID.ToString() }), selectedEmploye);
            encVM.Employes = (IEnumerable<SelectListItem>)(empSelectList.Items);
        }

        private void PopulateBlockList(BlockVM blockVM, object selectedBlocks = null)
        {
            TypicalBlockService typicalBlock = new TypicalBlockService();
            SelectList tBlockSelectList = new SelectList(typicalBlock.Get().Select(emp => new SelectListItem { Text = emp.BlockName, Value = emp.TypicalBlockID.ToString() }), selectedBlocks);
            blockVM.TypicalBlocks = (IEnumerable<SelectListItem>)(tBlockSelectList.Items);
        }
        #endregion

        #region File
        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> files, int id)
        {
            Enclosure enclosure = enclosureService.GetByID(id);
            FileService fileService = new FileService(enclosureService.unitOfWork);
            if (files != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                foreach (HttpPostedFileBase file in files)
                {
                    file.InputStream.CopyTo(memoryStream);
                    EnclosureFile enclosureFile = new EnclosureFile()
                    {
                        Filename = Path.GetFileName(file.FileName),
                        Bytes = memoryStream.ToArray(),
                        MimeType = MIME.GetMimeType(Path.GetFileName(file.FileName)),
                        Temporary = true,
                        EnclosureVersion = enclosure.Version.ToString(),
                        EnclosureID = enclosure.EnclosureID
                    };
                    fileService.Insert(enclosureFile);
                    fileService.Save();
                }
                return Content("");
            }
            else return Content("Error");
        }

        public ActionResult RemoveFile(string[] fileNames, int id)
        {
            if (fileNames != null)
            {
                Enclosure enclosure = enclosureService.GetByID(id);
                FileService fileService = new FileService(enclosureService.unitOfWork);
                foreach (string fileName in fileNames)
                {
                    string filename = Path.GetFileName(fileName);
                    List<EnclosureFile> enclosureFiles = fileService.Get(x => x.EnclosureID == enclosure.EnclosureID && x.Filename == filename).ToList();
                    if (enclosureFiles?.Count >= 0)
                        for (int i = 0; i < enclosureFiles.Count; i++)
                        {
                            fileService.Delete(enclosureFiles[i]);
                        }
                    else return Content("Ошибка удаления");
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