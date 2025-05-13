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

        public async Task<IActionResult> Create(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var farmer = new Farmer { FarmerId = user.Id };
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Farmer/DeleteFarmer")]
        public async Task<IActionResult> DeleteFarmer(string userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteFarmer called with userId: {userId}");
                System.Diagnostics.Debug.WriteLine($"Current route: {Request.Path}");
                System.Diagnostics.Debug.WriteLine($"HTTP Method: {Request.Method}");
                System.Diagnostics.Debug.WriteLine($"Content Type: {Request.ContentType}");

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

                // Then find the farmer record
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.FarmerId == userId);
                System.Diagnostics.Debug.WriteLine($"Found farmer: {farmer != null}");
                
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
                System.Diagnostics.Debug.WriteLine($"Error in DeleteFarmer: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Error deleting farmer: " + ex.Message });
            }
        }
    }
}

