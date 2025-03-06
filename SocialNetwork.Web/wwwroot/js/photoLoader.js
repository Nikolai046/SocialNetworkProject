//Обработчик проверки загружаемой фотографии

// Сохраняем исходный src изображения для возможности отмены
const defaultImageSrc = document.getElementById('user_photo').src;

// Обработчик проверки загружаемой фотографии
document.getElementById('photo').addEventListener('change',
    function(event) {
        const file = event.target.files[0];
        if (file) {
            // Проверка формата файла
            if (!['image/jpeg', 'image/jpg'].includes(file.type)) {
                alert('Файл должен быть в формате JPG или JPEG.');
                return;
            }

            // Проверка размера файла
            if (file.size > 1048576) { // 1 MB = 1048576 bytes
                alert('Размер файла не должен превышать 1 МБ.');
                return;
            }

            // Предварительный просмотр изображения
            const reader = new FileReader();
            reader.onload = function(e) {
                const previewImage = document.getElementById('user_photo');
                const selectFileButton = document.getElementById('select_a_file');
                const submitButton = document.querySelector('.add-photo-button[type="submit"]');

                // Устанавливаем источник изображения
                previewImage.src = e.target.result;

                // Заменяем кнопку "Выбрать файл" на "Отменить"
                submitButton.style.display = "inline-block";
                selectFileButton.textContent = 'Отменить';
                selectFileButton.onclick = function() {
                    // Возвращаем исходное изображение
                    previewImage.src = defaultImageSrc;

                    // Возвращаем кнопку "Выбрать файл"
                    submitButton.style.display = "none";
                    selectFileButton.textContent = 'Выбрать файл';
                    selectFileButton.onclick = function() {
                        document.getElementById('photo').click();
                    };

                    // // Очищаем поле выбора файла
                    document.getElementById('photo').value = '';
                };
            };
            reader.readAsDataURL(file); // Читаем файл как Data URL
        }
    });