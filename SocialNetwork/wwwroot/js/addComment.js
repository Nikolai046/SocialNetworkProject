// Обработчик комментариев
document.addEventListener('click',
    async function (e) {
        // Добавление поля для комментария
        if (e.target.classList.contains('addComment')) {
            const closeUrl = window.closeUrl; //прием адреса иконки закрыть из представления
            const commentInput = `
                <div class="comment-input-container">
                    <textarea class="form-control comment-area" rows="2" placeholder="Введите комментарий..."></textarea>
                    <span class="close-icon"><img src="${closeUrl}" alt="Close"></span>
                 </div>
                <button class="btn-primary submitComment">Отправить</button>`;
            const commentSection = e.target.previousElementSibling;
            commentSection.insertAdjacentHTML('beforeend', commentInput);
            e.target.remove();
        }

        // Обработчик закрытия textarea комментария при нажатии на крестик
        if (e.target.closest('.close-icon')) {
            const commentInputContainer = e.target.closest('.comment-input-container');
            const newButtonSection = commentInputContainer.parentElement;
            const commentButton = '<button class="btn-primary addComment">Добавить комментарий</button>';
            commentInputContainer.nextElementSibling.remove(); // Удаляем существующую кнопку
            commentInputContainer.remove(); // Удаляем textarea
            newButtonSection.insertAdjacentHTML('afterend', commentButton); // вставляем кнопку "Добавить комментарий"
        }

        // Отправка комментария
        if (e.target.classList.contains('submitComment')) {
            const textarea = e.target.previousElementSibling.getElementsByClassName('comment-area')[0];

            console.log("textarea");
            console.log(textarea);
            console.log(textarea.closest('.comment-input-container'));

            const text = textarea.value.trim();
            const messageId = e.target.closest('.card').dataset.messageId;

            if (!text) return;

            try {
                const response = await fetch('/AccountManager/add-comment',
                    {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({
                            MessageId: parseInt(messageId),
                            Text: text
                        })
                    });

                const commentData = await response.json();
                const trashboxUrl = window.trashboxUrl;
                // Тело комментария
                const commentHtml = `
                            <div class="card-comment">
                                <div class="card-subtitle">
                                    <h6 class="card-subtitle-author">${commentData.author}</h6>
                                    <h6 class="card-subtitle-date">${commentData.timestamp}</h6>
                                            <span class="commentDel"><img src="${trashboxUrl}"></span>
                                    </div>
                                <p class="card-text">${commentData.text}</p>
                            </div>`;

                const commentSection = e.target.parentElement;
                const commentButton = '<button class="btn-primary addComment">Добавить комментарий</button>';
                // Добавляем комментарий
                commentSection.insertAdjacentHTML('beforeend', commentHtml);
                // Добавляем кнопку
                commentSection.insertAdjacentHTML('afterend', commentButton);

                // Удаляем поле ввода
                textarea.closest('.comment-input-container').remove();
                e.target.remove();
            } catch (error) {
                console.error('Ошибка:', error);
            }
        }
    });