using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
    public string Password { get; set; }
}