// Обработчик автоматической подгонки высоты textarea

document.addEventListener('DOMContentLoaded',
    function () {
        // Функция для настройки textarea
        function processTextarea(textarea) {
            // Если обработчик уже прикреплён, пропускаем
            if (!textarea.hasAttribute('data-autosize-attached')) {
                textarea.addEventListener('input',
                    function () {
                        textarea.style.height = 'auto';
                        textarea.style.height = textarea.scrollHeight + 'px';
                    });
                textarea.setAttribute('data-autosize-attached', 'true');
            }
            // Первоначальная настройка высоты
            textarea.style.height = 'auto';
            textarea.style.height = textarea.scrollHeight + 'px';
        }

        // Инициализируем уже существующие textarea
        document.querySelectorAll('textarea.form-control').forEach(processTextarea);

        // Создаём наблюдатель за изменениями в документе
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    // Если добавлен узел является элементом
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        // Если сам узел является textarea с нужным классом
                        if (node.matches && node.matches('textarea.form-control')) {
                            processTextarea(node);
                        }
                        // Если внутри добавленного узла есть дочерние textarea
                        if (node.querySelectorAll) {
                            node.querySelectorAll('textarea.form-control').forEach(processTextarea);
                        }
                    }
                });
            });
        });

        // Настраиваем наблюдение за всем документом
        observer.observe(document.body, { childList: true, subtree: true });
    });