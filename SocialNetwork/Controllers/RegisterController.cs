using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.Models.ViewModels.Account;

namespace SocialNetwork.Controllers;

[Route("[controller]")]
public class RegisterController : Controller
{
    private IMapper _mapper;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    /// <summary>
    /// Конструктор для создания экземпляра RegisterController с зависимостями.
    /// </summary>
    /// <param name="mapper">Сервис для маппинга объектов.</param>
    /// <param name="userManager">Сервис для управления пользователями.</param>
    /// <param name="signInManager">Сервис для управления входом в систему.</param>
    public RegisterController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Обрабатывает HTTP GET запрос для отображения страницы регистрации.
    /// </summary>
    /// <returns>
    /// Возвращает представление страницы регистрации.
    /// </returns>
    [Route("register")]
    [HttpGet]
    public Task<IActionResult> Register()
    {
        return Task.FromResult<IActionResult>(View("Register"));
    }

    /// <summary>
    /// Обрабатывает HTTP GET запрос для отображения второй части формы регистрации.
    /// </summary>
    /// <param name="model">Модель представления с данными, введенными пользователем в первой части формы регистрации.</param>
    /// <returns>
    /// Возвращает представление "RegisterPart2" для ввода дополнительных данных пользователем.
    /// </returns>
    [Route("register2")]
    [HttpGet]
    public Task<IActionResult> RegisterPart2(RegisterViewModel model)
    {
        return Task.FromResult<IActionResult>(View("RegisterPart2"));
    }

    /// <summary>
    /// Асинхронный метод для регистрации пользователя в системе на втором этапе регистрации.
    /// </summary>
    /// <param name="model">Модель данных, содержащая информацию для регистрации пользователя.</param>
    /// <returns>
    /// Перенаправление на страницу пользователя в случае успешной регистрации или возвращение текущей страницы с ошибками в случае неудачи.
    /// </returns>
    [Route("register2")]
    [HttpPost]
    [ActionName("RegisterPart2")]
    public async Task<IActionResult> RegisterPart2Post(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.PasswordReg);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return View("RegisterPart2", model);
    }
}