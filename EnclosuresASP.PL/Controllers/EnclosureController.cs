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
                BlocksJSON = JsonConvert.SerializeObject(enclosure.Blocks)
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
                enclosure.RootLogin = enclosureVM.RootLogin;
                enclosure.RootPassword = enclosureVM.RootPassword;
                enclosure.Supervisor = enclosureVM.EmployeID == null ? null : employeService.GetByID(enclosureVM.EmployeID);


                List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(enclosureVM.BlocksJSON);
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
                            BlockName = blocks[i].BlockName == null ? null : typicalBlockService.GetByID(blocks[i].BlockName.TypicalBlockID)
                        };
                        enclosure.Blocks.Add(block);
                    }
                }

                if (enclosure.Files != null)
                {
                    for (int i = 0; i < enclosure.Files.Count; i++)
                    {
                        enclosure.Files.ToList()[i].Temporary = false;
                    }
                }

                enclosure.Temporary = false;
                enclosureService.Insert(enclosure);
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
                RootLogin = enclosure.RootLogin,
                RootPassword = enclosure.RootPassword,
                BlocksJSON = JsonConvert.SerializeObject(enclosure.Blocks),
                FilesJSON = JsonConvert.SerializeObject(enclosure.Files.Select(x => new { name = x.Filename, extension = Path.GetExtension(x.Filename), size = x.Bytes.Length }).ToList()),
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
                enclosureToUpdate.Number = enclosureVM.Number;
                enclosureToUpdate.RootLogin = enclosureVM.RootLogin;
                enclosureToUpdate.RootPassword = enclosureVM.RootPassword;
                enclosureToUpdate.Supervisor = enclosureVM.EmployeID == null ? null : employeService.GetByID(enclosureVM.EmployeID);

                List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(enclosureVM.BlocksJSON);
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
                            BlockName = blocks[i].BlockName == null ? null : typicalBlockService.GetByID(blocks[i].BlockName.TypicalBlockID)
                        };
                        enclosureToUpdate.Blocks.Add(block);
                    }
                }

                enclosureService.Update(enclosureToUpdate);
                enclosureService.Save();
                return Redirect(returnUrl);
            }
            if (enclosureVM.EmployeID != null)
                PopulateEmployeList(enclosureVM, enclosureVM.EmployeID);
            else
                PopulateEmployeList(enclosureVM);
            return View(enclosureVM);
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
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Enclosure enclosureToDelete = enclosureService.GetByID(id);
            //enclosureService.Delete(id);
            enclosureService.Save();
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                enclosureService.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Privates
        private void PopulateEmployeList(EnclosureVM encVM, object selectedEmploye = null)
        {
            EmployeService employeService = new EmployeService(enclosureService.unitOfWork);
            SelectList empSelectList = new SelectList(employeService.Get().Select(emp => new SelectListItem { Text = emp.FullName + (emp.EmpPosition == null ? string.Empty : ", " + emp.EmpPosition.PosName), Value = emp.EmployeID.ToString() }), selectedEmploye);
            encVM.Employes = (IEnumerable<SelectListItem>)(empSelectList.Items);
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