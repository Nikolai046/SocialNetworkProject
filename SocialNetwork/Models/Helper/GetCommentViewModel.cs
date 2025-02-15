using Microsoft.EntityFrameworkCore;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.UoW;
using SocialNetwork.DLL.Repositories;
using Microsoft.AspNetCore.Identity;
using SocialNetwork.Models.ViewModels;

namespace SocialNetwork.Models.Helper;

public class GetCommentViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    private User _user;
    private UserManager<User> _userManager;
    public GetCommentViewModel(IUnitOfWork unitOfWork, UserManager<User> userManager, User user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
        _userManager = userManager;

    }

    public async Task<List<MessageViewModel>> GetMessagesWithComments()
    {
        var result = new List<MessageViewModel>(); // Заменяем null на новый список

        var messages = await _unitOfWork.GetRepository<Message>()
            .GetAll()
            .Where(x => x.SenderId == _user.Id)
            .Include(m => m.Comments) // Добавляем жадную загрузку комментариев
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();

        foreach (var message in messages)
        {
            var commentViewModels = new List<CommentViewModel>();
            foreach (var comment in message.Comments)
            {
                var author = await _userManager.FindByIdAsync(comment.SenderId);
                commentViewModels.Add(new CommentViewModel
                {
                    Text = comment.Text,
                    Author = author?.GetFullName() ?? "Удалённый пользователь",
                    CreatedAt = comment.Timestamp
                });
            }

            result.Add(new MessageViewModel // Используем Add вместо Append
            {
                Text = message.Text,
                AuthorFullName = _user.GetFullName(),
                CreatedAt = message.Timestamp,
                MessageId = message.Id,
                Comments = commentViewModels
            });
        }

        return result;
    }
}