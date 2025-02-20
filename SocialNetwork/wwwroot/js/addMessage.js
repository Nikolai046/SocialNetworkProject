    // Обработчик отправки сообщения
    document.getElementById('sendMessage').addEventListener('click',
        async function() {
            const messageInput = document.getElementById('messageInput');
            const text = messageInput.value.trim();

            if (!text) return;

            try {
                const response = await fetch('/AccountManager/add-message',
                    {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({ Text: text })
                    });

                const messageData = await response.json();

                // Добавляем сообщение в интерфейс
                const messageHtml = `
                    <div class="card" data-message-id="${messageData.id}">
                        <div class="card-body">
                            <div class="card-subtitle">
                                <h6 class="card-subtitle-author">${messageData.author}</h6>
                                <h6 class="card-subtitle-date">${messageData.timestamp}</h6>
                                <span class="messageDel"><img src="@Url.Content("~/images/trashbox.svg")"></span>
                            </div>
                            <p class="card-text">${messageData.text}</p>
                            <div class="comment-section"></div>
                            <button class="btn-primary addComment">Добавить комментарий</button>
                        </div>
                    </div>`;

                document.getElementById('messageSection').insertAdjacentHTML('afterbegin', messageHtml);
                messageInput.value = '';
                messageInput.style.height = 'auto';

            } catch (error) {
                console.error('Ошибка:', error);
            }
        });