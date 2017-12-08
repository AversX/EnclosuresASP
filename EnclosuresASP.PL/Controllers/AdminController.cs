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

        #region User
        [HttpGet]
        public ActionResult Users()
        {
            return View(UserManager.Users.Where(x => x.UserName!="Root"));
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            UserVM userVM = new UserVM();
            PopulateRolesList(userVM);
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
            PopulateRolesList(userVM);
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
                PopulateRolesList(userVM);
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
            if (user != null && userVM.RoleIDs.Count>0)
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

                if ((validName.Succeeded && validPass == null) || (validName.Succeeded && userVM.Password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    foreach (IdentityUserRole userRole in user.Roles)
                    {
                        AppRole role = RoleManager.FindById(userRole.RoleId);
                        if (role == null)
                        {
                            if (!userVM.RoleIDs.Contains(role.Id))
                            {

                            }
                            IdentityResult roleResult = await UserManager.AddToRoleAsync(user.Id, role.Name);
                            if (!roleResult.Succeeded)
                            {
                                AddErrorsFromResult(result);
                            }
                        }

                    }
                    return RedirectToAction("Users");
                    //foreach (IdentityUserRole userRole in user.Roles)
                    //{
                    //    AppRole role = RoleManager.FindById(userRole.RoleId);

                    //}
                    //for (int i = 0; i < user.Roles.Count; i++)
                    //{

                    //    if (role==null)
                    //    {
                    //        IdentityResult roleResult = await UserManager.AddToRoleAsync(user.Id, role.Name);
                    //        if (!roleResult.Succeeded)
                    //        {
                    //            AddErrorsFromResult(result);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        IdentityResult roleResult = await UserManager.RemoveFromRoleAsync(user.Id, role.Name);
                    //        if (!roleResult.Succeeded)
                    //        {
                    //            AddErrorsFromResult(result);
                    //        }
                    //    }
                    //}

                    //for (int i=0; i<userVM.RoleIDs.Count; i++)
                    //{
                    //    AppRole role = RoleManager.FindById(userVM.RoleIDs[i]);
                    //    if (role != null)
                    //    {
                    //        if (!UserManager.IsInRole(userVM.Id, role.Name))
                    //        {


                    //        }
                    //    }
                    //}

                    //    
                    //if (result.Succeeded && (roleResult.Succeeded || roleResult == null))
                    //{
                    //    return RedirectToAction("Index");
                    //}
                    //else
                    //{
                    //    AddErrorsFromResult(result);
                    //}
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

        private void PopulateRolesList(UserVM userVM, object selectedRole = null)
        {
            userVM.Roles = RoleManager.Roles.ToList();
        }
        #endregion

        #region Role
        //[HttpGet]
        //public ActionResult Roles()
        //{
        //    return View(RoleManager.Roles);
        //}

        //[HttpGet]
        //public ActionResult CreateRole()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> CreateRole(AppRole role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityResult result =
        //            await RoleManager.CreateAsync(role);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Roles");
        //        }
        //        else
        //        {
        //            AddErrorsFromResult(result);
        //        }
        //    }
        //    return View(role);
        //}

        //[HttpGet]
        //public async Task<ActionResult> EditRole(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AppRole role = await RoleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(role);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditRole(AppRole role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppRole roleToUpdate = await RoleManager.FindByIdAsync(role.Id);
        //        if (roleToUpdate == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        roleToUpdate.Name = role.Name;
        //        IdentityResult validName = await RoleManager.RoleValidator.ValidateAsync(roleToUpdate);
        //        if (validName.Succeeded)
        //        {
        //            IdentityResult result = await RoleManager.UpdateAsync(roleToUpdate);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Roles");
        //            }
        //            else
        //            {
        //                AddErrorsFromResult(result);
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Роль не найдена");
        //        }
        //    }
        //    return View(role);
        //}

        //[HttpGet]
        //public async Task<ActionResult> DeleteRole(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AppRole role = await RoleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    RoleVM roleVM = new RoleVM
        //    {
        //        Id = role.Id,
        //        Name = role.Name,
        //        Users = new List<AppUser>()
        //    };
        //    for (int i=0; i<UserManager.Users.Count(); i++)
        //    {
        //        if (UserManager.IsInRole(UserManager.Users.ToList()[i].Id, role.Id))
        //            roleVM.Users.Add(UserManager.Users.ToList()[i]);
        //    }
        //    return View(roleVM);
        //}

        //[HttpPost, ActionName("DeleteRole")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteRoleConfirmed(string id)
        //{
        //    AppUser user = await UserManager.FindByIdAsync(id);

        //    if (user != null)
        //    {
        //        IdentityResult result = await UserManager.DeleteAsync(user);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return View("Error", result.Errors);
        //        }
        //    }
        //    else
        //    {
        //        return View("Error", new string[] { "Роль не найдена" });
        //    }
        //}
        #endregion
    }
}
