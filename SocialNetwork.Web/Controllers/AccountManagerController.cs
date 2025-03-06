using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models.ViewModels;
using SocialNetwork.Core.Models.ViewModels.DTO;
using SocialNetwork.Data.Entities;
using SocialNetwork.Data.UoW;

namespace SocialNetwork.Web.Controllers;

/// <summary>
/// Контроллер для управления аккаунтом пользователя
/// </summary>
[Route("[controller]")]
public class AccountManagerController : Controller
{
    private IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountManagerController> _logger;

    public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper,
        IUnitOfWork unitOfWork, ILogger<AccountManagerController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Главная страница пользователя
    /// </summary>
    [Authorize]
    [HttpGet("my-page")]
    public async Task<IActionResult> MyPage()
    {
        var authorizedUser = await _userManager.GetUserAsync(User);
        var model = _mapper.Map<UserViewModel>(authorizedUser);

        if (authorizedUser != null) return View("Mypage", model);
        return RedirectToAction("Index", "Home");
    }


    /// <summary>
    /// Асинхронный метод для загрузки сообщений пользователя с возможностью пагинации. Если идентификатор пользователя не указан, используется идентификатор текущего авторизованного пользователя.
    /// </summary>
    /// <param name="UserID">Необязательный параметр, идентификатор пользователя, чьи сообщения нужно загрузить. Если не указан, используется идентификатор текущего пользователя.</param>
    /// <param name="page">Номер страницы результатов, начиная с 1, для пагинации сообщений.</param>
    /// <returns>
    /// Возвращает JSON объект с двумя полями: data содержит список сообщений с комментариями и информацией об авторах, hasMore указывает, есть ли еще страницы для загрузки.
    /// В случае ошибки возвращает статус 500 с сообщением о внутренней ошибке сервера.
    /// </returns>
    [Authorize]
    [HttpGet]
    [Route("load-messages")]
    public async Task<IActionResult> LoadMessages([FromQuery] string? UserID, int page)
    {
        try
        {
            int pageSize = 10;

            var authorizedUser = await _userManager.GetUserAsync(User);

            // Проверка UserID
            var targetUserId = UserID ?? authorizedUser?.Id;

            // Загрузка сообщений с проверкой существования отправителя
            var allMessages = await _unitOfWork.GetRepository<Message>()
                .GetAll()
                .Where(m => m.SenderId == targetUserId)
                .Include(m => m.Comments)
                .ThenInclude(c => c.Sender)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            // Постраничная разбивка
            var messages = allMessages
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            // Собираем все уникальные SenderId из сообщений и комментариев
            var senderIds = messages
                .Select(m => m.SenderId)
                .Concat(messages.SelectMany(m => m.Comments).Select(c => c.SenderId))
                .Where(id => id != null)
                .Distinct()
                .ToList();

            // Загружаем только нужных пользователей
            var users = await _userManager.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u);

            // Формируем ViewModel
            var result = messages.Select(m => new MessageViewModel
            {
                MessageId = m.Id,
                Text = m.Text,
                AuthorFullName = users.ContainsKey(m.SenderId!) ? users[m.SenderId!].GetFullName() : "Аноним",
                CreatedAt = m.Timestamp,
                Deletable = (m.SenderId == authorizedUser!.Id),
                Comments = m.Comments.Select(c => new CommentViewModel
                {
                    CommentId = c.Id,
                    Text = c.Text,
                    Author = users.ContainsKey(c.SenderId!) ? users[c.SenderId!].GetFullName() : "Аноним",
                    CreatedAt = c.Timestamp,
                    Deletable = (c.SenderId == authorizedUser.Id),
                }).ToList()
            }).ToList();

            var hasMore = (allMessages.Count / pageSize - page) >= 0;

            return Json(new
            {
                data = result,
                hasMore = hasMore
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке сообщений для UserID: {UserID}, страница: {Page}", UserID, page);
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Добавляет новое сообщение в систему на основе полученных данных.
    /// </summary>
    /// <param name="messageDto">DTO сообщения, содержащее текст сообщения.</param>
    /// <returns>
    /// Возвращает JSON объект с информацией о добавленном сообщении, включая ID, текст, временную метку и автора.
    /// Если текст сообщения не предоставлен, возвращает BadRequest с соответствующим сообщением.
    /// </returns>
    [Authorize]
    [HttpPost("add-message")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMessage([FromBody] MessageDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto?.Text))
            return BadRequest("Текст сообщения обязателен");

        var user = await _userManager.GetUserAsync(User);

        var message = new Message
        {
            Text = messageDto.Text,
            Timestamp = DateTime.Now,
            SenderId = user?.Id
        };

        await _unitOfWork.GetRepository<Message>().CreateAsync(message);

        return Json(new
        {
            id = message.Id,
            text = message.Text,
            timestamp = message.Timestamp.ToString("G"),
            author = user!.GetFullName()
        });
    }


    /// <summary>
    /// Добавляет новый комментарий к сообщению на основе полученных данных.
    /// </summary>
    /// <param name="commentDto">DTO комментария, содержащий текст комментария и идентификатор сообщения.</param>
    /// <returns>
    /// Возвращает JSON объект с текстом комментария, временем создания и полным именем автора.
    /// </returns>
    [Authorize]
    [HttpPost("add-comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment([FromBody] CommentDto commentDto)
    {
        var user = await _userManager.GetUserAsync(User);
        var message = await _unitOfWork.GetRepository<Message>().GetAsync(commentDto.MessageId);

        var comment = new Comment
        {
            Text = commentDto.Text,
            Timestamp = DateTime.Now,
            InitialMessageId = message!.Id,
            SenderId = user?.Id
        };

        await _unitOfWork.GetRepository<Comment>().CreateAsync(comment);

        return Json(new
        {
            text = comment.Text,
            timestamp = comment.Timestamp.ToString("G"),
            author = user?.GetFullName()
        });
    }


    /// <summary>
    /// Асинхронно удаляет сообщение или комментарий, идентифицируемый по типу и идентификатору, если текущий пользователь является отправителем.
    /// </summary>
    /// <param name="idType">Тип удаляемого элемента ("message" для сообщений, "comment" для комментариев).</param>
    /// <param name="postId">Идентификатор удаляемого сообщения или комментария.</param>
    /// <returns>
    /// Возвращает статус 403 (Forbid), если пользователь не имеет прав на удаление, 400 (BadRequest) при неверном типе идентификатора, или 200 (Ok) при успешном удалении.
    /// </returns>
    [Authorize]
    [HttpDelete("delete-post")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost([FromQuery] string idType, int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (idType == "message")
        {
            var message = await _unitOfWork.GetRepository<Message>().GetAsync(postId);
            if (message == null || message.SenderId != user?.Id)
            {
                return Forbid();
            }

            await _unitOfWork.GetRepository<Message>().DeleteAsync(message);
            Console.WriteLine($"\n\x1b[42m\x1b[37mСообщение {postId} удалено\x1b[0m");
        }
        else if (idType == "comment")
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(postId);
            if (comment == null || comment.SenderId != user?.Id)
            {
                return Forbid();
            }

            await _unitOfWork.GetRepository<Comment>().DeleteAsync(comment);
            Console.WriteLine($"\n\x1b[42m\x1b[37mКомментарий {postId} удалty\x1b[0m");
        }
        else
        {
            return BadRequest("Invalid idType");
        }

        return Ok();
    }

    /// <summary>
    /// Контроллер для страницы редактирования/обновления пользовательских данных
    /// </summary>
    [Authorize]
    [HttpGet("update-user-data")]
    public async Task<IActionResult> UpdateUserData()
    {
        var user = await _userManager.GetUserAsync(User);
        var editingUser = _mapper.Map<UpdateViewModel>(user);

        return View(editingUser);
    }

    /// <summary>
    /// Метод для обновления фотографии пользователя
    /// </summary>
    [Authorize]
    [HttpPost("upload-photo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Пользователь не найден.");
        }

        // Проверка размера файла
        if (photo.Length > 1048576) // 1 MB = 1048576 bytes
        {
            ModelState.AddModelError("photo", "Размер файла не должен превышать 1 МБ.");
            return RedirectToAction("UpdateUserData");
        }

        // Проверка формата файла
        if (!photo.ContentType.StartsWith("image/jpeg"))
        {
            ModelState.AddModelError("photo", "Файл должен быть в формате JPG или JPEG.");
            return RedirectToAction("UpdateUserData");
        }

        // Генерация имени файла
        var fileName = user.Id + Path.GetExtension(photo.FileName);

        // Путь к папке для сохранения изображений
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "userphoto", fileName);

        // Сохранение файла
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        // Обновление пути к изображению в модели пользователя
        user.Image = $"/images/userphoto/{fileName}";
        await _userManager.UpdateAsync(user);

        return RedirectToAction("UpdateUserData");
    }

    /// <summary>
    /// Метод для обновления пользовательских данных
    /// </summary>
    [Authorize]
    [HttpPost("update-profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("Пользователь не найден.");
        model.Image = user.Image;

        if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
        {
            ModelState.AddModelError("CurrentPassword", "Текущий пароль неверен.");
            return View("UpdateUserData", model);
        }

        _mapper.Map(model, user);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return RedirectToAction("MyPage");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View("UpdateUserData", model);
    }


    /// <summary>
    /// Инициирует запрос на подписку текущего пользователя на другого пользователя, указанного по userId.
    /// </summary>
    /// <param name="userId">ID пользователя, на которого нужно подписаться.</param>
    /// <returns>
    /// Возвращает IActionResult, указывающий результат операции подписки. Возвращает Ok(), если подписка прошла успешно,
    /// и Unauthorized(), если текущий пользователь не аутентифицирован.
    /// </returns>
    [HttpPost("FollowUser/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FollowUser(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var friend = new Friend
        {
            UserId = currentUser.Id,
            CurrentFriendId = userId
        };

        await _unitOfWork.GetRepository<Friend>().CreateAsync(friend);
        return Ok();
    }

    /// <summary>
    /// Осуществляет процесс отписки текущего пользователя от другого пользователя по идентификатору.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, от которого текущий пользователь хочет отписаться.</param>
    /// <returns>
    /// Возвращает результат выполнения операции: HTTP статус 200 (OK) если отписка прошла успешно, и HTTP статус 401 (Unauthorized), если текущий пользователь не аутентифицирован.
    /// </returns>
    [HttpPost("UnfollowUser/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnfollowUser(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var friendship = await _unitOfWork.GetRepository<Friend>()
            .GetAll()
            .FirstOrDefaultAsync(f =>
                (f.UserId == currentUser.Id && f.CurrentFriendId == userId) ||
                (f.UserId == userId && f.CurrentFriendId == currentUser.Id));

        if (friendship != null)
        {
            await _unitOfWork.GetRepository<Friend>().DeleteAsync(friendship);
        }

        return Ok();
    }

    /// <summary>
    /// Получает список друзей пользователя по его идентификатору.
    /// </summary>
    /// <param name="UserID">Идентификатор пользователя, для которого необходимо получить список друзей.</param>
    /// <returns>
    /// Возвращает объект IActionResult, содержащий JSON с данными о друзьях пользователя.
    /// </returns>
    [HttpGet("get-friends")]
    public async Task<IActionResult> GetFriends([FromQuery] string UserID)
    {
        var friends = await _unitOfWork.GetRepository<Friend>()
            .GetAll()
            .Where(f => f.UserId == UserID || f.CurrentFriendId == UserID)
            .Include(f => f.User)
            .Include(f => f.CurrentFriend)
            .Select(f => f.UserId == UserID
                ? new FriendDto
                {
                    FriendId = f.CurrentFriend!.Id,
                    FriendFullName = f.CurrentFriend.FirstName + " " + f.CurrentFriend.LastName,
                    Image = f.CurrentFriend.Image
                }
                : new FriendDto
                {
                    FriendId = f.User!.Id,
                    FriendFullName = f.User.FirstName + " " + f.User.LastName,
                    Image = f.User.Image
                })
            .Distinct()
            .ToListAsync();

        return Json(new
        {
            data = friends
        });
    }
}