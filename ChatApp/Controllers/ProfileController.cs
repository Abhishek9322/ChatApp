using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult profile()
        {
            var username = User.Identity?.Name;
            return View(model:username);
        }
    }
}
