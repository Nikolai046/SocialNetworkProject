using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models.ViewModels.Account;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SocialNetwork.Controllers
{
    public class UserController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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
}
