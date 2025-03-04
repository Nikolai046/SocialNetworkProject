using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models.ViewModels.Account;
using System.Diagnostics;

namespace SocialNetwork.Controllers;

/// <summary>
/// Главный контроллер приложения
/// </summary>
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

    /// <summary>
    /// Асинхронный метод для аутентификации пользователя.
    /// </summary>
    /// <param name="model">Модель данных для входа, содержащая электронную почту, пароль и флаг запоминания пользователя.</param>
    /// <returns>
    /// Перенаправление на страницу пользователя в случае успешной аутентификации или возвращение представления с ошибками в случае неудачи.
    /// </returns>
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                return View(model); // возвращаем представление, чтобы отобразились ошибки
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                return View(model);
            }
        }

        return View(model);
    }

    /// <summary>
    /// Обрабатывает GET-запрос к методу Index. Перенаправляет аутентифицированных пользователей на их персональную страницу,
    /// а неаутентифицированных пользователей возвращает на главную страницу.
    /// </summary>
    /// <returns>
    /// Task<IActionResult>, который выполняет перенаправление на страницу пользователя, если пользователь аутентифицирован,
    /// или возвращает представление главной страницы, если пользователь не аутентифицирован.
    /// </returns>
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

    /// <summary>
    /// Метод действия для обработки ошибок в приложении. Отключает кэширование для данного метода.
    /// </summary>
    /// <returns>
    /// Возвращает представление с моделью ErrorViewModel, содержащей идентификатор запроса.
    /// </returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}