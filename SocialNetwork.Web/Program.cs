using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Data.Entities;
using SocialNetwork.Data.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Data.Seeders;
using SocialNetwork.Data.UoW;
using SocialNetwork.Web.Infrastructure.Mapping;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetAssembly(typeof(MappingProfile));

// Получаем ILoggerFactory из контейнера внедрения зависимостей
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

// Подключение контроллеров и представлений
builder.Services.AddControllersWithViews();

// Настройка DbContext с использованием ILoggerFactory
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLoggerFactory(loggerFactory)) // Передаем уже созданную фабрику логгера
    .AddCustomRepository<Message, MessagesRepository>()
    .AddCustomRepository<Comment, CommentsRepository>()
    .AddCustomRepository<Friend, FriendsRepository>()
    .AddCustomRepository<ServiceData, ServiceDataRepository>()
    .AddTransient<IUnitOfWork, UnitOfWork>();

// Настройка Identity с усиленными требованиями к паролям
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 5;   // Минимальная длина пароля
    opts.Password.RequireNonAlphanumeric = false;   // Не требовать не алфавитно-цифровых символов
    opts.Password.RequireLowercase = false; // Не требовать строчных букв
    opts.Password.RequireUppercase = false; // Не требовать заглавных букв
    opts.Password.RequireDigit = false; // Не требовать цифр
    opts.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" +
        "абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
        "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ" +
        "-_.";
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Подключение AutoMapper
builder.Services.AddAutoMapper(assembly);

// Настройка маршрутизации (URL в нижнем регистре)
builder.Services.AddRouting(options => options.LowercaseUrls = true);


//// Добавляем сервис IHttpContextAccessor в контейнер внедрения зависимостей
//builder.Services.AddHttpContextAccessor();

// Регистрация генератора тестовых данных
builder.Services.AddScoped<TestDataGenerator>();

var app = builder.Build();

// Условная настройка в зависимости от окружения
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    // Запретить кэширование статических файлов в режиме разработки
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "0");
        }
    });
}
else
{
    // В продакшене разрешаем кэширование статических файлов
    app.UseStaticFiles();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Обработка ошибок с перенаправлением на страницу ошибки
app.UseStatusCodePagesWithReExecute("/Error/Error/{0}");

app.UseAuthentication();
app.UseAuthorization();

// Применение миграций и генерация тестовых данных
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    if (app.Environment.IsDevelopment())
    {
        var dataGen = scope.ServiceProvider.GetRequiredService<TestDataGenerator>();
        await dataGen.Generate(30);
    }
}

// Настройка маршрута по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Перенаправление с корневого URL на /Home/Index
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Home/Index");
        return;
    }
    await next();
});

app.Run();