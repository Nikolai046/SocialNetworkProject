using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ViewModels.Account;

namespace SocialNetwork.Controllers;

public class RegisterController : Controller
{
    [Route("Register")]
    [HttpGet]
    public IActionResult Register()
    {
        return View("Register");
    }

    [Route("RegisterPart2")]
    [HttpGet]
    public IActionResult RegisterPart2(RegisterViewModel model)
    {
        return View("RegisterPart2", model);
    }

}
