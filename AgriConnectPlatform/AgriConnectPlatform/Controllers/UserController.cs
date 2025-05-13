using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AgriConnectPlatform.Models;
using AgriConnectPlatform.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace AgriConnectPlatform.Controllers
{
    public class UserController : Controller
    {
        private readonly AgriConnectContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private const string ENCRYPTION_KEY = "AgriConnect!@#$%^&*()";

        public UserController(
            AgriConnectContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRoles = new List<UserRole>();

            foreach (var user in users)
            {
                UserRole userRole = new UserRole
                {
                    UserId = user.Id,
                    Email = user.Email,
                    isEmployee = await _userManager.IsInRoleAsync(user, "Employee")
                };
                userWithRoles.Add(userRole);
            }
            return View(userWithRoles);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetUserProducts(string userId)
        {
            var products = await _context.Products
                .Where(p => p.CreatedByUserId == userId)
                .ToListAsync();
            return Json(products);
        }

        [Authorize(Roles = "Farmer")]
        [HttpGet]
        [Route("User/FarmerDashboard")]
       public async Task<IActionResult> FarmerDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var products = await _context.Products
                .Where(p => p.CreatedByUserId == user.Id)
                .ToListAsync();
            var model = new FarmerDashboardViewModel
            {
                FarmerName = user.Email, 
                ProductCount = products.Count,
                Products = products,
                UserId = user.Id
            };

            return View(model);
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> AddProduct()
        {
            return View();
        }

        private bool farmerExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        private class credentialRequest
        {
            public required string email { get; set; }
            public required string password { get; set; }
        }

        private credentialRequest? DecryptData(string encryptedData)
        {
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(encryptedData);
                string decoded = Encoding.UTF8.GetString(decodedBytes);
                
                string decrypted = "";
                for (int i = 0; i < decoded.Length; i++)
                {
                    decrypted += (char)(decoded[i] ^ ENCRYPTION_KEY[i % ENCRYPTION_KEY.Length]);
                }
                
                return JsonSerializer.Deserialize<credentialRequest>(decrypted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return null;
            }
        }

        [HttpPost]
        [Route("User/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EncryptedDataModel model)
        {
            try
            {
                if (model?.EncryptedData == null)
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                Console.WriteLine("Received encrypted data: " + model.EncryptedData);
                
                var loginRequest = DecryptData(model.EncryptedData);
                if (loginRequest == null)
                {
                    return Json(new { success = false, message = "Invalid data format" });
                }

                string email = loginRequest.email;
                string password = loginRequest.password;
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return Json(new { success = false, message = "Email and password are required" });
                }

                Console.WriteLine("Attempting login for email: " + email);
                
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    Console.WriteLine("User not found for email: " + email);
                    return Json(new { success = false, message = "Invalid email or password" });
                }
                
                var passwordValid = await _userManager.CheckPasswordAsync(user, password);
                if(!passwordValid)
                {
                    Console.WriteLine("Invalid password for user: " + email);
                    return Json(new { success = false, message = "Invalid email or password" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var isEmployee = roles.Contains("Employee");
                Console.WriteLine($"User {email} roles: {string.Join(", ", roles)}");

                // Sign in the user
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    Console.WriteLine($"Successfully signed in user: {email}");
                    return Json(new { success = true, message = "Login successful", isEmployee = isEmployee });
                }
                else if (signInResult.IsLockedOut)
                {
                    Console.WriteLine($"Account locked out for user: {email}");
                    return Json(new { success = false, message = "Account is locked out" });
                }
                else
                {
                    Console.WriteLine($"Failed sign in attempt for user: {email}");
                    return Json(new { success = false, message = "Invalid login attempt" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "An error occurred during login: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("User/Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("User/RegisterFarmer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterFarmer([FromBody] EncryptedDataModel model)
        {
            Console.WriteLine("Received encrypted data: " + model.EncryptedData);
            try
            {
                var decryptedData = DecryptData(model.EncryptedData);
                if (decryptedData == null)
                {
                    return Json(new { success = false, message = "Invalid data format" });
                }

                string email = decryptedData.email;
                string password = decryptedData.password;

                if (farmerExists(email))
                {
                    return Json(new { success = false, message = "Farmer already exists" });
                }

                var farmer = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(farmer, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(farmer, "Farmer");
                    return Json(new { success = true, message = "Farmer registered successfully" });
                }
                return Json(new { success = false, message = "Failed to register farmer" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred during registration" });
            }
        }
    }

    public class EncryptedDataModel
    {
        public required string EncryptedData { get; set; }
    }
}