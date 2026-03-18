using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class OrderItemController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}