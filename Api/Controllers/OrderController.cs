using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class OrderController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}