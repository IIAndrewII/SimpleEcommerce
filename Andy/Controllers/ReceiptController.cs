using Andy.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Models;

namespace Andy.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceiptController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // GET: /Receipt/Index
        [Authorize]
        public IActionResult Index(Receipt Receipt)
        {
            return View(Receipt);
        }
        // GET: Receipts
        public ActionResult Receipts()
        {
            var receipts = _unitOfWork.Receipts.GetAll();
            return View(receipts);
        }
        // POST: /Receipt/Save
        [HttpPost]
        [Authorize]
        public IActionResult Save(Receipt Receipt, string location)
        {
            // Get the product from the Session
            List<Product> RProducts = HttpContext.Session.Get<List<Product>>("Cart") ?? new List<Product>();

            if (RProducts == null)
            {
                return NotFound();
            }


            decimal totalprice = 0; // Declare and initialize totalprice outside the loop



            // Retrieve session value as a string
            string userIdString = HttpContext.Session.GetString("UserId");

            // Define a variable to store the parsed user ID
            int userId;

            // Parse the string value to an integer
            if (int.TryParse(userIdString, out userId))
            {
                // Parsing successful, userId now holds the integer value
            }
            else
            {
                // Parsing failed, assign zero as the default value
                userId = 1;
            }



            foreach (var group in RProducts.GroupBy(p => p.Id))
            {
                var product = group.First(); // Get the first product in the group
                var quantity = group.Count(); // Count the number of occurrences of the product
                totalprice = totalprice + (product.Price * quantity); // Update totalprice

            }


            // Update product stock quantity
            foreach (var product in RProducts)
            {
                var dbProduct = _unitOfWork.products.GetById(product.Id);
                if (dbProduct != null)
                {
                    dbProduct.StockQuantity -= 1;
                }
            }


            // Create a Receipt object
            var receipt = new Receipt
            {
                TotalPrice = totalprice,
                Location = location,
                UserAccountID = userId,
                CreatedAt = DateTime.Now,
                Items = RProducts.Select(p => new ReceiptItem
                {
                    Name = p.Name,
                    ProductID = p.Id,
                    Description = p.Description,
                    Price = p.Price
                }).ToList()
            };

            // Add receipt to the database
            _unitOfWork.Receipts.Add(receipt);
            _unitOfWork.Complete();

            // Remove cart data from session
            HttpContext.Session.Remove("Cart");


            return RedirectToAction("SecurePage", "Account");
            //return RedirectToAction("Details", "Receipts", new { id = receipt.Id });

        }
        public ActionResult Details(int id)
        {
            string[] Items = { "Items" };
            Receipt receipt = _unitOfWork.Receipts.Find(p => p.Id == id, Items);


            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }
    }
}
