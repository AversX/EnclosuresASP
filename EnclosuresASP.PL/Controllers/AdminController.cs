using System.Web;
using System.Web.Mvc;
using EnclosuresASP.BLL.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using EnclosuresASP.PL.Models;
using EnclosuresASP.DAL.Identity;
using Microsoft.AspNet.Identity;
using System.Net;

namespace EnclosuresASP.PL.Controllers
{
    public class AdminController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public async Task<ActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = userVM.Name};
                IdentityResult result =
                    await UserManager.CreateAsync(user, userVM.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(userVM);
        }

        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                UserVM userVM = new UserVM()
                {
                    Id = user.Id,
                    Name = user.UserName
                };
                return View(userVM);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserVM userVM)
        {
            ModelState.Remove("Password");
            AppUser user = await UserManager.FindByIdAsync(userVM.Id);
            if (user != null)
            {
                user.UserName = userVM.Name;
                IdentityResult validName = await UserManager.UserValidator.ValidateAsync(user);
                if (!validName.Succeeded)
                {
                    AddErrorsFromResult(validName);
                }

                IdentityResult validPass = null;
                if (userVM.Password!=null && userVM.Password != string.Empty)
                {
                    validPass = await UserManager.PasswordValidator.ValidateAsync(userVM.Password);

                    if (validPass.Succeeded)
                    {
                        user.PasswordHash =
                            UserManager.PasswordHasher.HashPassword(userVM.Password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

                if ((validName.Succeeded && validPass == null) || (validName.Succeeded && userVM.Password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }
            return View(userVM);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "Пользователь не найден" });
            }
        }
    }
}
