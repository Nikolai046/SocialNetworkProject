using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DLL.Entities;
using SocialNetwork.ViewModels.Account;

public class AccountManagerController : Controller
{
    private IMapper _mapper;

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
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
        return View("Mypage",autorizedUser);
    }

}