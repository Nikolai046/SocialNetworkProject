// Обработчик добавления друзей в боковую панель
document.addEventListener('DOMContentLoaded',
    async () => {
        try {
            // Получаем UserID из Razor-модели
            const userId = window.userId;

            // Запрашиваем данные с сервера
            const response = await fetch(`/AccountManager/get-friends?UserID=${userId}`);
            const result = await response.json();
            console.log(result.data);
            if (result.data && result.data.length > 0) {
                // Добавляем друзей
                const friendsSection = document.getElementById('friendsSection');
                result.data.forEach(friend => {
                    const friendHtml = `
                           <li>
                            <a href="/user_page?userID=${friend.friendId}">
                            <img src="${friend.image}" />
                            ${friend.friendFullName}
                            </a>
                        </li>`;
                    friendsSection.insertAdjacentHTML('beforeend', friendHtml);
                });
            }
        } catch (error) {
            console.error('Ошибка при загрузке друзей:', error);
        }
    });