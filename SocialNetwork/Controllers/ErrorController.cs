using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers;

[Route("[controller]")]
public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public async Task<IActionResult> Error(int statusCode)
    {
        return View("NotFound");
    }

}
