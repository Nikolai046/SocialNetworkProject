using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNet.Data.UoW;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models.ViewModels.Account;

namespace SocialNetwork.Controllers;

[Route("[controller]")]
public class SearchController : Controller
{
    private IMapper _mapper;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<SearchController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public SearchController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<SearchController> logger)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

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
        var model = new UserViewModel(currentUser);

        // Разделяем запрос на части
        var queryParts = query?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        // Поиск пользователей по запросу
        var users = (query == "*")
            ? await _userManager.Users.ToListAsync()
            : await _userManager.Users
                .Where(u => queryParts.All(part =>
                    u.FirstName.ToLower().Contains(part.ToLower()) ||
                    u.LastName.ToLower().Contains(part.ToLower())))
                .ToListAsync();

        ViewBag.Users = users; // Передаем результаты поиска в представление
        return View("Search", model);
    }
}