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
    /// Метод сохранения комментариев к сообщениям в БД
    /// </summary>
    [HttpDelete("delete-message")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage([FromQuery] int messageId)
    {
        var user = await _userManager.GetUserAsync(User);
        var message = await _unitOfWork.GetRepository<Message>().Get(messageId);
        if (message.Sender.Id == user.Id) Console.WriteLine($"\n\x1b[42m\x1b[37mСообщение можно удалять\x1b[0m");

        return Ok();
    }

}