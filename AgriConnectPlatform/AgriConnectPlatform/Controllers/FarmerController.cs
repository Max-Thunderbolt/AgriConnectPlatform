using AgriConnectPlatform.Data;
using Microsoft.AspNetCore.Mvc;
using AgriConnectPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPlatform.Controllers
{
    public class FarmerController : Controller
    {
        private readonly AgriConnectContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FarmerController(AgriConnectContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var farmers = _context.Farmers.ToList();
            return View(farmers);
        }   

        [HttpGet]
        [Route("Farmer/GetFarmerDetails")]
        public async Task<IActionResult> GetFarmerDetails(string userId)
        {
            try
            {
                Console.WriteLine($"Getting farmer details for userId: {userId}");
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.FarmerId == userId);
                
                Console.WriteLine($"Farmer found: {farmer != null}");
                if (farmer == null)
                {
                    return Json(new { success = false, message = "Farmer not found" });
                }

                Console.WriteLine($"Farm Name: {farmer.FarmName}, Location: {farmer.Location}");
                return Json(new { 
                    success = true, 
                    farmName = farmer.FarmName,
                    location = farmer.Location
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFarmerDetails: {ex.Message}");
                return Json(new { success = false, message = "Error getting farmer details: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Farmer/Create")]
        public async Task<IActionResult> Create(string userId)
        {
            try 
            {
                Console.WriteLine($"Create method called with userId: {userId}");
                var user = await _userManager.FindByIdAsync(userId);
                Console.WriteLine($"User found: {user != null}");
                if (user == null)
                {
                return NotFound($"User with ID {userId} not found.");
            }

            var farmer = new Farmer { FarmerId = user.Id };
            Console.WriteLine($"Farmer created: {farmer != null}");
            Console.WriteLine($"Farmer ID: {farmer.FarmerId}");
            Console.WriteLine($"Farmer: {farmer}");
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Farmer saved: {farmer.FarmerId}");

                return Json(new { success = true, userId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create: {ex.Message}");
                return Json(new { success = false, message = "Error creating farmer: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Farmer/DeleteFarmer")]
        public async Task<IActionResult> DeleteFarmer(string userId)
        {
            try
            {
                Console.WriteLine($"DeleteFarmer called with userId: {userId}");
                Console.WriteLine($"Current route: {Request.Path}");
                Console.WriteLine($"HTTP Method: {Request.Method}"); 
                Console.WriteLine($"Content Type: {Request.ContentType}");

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Invalid user ID" });
                }

                // First find the user to verify they exist
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                Console.WriteLine($"User found: {user != null}");
                // Then find the farmer record
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.FarmerId == userId);
                Console.WriteLine($"Found farmer: {farmer != null}");
                
                if (farmer == null)
                {
                    return Json(new { success = false, message = "Farmer record not found" });
                }

                _context.Farmers.Remove(farmer);
                await _context.SaveChangesAsync();

                // Delete the user from the database
                await _userManager.DeleteAsync(user);

                return Json(new { success = true, message = "Farmer deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteFarmer: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Error deleting farmer: " + ex.Message });
            }
        }
    }
}

