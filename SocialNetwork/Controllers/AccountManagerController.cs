using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models.ViewModels.Account;

[Route("[controller]")]
public class AccountManagerController : Controller
{
    private IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountManagerController> _logger;

    public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<AccountManagerController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet("my-page")]
    public async Task<IActionResult> MyPage()
    {
        var authorizedUser = await _userManager.GetUserAsync(User);
        return View("Mypage", new UserViewModel(authorizedUser));
    }

    [Authorize]
    [HttpGet]
    [Route("load-messages")]
    public async Task<IActionResult> LoadMessages([FromQuery] string? UserID, int page)
    {
        int pageSize = 10;

        var authorizedUser = await _userManager.GetUserAsync(User);

        var allmessages = await _unitOfWork.GetRepository<Message>()
            .GetAll()
            .Where(m => m.SenderId == UserID)
            .Include(m => m.Comments)
            .ThenInclude(c => c.Sender)
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();

        var messages = allmessages
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var result = await Task.WhenAll(messages.Select(async m => new MessageViewModel
        {
            MessageId = m.Id,
            Text = m.Text,
            AuthorFullName = (await _userManager.FindByIdAsync(m.SenderId)).GetFullName(),
            CreatedAt = m.Timestamp,
            Deletable = (m.SenderId == authorizedUser.Id),
            Comments = m.Comments.Select(c => new CommentViewModel
            {
                CommentId = c.Id,
                Text = c.Text,
                Author = c.Sender?.GetFullName() ?? "Аноним",
                CreatedAt = c.Timestamp,
                Deletable = (c.SenderId == authorizedUser.Id),
            }).ToList()
        }));

        var hasMore = (allmessages.Count / pageSize - page) >= 0;
        //_logger.LogInformation("\nLoadMessages called with page {Page} of {2}", page, allmessages.Count / pageSize);
        //_logger.LogInformation($"{hasMore}\n");

        return Json(new
        {
            data = result,
            hasMore = hasMore
        });
    }

    /// <summary>
    /// Метод сохранения сообщений в БД
    /// </summary>
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
            SenderId = user.Id
        };

        await _unitOfWork.GetRepository<Message>().Create(message);

        return Json(new
        {
            id = message.Id,
            text = message.Text,
            timestamp = message.Timestamp.ToString("G"),
            author = user.GetFullName()
        });
    }

    /// <summary>
    /// Метод сохранения комментариев к сообщениям в БД
    /// </summary>
    [Authorize]
    [HttpPost("add-comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment([FromBody] CommentDto commentDto)
    {
        var user = await _userManager.GetUserAsync(User);
        var message = await _unitOfWork.GetRepository<Message>().Get(commentDto.MessageId);

        var comment = new Comment
        {
            Text = commentDto.Text,
            Timestamp = DateTime.Now,
            InitialMessageId = message.Id,
            SenderId = user.Id
        };

        await _unitOfWork.GetRepository<Comment>().Create(comment);

        return Json(new
        {
            text = comment.Text,
            timestamp = comment.Timestamp.ToString("G"),
            author = user.GetFullName()
        });
    }

    /// <summary>
    /// Метод удаления сообщений b комментариев в БД
    /// </summary>
    [Authorize]
    [HttpDelete("delete-post")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost([FromQuery] string idType, int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (idType == "message")
        {
            var message = await _unitOfWork.GetRepository<Message>().Get(postId);
            if (message == null || message.SenderId != user.Id)
            {
                return Forbid();
            }
            await _unitOfWork.GetRepository<Message>().Delete(message);
            Console.WriteLine($"\n\x1b[42m\x1b[37mСообщение {postId} удалено\x1b[0m");
        }
        else if (idType == "comment")
        {
            var comment = await _unitOfWork.GetRepository<Comment>().Get(postId);
            if (comment == null || comment.SenderId != user.Id)
            {
                return Forbid();
            }
            await _unitOfWork.GetRepository<Comment>().Delete(comment);
            Console.WriteLine($"\n\x1b[42m\x1b[37mКомментарий {postId} удалty\x1b[0m");
        }
        else
        {
            return BadRequest("Invalid idType");
        }

        return Ok();
    }

    /// <summary>
    /// Контроллер для страницы редактирования пользовательских данных
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

        await _unitOfWork.GetRepository<Friend>().Create(friend);
        return Ok();
    }

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
            .FirstOrDefaultAsync(f => f.UserId == currentUser.Id && f.CurrentFriendId == userId);

        if (friendship != null)
        {
            await _unitOfWork.GetRepository<Friend>().Delete(friendship);
        }

        return Ok();
    }

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
                FriendId = f.CurrentFriend.Id,
                FriendFullName = f.CurrentFriend.FirstName + " " + f.CurrentFriend.LastName,
                Image = f.CurrentFriend.Image
            }
            : new FriendDto
            {
                FriendId = f.User.Id,
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