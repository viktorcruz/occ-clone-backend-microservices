using Microsoft.AspNetCore.Mvc;

namespace RolesServices.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
