// Обработчик удаления сообщения или комментария
document.querySelectorAll('.messageDel, .commentDel').forEach(button => {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        const isComment = this.classList.contains('commentDel');
        const message = isComment
            ? 'Вы уверены, что хотите удалить комментарий?'
            : 'Вы уверены, что хотите удалить сообщение?';

        showConfirmModal(message, () => {
            // Удаление сообщения или комментария
            if (e.target.closest('.messageDel, .commentDel')) {
                const isMessage = e.target.closest('.messageDel');
                const cardElement = isMessage
                    ? e.target.closest('.card')
                    : e.target.closest('.card-comment');

                const idType = isMessage ? 'message' : 'comment';
                const elementId = cardElement.dataset[`${idType}Id`];
                try {
                    const response = await fetch(`/AccountManager/delete-post?idType=${idType}&postId=${parseInt(elementId)}`,
                        {
                            method: 'DELETE',
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                            }
                        });

                    if (response.ok) {
                        cardElement.remove();
                    } else {
                        console.error('Ошибка удаления:', response.statusText);
                    }
                } catch (error) {
                    console.error('Ошибка:', error);
                }
            }
            // Здесь выполнить логику удаления
            const item = this.closest(isComment ? '.card-comment' : '.card');
            item.remove();
        });
    });
});


