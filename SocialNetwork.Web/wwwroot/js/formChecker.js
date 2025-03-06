//Обработчик, который показывает ошибки валидатора ASP.NET только после нажатия submit

document.addEventListener("DOMContentLoaded",
    function() {
        // Ищем форму с id="checkMe"
        const form = document.querySelector("#editForm");

        // Проверяем, что форма найдена, чтобы избежать ошибок, если элемента нет
        if (form) {
            // Добавляем обработчик события на отправку формы
            form.addEventListener("submit",
                function(event) {
                    // Находим все сообщения валидации
                    const validationMessages = document.querySelectorAll(
                        ".field-validation-valid, .field-validation-error"
                    );

                    // Добавляем класс 'show' ко всем сообщениям
                    validationMessages.forEach((message) => {
                        message.classList.add("show");
                    });
                });
        } else {
            console.error("Форма с id 'myForm' не найдена");
        }
    });