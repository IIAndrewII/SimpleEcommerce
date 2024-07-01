using Andy.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Models;

namespace Andy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Product
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.products.GetAllAsync();
            return View(products);
        }

        // GET: Product/AddToCart
        [Authorize]
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product? product = await _unitOfWork.products.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Implementing a basic in-memory shopping cart
            List<Product> cart = HttpContext.Session.Get<List<Product>>("Cart") ?? new List<Product>();
            cart.Add(product);
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index", "Cart"); // Redirect to the cart page
        }
    }
}
