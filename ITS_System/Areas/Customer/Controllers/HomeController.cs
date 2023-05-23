using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FlexAppealFitness.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        [Area("Customer")]
        [Authorize(Roles = "Customer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
