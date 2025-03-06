// Обработчик для формы выбора даты рождения

// Массив месяцев
const months = [
    { name: "Январь", value: 1 },
    { name: "Февраль", value: 2 },
    { name: "Март", value: 3 },
    { name: "Апрель", value: 4 },
    { name: "Май", value: 5 },
    { name: "Июнь", value: 6 },
    { name: "Июль", value: 7 },
    { name: "Август", value: 8 },
    { name: "Сентябрь", value: 9 },
    { name: "Октябрь", value: 10 },
    { name: "Ноябрь", value: 11 },
    { name: "Декабрь", value: 12 }
];

// Получаем существующие значения из модели
const existingYear = window.existingYear;
const existingMonth = window.existingMonth;
const existingDay = window.existingDay;

// Инициализация годов, месяцев и дней при загрузке страницы
document.addEventListener('DOMContentLoaded',
    () => {
        const yearSelect = document.getElementById('birth-year');
        const monthSelect = document.getElementById('birth-month');
        const daySelect = document.getElementById('birth-day');
        const currentYear = new Date().getFullYear();
        const startYear = 1940;

        // Заполняем года
        for (let year = currentYear - 10; year >= startYear; year--) {
            yearSelect.appendChild(new Option(year, year));
        }

        // Заполняем месяцы
        months.forEach(month => {
            monthSelect.appendChild(new Option(month.name, month.value));
        });

        // Устанавливаем существующие значения, если они есть
        if (existingYear !== null) {
            yearSelect.value = existingYear;
        }
        if (existingMonth !== null) {
            monthSelect.value = existingMonth;
        }

        // Обновляем список дней на основе выбранных года и месяца
        updateDays();

        // Устанавливаем существующий день, если он есть
        if (existingDay !== null) {
            daySelect.value = existingDay;
        }
    });

function updateDays() {
    const month = document.getElementById("birth-month").value;
    const year = document.getElementById("birth-year").value;
    const daysSelect = document.getElementById("birth-day");

    daysSelect.innerHTML = '<option value="">День</option>';

    if (month && year) {
        const daysInMonth = new Date(year, month, 0).getDate();
        for (let day = 1; day <= daysInMonth; day++) {
            daysSelect.appendChild(new Option(day, day));
        }
    }
}