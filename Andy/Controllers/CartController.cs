using Andy.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core.Models;

namespace Andy.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        [Authorize]
        public IActionResult Index()
        {
            List<Product> cart = HttpContext.Session.Get<List<Product>>("Cart") ?? new List<Product>();
            return View(cart);
        }
    }
}
