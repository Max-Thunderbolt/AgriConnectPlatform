using AgriConnect.Utils;
using Microsoft.AspNetCore.Mvc;


namespace AgriConnect.Controllers
{
    public class MarketController : Controller
    {
        private readonly AgriConnectContext _context;

        public MarketController(AgriConnectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            if (products == null) return NotFound();
            return View(products);
        }
    }
}