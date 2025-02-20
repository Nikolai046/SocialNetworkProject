console.log('JS loaded');
console.log(new Date().toISOString());

//Обработчик для загрузки сообщений и комментариев ленту

document.addEventListener('DOMContentLoaded',
    () => {
        let currentPage = 1;
        let hasMore = true;
        let loading = false;
        const userId = window.userId;
        const trashboxUrl = window.trashboxUrl;
        const pageSize = 10;

        // Создаем триггер для загрузки
        const loadTrigger = document.createElement('div');
        loadTrigger.id = 'load-trigger';
        loadTrigger.style.height = '1px';
        loadTrigger.style.visibility = 'hidden';
        document.getElementById('messageSection').appendChild(loadTrigger);

        // Наблюдатель за триггером
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && hasMore && !loading) {
                    loadMessages();
                }
            });
        },
            { threshold: 0.1 });

        observer.observe(loadTrigger);

        async function loadMessages() {
            loading = true;

            try {
                const response = await fetch(`/AccountManager/load-messages?UserID=${userId}&page=${currentPage}`);
                const result = await response.json();

                if (result.data && result.data.length > 0) {
                    // Добавляем сообщения
                    const messageSection = document.getElementById('messageSection');
                    result.data.forEach(message => {
                        const messageHtml = `
                           <div class="card" data-message-id="${message.messageId}">
                           <div class="card-body">
                           <div class="card-subtitle">
                             <h6 class="card-subtitle-author">${message.authorFullName}</h6>
                             <h6 class="card-subtitle-date">${new Date(message.createdAt).toLocaleString()}</h6>
                             ${message.deletable ? `<span class="messageDel"><img src="${trashboxUrl} "></span>` : ''}
                    </div >
                         <p class="card-text">${message.text}</p>
                           <div class="comment-section">
                    ${message.comments.map(comment => `
                        <div class="card-comment" data-comment-id="${comment.commentId}">
                          <div class="card-subtitle">
                              <h6 class="card-subtitle-author">${comment.author}</h6>
                              <h6 class="card-subtitle-date">${new Date(comment.createdAt).toLocaleString()}</h6>
                              ${comment.deletable ? `<span class="commentDel"><img src="\${trashboxUrl}"></span>` : ''}
                           </div>
                            <p class="card-text">${comment.text}</p>
                       </div >
                        `).join('')}
                            </div>
                            <button class="btn-primary addComment">Добавить комментарий</button>
                        </div>
                    </div>`;

                        messageSection.insertAdjacentHTML('beforeend', messageHtml);
                    });

                    // Обновляем флаг и страницу
                    hasMore = result.hasMore;
                    currentPage++;

                    // Перемещаем триггер в конец
                    messageSection.appendChild(loadTrigger);
                } else {
                    hasMore = false;
                    loadTrigger.remove();
                }
            } catch (error) {
                console.error('Ошибка загрузки:', error);
            } finally {
                loading = false;
            }
        }

        // Первоначальная загрузка
        loadMessages();
    });