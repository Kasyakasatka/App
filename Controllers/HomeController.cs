using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserManagementApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home page requested. Checking user authentication status.");
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is authenticated. Redirecting to Admin page.");
                return RedirectToAction("Index", "Admin");
            }

            _logger.LogInformation("User is not authenticated. Redirecting to Login page.");
            return RedirectToAction("Login", "Account");
        }
    }
}