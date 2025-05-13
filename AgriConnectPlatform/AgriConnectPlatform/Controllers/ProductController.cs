using Microsoft.AspNetCore.Mvc;
using AgriConnectPlatform.Models;
using AgriConnectPlatform.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            if (ModelState.IsValid)
            {
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
            return Json(new { success = false, message = "Invalid product data" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
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

        
        private bool productExists (int id)
        {
            if (id == null)
            {
                return false;
            }
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,productName,Description")] Product product)
        {
            if(id != product.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            return View(product);
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

