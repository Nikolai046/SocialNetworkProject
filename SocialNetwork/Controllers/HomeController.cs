using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models.ViewModels.Account;


namespace SocialNetwork.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;


    public HomeController(ILogger<HomeController> logger, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }



    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("", "Ќеправильный логин и (или) пароль");
                return View(model); // возвращаем представление, чтобы отобразились ошибки
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                ModelState.AddModelError("", "Ќеправильный логин и (или) пароль");
                return View(model);
            }
        }

        return View(model);
    }


    [HttpGet("Index")]
    public Task<IActionResult> Index()
    {
        if (_signInManager.IsSignedIn(User))
        {

            return Task.FromResult<IActionResult>(RedirectToAction("MyPage", "AccountManager"));
        }
        else
        {
            return Task.FromResult<IActionResult>(View());
        }
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}