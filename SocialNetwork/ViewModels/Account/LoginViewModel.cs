using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
    [Display(Name = "Email пользователя")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; } = false;

    public string ReturnUrl { get; set; } = string.Empty;
}