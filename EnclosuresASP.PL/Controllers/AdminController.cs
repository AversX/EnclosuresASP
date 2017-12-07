using System.Web;
using System.Web.Mvc;
using EnclosuresASP.BLL.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using EnclosuresASP.PL.Models;
using EnclosuresASP.DAL.Identity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EnclosuresASP.PL.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #region User
        [HttpGet]
        public ActionResult Users()
        {
            return View(UserManager.Users);
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = userVM.Name };
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

        [HttpGet]
        public async Task<ActionResult> EditUser(string id)
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(UserVM userVM)
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
                if (userVM.Password != null && userVM.Password != string.Empty)
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

        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string id)
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
        #endregion

        #region Role
        [HttpGet]
        public ActionResult Roles()
        {
            return View(RoleManager.Roles);
        }

        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(AppRole role)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result =
                    await RoleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(role);
        }

        [HttpGet]
        public async Task<ActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(AppRole role)
        {
            if (ModelState.IsValid)
            {
                AppRole roleToUpdate = await RoleManager.FindByIdAsync(role.Id);
                if (roleToUpdate == null)
                {
                    return HttpNotFound();
                }
                roleToUpdate.Name = role.Name;
                IdentityResult validName = await RoleManager.RoleValidator.ValidateAsync(roleToUpdate);
                if (validName.Succeeded)
                {
                    IdentityResult result = await RoleManager.UpdateAsync(roleToUpdate);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Роль не найдена");
                }
            }
            return View(role);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleVM roleVM = new RoleVM
            {
                Id = role.Id,
                Name = role.Name,
                Users = UserManager.Users.ToList()
            };

            return View();
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRoleConfirmed(string id)
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
        #endregion













    }
}
