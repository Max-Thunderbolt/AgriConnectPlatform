using Microsoft.AspNetCore.Mvc;
using AgriConnect.Models;
using AgriConnect.Utils;
namespace AgriConnect.Controllers
{
    public class AuthController : Controller
    {
        private readonly Supabase.Client _supabase;

        public AuthController(Supabase.Client supabaseClient)
        {
            _supabase = supabaseClient;
        }

        [HttpPost]
        public async Task<IActionResult> register(string email, string password)
        {
            var response = await _supabase.Auth.SignUp(email, password);
            if (response.User != null)
            {
                return Ok("User registered successfully");
            }
            return BadRequest("Failed to register user");
        }

        [HttpPost]
        public async Task<IActionResult> login(string email, string password)
        {
            var response = await _supabase.Auth.SignInWithPassword(email, password);
            if (response.User != null)
            {
                return Ok("User logged in successfully");
            }
            return BadRequest("Failed to login user");
        }

        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await _supabase.Auth.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}

