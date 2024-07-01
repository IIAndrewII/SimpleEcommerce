using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Models;
using System.Security.Claims;

namespace Andy.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.UserAccounts.GetAll());
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserAccount model)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                string hashedPassword = new PasswordHashingService().HashPassword(model.Password);

                UserAccount account = new UserAccount();
                account.Email = model.Email;
                account.Password = hashedPassword;
                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.PhoneNumber = model.PhoneNumber;

                try
                {
                    _unitOfWork.UserAccounts.Add(account);
                    _unitOfWork.Complete();

                    ModelState.Clear();
                    ViewBag.Message = $"{account.FirstName} {account.LastName} registered successfully, Please Login.";
                    return View();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }

            }
            return View(model);
        }

        // GET: UserAccounts/Details/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _unitOfWork.UserAccounts
                .GetByIdAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // GET: UserAccounts/Create
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create([Bind("ID,Role,FirstName,LastName,Email,Password,PhoneNumber")] UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                // Hash the password
                userAccount.Password = new PasswordHashingService().HashPassword(userAccount.Password);

                _unitOfWork.UserAccounts.Add(userAccount);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }

        // GET: UserAccounts/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _unitOfWork.UserAccounts.GetByIdAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        // POST: UserAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Role,FirstName,LastName,Email,Password,PhoneNumber")] UserAccount userAccount)
        {
            if (id != userAccount.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Hash the password
                    userAccount.Password = new PasswordHashingService().HashPassword(userAccount.Password);
                    _unitOfWork.UserAccounts.Update(userAccount);
                    _unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }

        // GET: UserAccounts/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _unitOfWork.UserAccounts.GetByIdAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAccount = _unitOfWork.UserAccounts.GetById(id);
            _unitOfWork.UserAccounts.Delete(userAccount);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            var userAccount = _unitOfWork.UserAccounts.GetByIdAsync(id);
            if (userAccount == null)
            {
                return false;
            }
            return true;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserAccount model)
        {
            if (true)
            {

                var user = _unitOfWork.UserAccounts.Find(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // success: create a session
                    _httpContextAccessor.HttpContext.Session.SetString("UserId", user.ID.ToString());
                    _httpContextAccessor.HttpContext.Session.SetString("UserEmail", user.Email);
                    _httpContextAccessor.HttpContext.Session.SetString("UserName", user.FirstName);

                    // success: create a cookie and redirect
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.FirstName),
                        new Claim(ClaimTypes.Role, user.Role)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    //return RedirectToAction("SecurePage");
                    return RedirectToAction("Index", "product");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password is not correct");
                }
            }
            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            ViewBag.Name = HttpContext.User.Identity.Name;
            return View();
        }
    }
}
