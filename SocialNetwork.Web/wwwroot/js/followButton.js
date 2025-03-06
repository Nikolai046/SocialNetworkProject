// Обработчик добавления/удаления кнопки подписаться *@
async function follow(userId) {
    try {
        const response = await fetch(`/AccountManager/FollowUser/${userId}`,
            {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

        if (response.ok) {
            // Обновляем кнопку после успешной подписки
            const button = document.querySelector(`button[onclick="follow('${userId}')"]`);
            if (button) {
                button.textContent = 'Отписаться';
                button.setAttribute('onclick', `unfollow('${userId}')`);
            }
        } else {
            alert('Ошибка при подписке.');
        }
    } catch (error) {
        console.error('Ошибка:', error);
    }
}

async function unfollow(userId) {
    try {
        const response = await fetch(`/AccountManager/UnfollowUser/${userId}`,
            {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

        if (response.ok) {
            // Обновляем кнопку после успешной отписки
            const button = document.querySelector(`button[onclick="unfollow('${userId}')"]`);
            if (button) {
                button.textContent = 'Подписаться';
                button.setAttribute('onclick', `follow('${userId}')`);
            }
        } else {
            alert('Ошибка при отписке.');
        }
    } catch (error) {
        console.error('Ошибка:', error);
    }
}