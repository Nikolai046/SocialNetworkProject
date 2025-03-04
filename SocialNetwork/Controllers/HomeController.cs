using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models.ViewModels.Account;
using System.Diagnostics;

namespace SocialNetwork.Controllers;

/// <summary>
/// ������� ���������� ����������
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
    /// ����������� ����� ��� �������������� ������������.
    /// </summary>
    /// <param name="model">������ ������ ��� �����, ���������� ����������� �����, ������ � ���� ����������� ������������.</param>
    /// <returns>
    /// ��������������� �� �������� ������������ � ������ �������� �������������� ��� ����������� ������������� � �������� � ������ �������.
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
                ModelState.AddModelError("", "������������ ����� � (���) ������");
                return View(model); // ���������� �������������, ����� ������������ ������
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                ModelState.AddModelError("", "������������ ����� � (���) ������");
                return View(model);
            }
        }

        return View(model);
    }

    /// <summary>
    /// ������������ GET-������ � ������ Index. �������������� ������������������� ������������� �� �� ������������ ��������,
    /// � ��������������������� ������������� ���������� �� ������� ��������.
    /// </summary>
    /// <returns>
    /// Task<IActionResult>, ������� ��������� ��������������� �� �������� ������������, ���� ������������ ����������������,
    /// ��� ���������� ������������� ������� ��������, ���� ������������ �� ����������������.
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
    /// ����� �������� ��� ��������� ������ � ����������. ��������� ����������� ��� ������� ������.
    /// </summary>
    /// <returns>
    /// ���������� ������������� � ������� ErrorViewModel, ���������� ������������� �������.
    /// </returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}