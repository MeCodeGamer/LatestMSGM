using Microsoft.AspNetCore.Mvc;

namespace MSGM.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Shop()
        {
            return View();
        }
    }
}
