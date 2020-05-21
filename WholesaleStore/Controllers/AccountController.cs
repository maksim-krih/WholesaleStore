using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models;

namespace WholesaleStore.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager
            ) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.EmployeeRepository.Query
                    .Include(x => x.Position),
                    x => model.Login == x.Login && model.Password == x.Password
                );

                if (user == null)
                {
                    ModelState.AddModelError("", "Wrong login or password");
                }
                else
                {
                    Authenticate(user, user.Position.Name);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        private void Authenticate(Employee user, string role)
        {
            var claim = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            claim.AddClaims(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                    "OWIN Provider", ClaimValueTypes.String)
            });

            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, claim);
        }
    }
}