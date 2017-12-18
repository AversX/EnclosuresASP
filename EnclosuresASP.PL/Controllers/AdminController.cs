using EnclosuresASP.DAL.Identity;
using EnclosuresASP.DAL.Infrastructure;
using EnclosuresASP.PL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EnclosuresASP.PL.Controllers
{
    [Authorize(Roles = "Administrator")]
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

        [HttpGet]
        public ActionResult Users()
        {
            return View(UserManager.Users);
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            UserVM userVM = new UserVM
            {
                Roles = RoleManager.Roles.ToList(),
                RoleIDs = new System.Collections.Generic.List<string>()
            };
            return View(userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = userVM.Name};
                IdentityResult result = UserManager.Create(user, userVM.Password);
                IdentityResult roleResult;
                if (result.Succeeded)
                {
                    for (int i = 0; i < userVM.RoleIDs.Count; i++)
                    {
                        AppRole role = RoleManager.FindById(userVM.RoleIDs[i]);
                        roleResult = UserManager.AddToRole(user.Id, role.Name);
                        if (!roleResult.Succeeded)
                        {
                            return View("Error", roleResult.Errors);
                        }
                    }
                    return RedirectToAction("Users");
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
                    Name = user.UserName,
                    Roles = RoleManager.Roles.ToList(),
                    RoleIDs = user.Roles.Select(x => x.RoleId).ToList()
                };
                return View(userVM);
            }
            else
            {
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(UserVM userVM)
        {
            ModelState.Remove("Password");
            AppUser user = await UserManager.FindByIdAsync(userVM.Id);
            if (user != null && userVM.RoleIDs?.Count>0)
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
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(userVM.Password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

                if ((validName.Succeeded && validPass == null) || (validName.Succeeded && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);

                    string[] allUserRoles = UserManager.GetRoles(user.Id).ToArray();
                    UserManager.RemoveFromRoles(user.Id, allUserRoles);
                    IdentityResult roleResult = null;
                    foreach (string roleId in userVM.RoleIDs)
                    {
                        AppRole role = await RoleManager.FindByIdAsync(roleId);

                        roleResult = await RoleManager.RoleValidator.ValidateAsync(role);
                        if (roleResult.Succeeded)
                        {
                            roleResult = await UserManager.AddToRoleAsync(user.Id, role.Name);
                            if (!roleResult.Succeeded) AddErrorsFromResult(roleResult);
                        }
                        else AddErrorsFromResult(roleResult);
                    }

                    if (result.Succeeded && roleResult.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else ModelState.AddModelError("", "Пользователю не назначена ни одна роль");
            userVM.Roles = RoleManager.Roles.ToList();
            userVM.RoleIDs = userVM.RoleIDs == null ? new System.Collections.Generic.List<string>() : userVM.RoleIDs;
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
    }
}
