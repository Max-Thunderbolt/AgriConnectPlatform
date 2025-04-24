using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgriConnect.Controllers
{
    public class AuthRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class AuthController : Controller
    {
        private readonly Supabase.Client _supabase;
        private readonly ILogger<AuthController> _logger;

        public AuthController(Supabase.Client supabaseClient, ILogger<AuthController> logger)
        {
            _supabase = supabaseClient;
            _logger = logger;
        }

        [HttpGet]
        [Route("auth/check")]
        public async Task<IActionResult> CheckAuth()
        {
            try
            {
                var session = _supabase.Auth.CurrentSession;
                if (session?.User != null)
                {
                    return Ok(new { 
                        isAuthenticated = true,
                        user = new {
                            email = session.User.Email,
                            id = session.User.Id
                        }
                    });
                }
                return Ok(new { isAuthenticated = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking authentication state");
                return StatusCode(500, new { message = "Error checking authentication state" });
            }
        }

        [HttpPost]
        [Route("auth/register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.password))
                {
                    _logger.LogWarning("Invalid registration request: {Request}", JsonSerializer.Serialize(request));
                    return BadRequest("Email and password are required");
                }

                _logger.LogInformation("Attempting to register user with email: {Email}", request.email);
                var response = await _supabase.Auth.SignUp(request.email, request.password);
                
                if (response.User != null)
                {
                    _logger.LogInformation("User registered successfully: {Email}", request.email);
                    return Ok(new { message = "User registered successfully" });
                }
                
                _logger.LogWarning("Registration failed for email: {Email}", request.email);
                return BadRequest(new { message = "Failed to register user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request?.email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.password))
                {
                    _logger.LogWarning("Invalid login request: {Request}", JsonSerializer.Serialize(request));
                    return BadRequest("Email and password are required");
                }

                _logger.LogInformation("Attempting to log user with email: {Email}", request.email);
                var response = await _supabase.Auth.SignInWithPassword(request.email, request.password);
                
                if (response.User != null)  
                {
                    _logger.LogInformation("User logged in successfully: {Email}", request.email);
                    return Ok(new { message = "User logged in successfully" });
                }
                
                _logger.LogWarning("Login failed for email: {Email}", request.email);
                return BadRequest(new { message = "Failed to login user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request?.email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost]
        [Route("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _supabase.Auth.SignOut();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "Error during logout" });
            }
        }
    }
}

