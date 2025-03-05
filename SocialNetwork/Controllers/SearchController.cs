using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models.ViewModels.Account;
using SocialNetwork.Models.ViewModels.DTO;

namespace SocialNetwork.Controllers;

[Route("[controller]")]
public class SearchController : Controller
{
    private IMapper _mapper;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<SearchController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Конструктор для создания экземпляра контроллера поиска, инициализирующий необходимые зависимости.
    /// </summary>
    /// <param name="userManager">Менеджер пользователей для операций, связанных с пользователями.</param>
    /// <param name="signInManager">Менеджер входа в систему для управления аутентификацией пользователей.</param>
    /// <param name="mapper">Маппер для преобразования одних типов данных в другие.</param>
    /// <param name="unitOfWork">Единица работы, координирующая работу с репозиториями.</param>
    /// <param name="logger">Логгер для ведения журнала событий или ошибок.</param>
    public SearchController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<SearchController> logger)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Асинхронный метод для поиска пользователей по заданному запросу. Метод доступен только для аутентифицированных пользователей.
    /// </summary>
    /// <param name="query">Строка поиска, которая может содержать имя, фамилию или оба компонента. Специальный символ "*" возвращает всех пользователей, кроме текущего.</param>
    /// <returns>
    /// Возвращает представление с результатами поиска, если запрос не пуст. При пустом запросе перенаправляет на страницу аккаунта пользователя. Если пользователь не аутентифицирован, перенаправляет на главную страницу.
    /// </returns>
    [Authorize]
    [HttpGet("search_results")]
    public async Task<IActionResult> Search(string query)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(query)) return RedirectToAction("MyPage", "AccountManager");

        var currentUser = await _userManager.GetUserAsync(User);
        var model = new UserViewModel(currentUser!);

        // Разделяем запрос на части (на случай если в запросе Имя и Фамилия)
        var queryParts = query?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        // Поиск пользователей по запросу
        var users = (query == "*")
            ? await _userManager.Users
                .Where(u => u != currentUser)
                .ToListAsync()
            : await _userManager.Users
                .Where(u => u != currentUser && queryParts.All(part =>
                    u.FirstName.ToLower().Contains(part.ToLower()) ||
                    u.LastName.ToLower().Contains(part.ToLower())))
                .ToListAsync();

        // Создаем список UserlistDto
        var userlist = new List<UserlistDto>();
        foreach (var user in users)
        {
            // Проверяем, является ли пользователь другом текущего пользователя
            var isFriend = await _unitOfWork.GetRepository<Friend>()
                .GetAll()
                .AnyAsync(f =>
                    (f.UserId == currentUser.Id && f.CurrentFriendId == user.Id) ||
                    (f.CurrentFriendId == currentUser.Id && f.UserId == user.Id));

            // Добавляем пользователя в список
            userlist.Add(new UserlistDto
            {
                user = user,
                IsMyFriend = isFriend
            });
        }

        // Передаем результаты поиска в ViewBag
        ViewBag.Users = userlist;

        return View("Search", model);
    }
}