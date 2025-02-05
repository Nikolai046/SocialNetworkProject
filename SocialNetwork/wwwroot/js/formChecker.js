document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form"); // Находим форму

    // Добавляем обработчик события на отправку формы
    form.addEventListener("submit", function (event) {
    
        // Находим все сообщения верификации
        const validationMessages = document.querySelectorAll(
            ".field-validation-valid, .field-validation-error"
        );

        // Добавляем класс 'show' ко всем сообщениям
        validationMessages.forEach((message) => {
            message.classList.add("show");
        });
    });
});