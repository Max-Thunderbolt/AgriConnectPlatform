using Microsoft.AspNetCore.Mvc;
using AgriConnectPlatform.Models;
using AgriConnectPlatform.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AgriConnectPlatform.Controllers
{
    public class ProductController : Controller
    {
        private readonly AgriConnectContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(AgriConnectContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search)
        {
            var products = SearchProducts(from a in _context.Products select a, search);
            return View(await products.ToListAsync());
        }
        
        public async Task<IActionResult> Details(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            Console.WriteLine("Creating product " + product.productName);
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid");
                try
                {
                    product.CreatedByUserId = _userManager.GetUserId(User);
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product added successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error adding product: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Invalid product data." });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductName(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return Json(product?.productName);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDescription(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return Json(product?.Description);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return Json((int?)product?.Category);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDateCreated(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return Json(product?.DateCreated);
        }

        private bool productExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Product/UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                Console.WriteLine("Product is null");
                return Json(new { success = false, message = "Invalid product data" });
            }

            Console.WriteLine($"Received product - ID: {product.Id}, Name: {product.productName}, Description: {product.Description}");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(product.Id);
                    if (existingProduct == null)
                    {
                        return Json(new { success = false, message = "Product not found" });
                    }

                    existingProduct.productName = product.productName;
                    existingProduct.Description = product.Description;
                    existingProduct.Category = product.Category;
                    existingProduct.DateCreated = product.DateCreated;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product updated successfully" });
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!productExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Validation errors: " + string.Join(", ", errors));
            return Json(new { success = false, message = "Invalid product data: " + string.Join(", ", errors) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Delete called with id: {id}");
                System.Diagnostics.Debug.WriteLine($"Current route: {Request.Path}");
                System.Diagnostics.Debug.WriteLine($"HTTP Method: {Request.Method}");
                System.Diagnostics.Debug.WriteLine($"Content Type: {Request.ContentType}");

                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid product ID" });
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Delete: {ex.Message}");
                return Json(new { success = false, message = "Error deleting product: " + ex.Message });
            }
        }

        // Test endpoint
        [HttpPost]
        public IActionResult Test()
        {
            return Json(new { success = true, message = "Test POST successful" });
        }

        public IQueryable<Product> SearchProducts(IQueryable<Product> products, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(a => a.Id.ToString().Contains(search)
                    || a.productName.Contains(search)
                    || a.Description.Contains(search));
            }
            return products;
        }
    }
}

