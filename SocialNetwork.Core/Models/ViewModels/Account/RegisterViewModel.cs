using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.Account;

/// <summary>
/// Класс модели представления для регистрации пользователя.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
    [Display(Name = "Email")]
    public string? EmailReg { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Год")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "День")]
    public int Date { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Месяц")]
    public int Month { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
    public string PasswordReg { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Никнейм")]
    public string Login { get; set; }
}