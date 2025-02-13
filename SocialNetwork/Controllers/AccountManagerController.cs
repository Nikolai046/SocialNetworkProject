using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models.ViewModels.Account;


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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [Route("my_page")]
    [HttpGet]
    public async Task<IActionResult> MyPage()
    {
        var result = await _userManager.GetUserAsync(User);

        var autorizedUser = new UserViewModel(result);
        //model.Friends = await GetAllFriend(model.User);

        //return View("User", model);
        return View("Mypage", autorizedUser);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMessage([FromBody] MessageDto messageDto)
    {
        _logger.LogInformation($"Received text: {messageDto?.Text}");

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
            timestamp = message.Timestamp.ToString("g"),
            author = user.GetFullName()
        });
    }

    [HttpPost]
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
            timestamp = comment.Timestamp.ToString("g"),
            author = user.GetFullName()
        });
    }
}