using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data.Entities;

namespace SocialNetwork.Data;

/// <summary>
/// Класс контекста данных, наследующийся от IdentityDbContext для работы с пользователями.
/// Предоставляет API для доступа к базе данных, где хранятся данные о пользователях и их ролях.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User>
{
    /// <summary>
    /// Получает или задает набор служебнвх данных в контексте базы данных.
    /// </summary>
    public DbSet<ServiceData> ServiceData { get; set; }

    /// <summary>
    /// Получает или задает набор сообщений в контексте базы данных.
    /// </summary>
    public DbSet<Message> Messages { get; set; }

    /// <summary>
    /// Получает или задает набор комментариев в контексте базы данных.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// Получает или задает набор друзей в контексте базы данных.
    /// </summary>
    public DbSet<Friend> Friends { get; set; }

    /// <summary>
    /// Конструктор для контекста базы данных, инициализирующий новый экземпляр ApplicationDbContext с указанными параметрами.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста базы данных.</param>
    /// <returns>
    /// Не возвращает значение.
    /// </returns>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }

    /// <summary>
    /// Настраивает модели Entity Framework, применяя конфигурации из сборки, в которой определен контекст базы данных.
    /// </summary>
    /// <param name="modelBuilder">Построитель моделей, используемый для конфигурирования сущностей в контексте базы данных.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}