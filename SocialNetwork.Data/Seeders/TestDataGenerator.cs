using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data.Entities;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Data.UoW;

namespace SocialNetwork.Data.Seeders;

/// <summary>
/// Генерирует тестовые данные для пользователей, включая профили, друзей, сообщения и комментарии.
/// </summary>
/// <param name="userCount">Количество пользователей для генерации.</param>
/// <returns>
/// Задача, представляющая асинхронную операцию генерации данных, не возвращает значений.
/// </returns>
public class TestDataGenerator
{
    private readonly IRepository<ServiceData> _serviceDataRepo;
    private readonly UserManager<User> _userManager;
    private IUnitOfWork _unitOfWork;

    private string[] _firstNames;
    private string[] _lastNames;
    private string[] _statuses;
    private string[] _abouts;
    private string[] _messages;
    private string[] _comments;

    public TestDataGenerator(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _serviceDataRepo = unitOfWork.GetRepository<ServiceData>();
        _userManager = userManager;
    }

    public async Task Generate(int userCount)
    {

        var isDatabaseEmpty = !await _userManager.Users.AnyAsync();
        if (isDatabaseEmpty)
            return;

        var testDataFlag = await _serviceDataRepo
            .GetAll()
            .FirstOrDefaultAsync(sd => sd.Key == "IsTestDataGenerated");

        if (testDataFlag?.Data == "true")
            return;

        var random = new Random();

        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SocialNetwork.Data");
        var seedTextPath = Path.Combine(basePath, "seedData", "textData");
        var seedImagePath = Path.Combine(basePath, "seedData", "Avatars");

        // Чтение данных из файлов
        _firstNames = File.ReadAllLines(Path.Combine(seedTextPath, "firstNames.txt"));
        _lastNames = File.ReadAllLines(Path.Combine(seedTextPath, "secondNames.txt"));
        _statuses = File.ReadAllLines(Path.Combine(seedTextPath, "status.txt"));
        _abouts = File.ReadAllLines(Path.Combine(seedTextPath, "about.txt"));
        _messages = File.ReadAllLines(Path.Combine(seedTextPath, "messages.txt"));
        _comments = File.ReadAllLines(Path.Combine(seedTextPath, "comments.txt"));

        // Генерация пользователей
        for (var i = 1; i <= userCount; i++)
        {
            // Генерация имени фото для сохранения в базу
            var sourcePhotoPath = Path.Combine(seedImagePath, $"{i % 30 + 1:D2}.jpeg");

            var user = new User
            {
                FirstName = _firstNames[random.Next(_firstNames.Length)],
                LastName = _lastNames[random.Next(_lastNames.Length)],
                UserName = $"user_{i:D2}",
                Email = $"test_{i:D2}@gmail.com",
                BirthDate = RandomDate(new DateTime(1970, 1, 1), new DateTime(2000, 12, 31)),
                Status = _statuses[random.Next(_statuses.Length)],
                About = _abouts[random.Next(_abouts.Length)]
            };
            await _userManager.CreateAsync(user, "123456");
            var userFromDb = await _userManager.FindByEmailAsync(user.Email);
            var destinationPhotoName = userFromDb.Id + ".jpeg";
            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "userphoto", destinationPhotoName);

            try
            {
                // Асинхронное копирование файла
                var imageBytes = await File.ReadAllBytesAsync(sourcePhotoPath);
                await File.WriteAllBytesAsync(destinationPath, imageBytes);

                // Обновляем путь к изображению
                userFromDb.Image = $"/images/userphoto/{destinationPhotoName}";
            }
            catch (Exception ex)
            {
                user.Image = "/images/person-unknown.svg";
            }
            await _userManager.UpdateAsync(userFromDb);
        }

        // Получаем всех пользователей из базы
        var allUsers = await _userManager.Users.ToListAsync();

        // Добавление друзей
        foreach (var user in allUsers)
        {
            var friendsCount = random.Next(5, 11);
            var friends = allUsers
                .Where(u => u.Id != user.Id)
                .OrderBy(x => random.Next())
                .Take(friendsCount)
                .ToList();

            foreach (var friend in friends)
            {
                await _unitOfWork.GetRepository<Friend>().CreateAsync(new Friend
                {
                    UserId = user.Id,
                    CurrentFriendId = friend.Id
                });
            }
        }

        // Создание сообщений
        foreach (var user in allUsers)
        {
            int messagesCount = random.Next(7, 21);
            for (int i = 0; i < messagesCount; i++)
            {
                await _unitOfWork.GetRepository<Message>().CreateAsync(new Message
                {
                    Text = _messages[random.Next(_messages.Length)],
                    Timestamp = DateTime.Now.AddHours(-i),
                    SenderId = user.Id
                });
            }
        }

        // Создание комментариев
        foreach (var user in allUsers)
        {
            var randomUsers = allUsers
                .Where(u => u.Id != user.Id) // Исключаем текущего пользователя
                .OrderBy(x => random.Next()) // Перемешиваем пользователей
                .Take(10) // Берем 10 случайных пользователей
                .ToList();

            foreach (var targetUser in randomUsers)
            {
                // Получаем все сообщения целевого пользователя
                var messages = await _unitOfWork.GetRepository<Message>()
                    .GetAll()
                    .Where(m => m.SenderId == targetUser.Id)
                    .ToListAsync();

                if (messages.Any()) // Проверяем, есть ли сообщения у пользователя
                {
                    // Выбираем случайное сообщение
                    var randomMessage = messages[random.Next(messages.Count)];

                    // Создаем комментарий к выбранному сообщению
                    await _unitOfWork.GetRepository<Comment>().CreateAsync(new Comment
                    {
                        Text = _comments[random.Next(_comments.Length)], // Случайный текст комментария
                        Timestamp = DateTime.Now, // Текущее время
                        InitialMessageId = randomMessage.Id, // ID выбранного сообщения
                        SenderId = user.Id // ID отправителя комментария
                    });
                }
            }
        }

        // Сохраняем флаг
        if (testDataFlag == null)
        {
            await _serviceDataRepo.CreateAsync(new ServiceData
            {
                Key = "IsTestDataGenerated",
                Data = "true"
            });
        }
        else
        {
            testDataFlag.Data = "true";
            await _serviceDataRepo.UpdateAsync(testDataFlag);
        }
    }

    private DateTime RandomDate(DateTime from, DateTime to)
    {
        var random = new Random();
        var range = to - from;
        return from.AddDays(random.Next(range.Days));
    }
}