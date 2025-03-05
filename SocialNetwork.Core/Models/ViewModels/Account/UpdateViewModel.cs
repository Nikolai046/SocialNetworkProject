using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models.ViewModels.Account;

/// <summary>
/// Класс UpdateViewModel представляет модель данных для обновления информации о пользователе.
/// Содержит свойства для имени, фамилии, никнейма, email, даты рождения, изображения,
/// статуса, информации "Обо мне" и текущего пароля.
/// Все обязательные поля имеют атрибуты валидации для проверки корректности ввода.
/// </summary>
public class UpdateViewModel
{
    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Никнейм")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Год")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "День")]
    public int Date { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    [Display(Name = "Месяц")]
    public int Month { get; set; }

    public string Image { get; set; }

    [Display(Name = "Статус")]
    public string? Status { get; set; }

    [Display(Name = "Обо мне")]
    public string? About { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Введите текущий пароль")]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; }


    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}