            // Функция для установки ширины footer равной ширине main-container или container (что больше)
            function setFooterWidth() {
                // Находим элементы
                const container = document.querySelector('.container');
                const mainContainer = document.querySelector('.main-container');
                const footer = document.querySelector('footer');

                // Проверяем, что элементы существуют
                if (mainContainer && footer && container) {
                    // Получаем ширину main-container
                    const mainContainerWidth = mainContainer.offsetWidth;
                    const ContainerWidth = container.offsetWidth;

                    // Устанавливаем ширину footer равной ширине main-container
                    if (mainContainerWidth > ContainerWidth)
                        footer.style.width = `${mainContainerWidth}px`;
                    else footer.style.width = `${ContainerWidth}px`;
                }
            }

        // Вызываем функцию при загрузке страницы
            window.addEventListener('load', setFooterWidth);

        // Вызываем функцию при изменении размера окна
            window.addEventListener('resize', setFooterWidth);