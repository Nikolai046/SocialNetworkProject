using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models.ViewModels.Account;

namespace SocialNetwork.Controllers;

/// <summary>
/// Контроллер для отображения страницы просматриваемого (не зарегистрированного) пользователя.
/// </summary>
public class UserController : Controller
{
    private IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Конструктор для создания экземпляра UserController, инициализирующий необходимые зависимости.
    /// </summary>
    /// <param name="userManager">Менеджер для управления пользователями.</param>
    /// <param name="signInManager">Менеджер для управления процессом входа в систему.</param>
    /// <param name="mapper">Инструмент для маппинга данных между объектами.</param>
    /// <param name="unitOfWork">Единица работы для управления транзакциями и репозиториями.</param>
    /// <param name="logger">Логгер для ведения журнала событий.</param>
    /// <returns>
    /// Не возвращает значений.
    /// </returns>
    public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<UserController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Получает страницу пользователя на основе предоставленного идентификатора userID.
    /// </summary>
    /// <param name="userID">Идентификатор пользователя, страницу которого необходимо отобразить. Если параметр пуст, перенаправляет на страницу текущего пользователя.</param>
    /// <returns>
    /// Возвращает представление страницы пользователя, если пользователь найден. В случае отсутствия аутентификации перенаправляет на главную страницу. Если пользователь не найден, перенаправляет на страницу ошибки.
    /// </returns>
    [Authorize]
    [HttpGet("user_page")]
    public async Task<IActionResult> UserPage(string userID)
    {
        // Проверка аутентификации
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        // Если ID пользователя пустой, перенаправляем на страницу пользователя
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("MyPage", "AccountManager");
        }

        // Получаем текущего пользователя
        var currentUser = await _userManager.GetUserAsync(User);

        // Ищем целевого пользователя по ID
        var targetUser = await _userManager.FindByIdAsync(userID);

        // Если пользователь не найден, перенаправляем на страницу ошибки или домой
        if (targetUser == null)
        {
            return RedirectToAction("Error", "Home");
        }

        // Передаем данные в представление
        ViewBag.targetUser = targetUser; // Передаем найденного пользователя

        // Создаем модель для текущего пользователя
        var model = new UserViewModel(currentUser);

        // Возвращаем представление
        return View("UserPage", model);
    }
}