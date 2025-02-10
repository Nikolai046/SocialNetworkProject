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

    // Инициализация годов и месяцев при загрузке страницы
    document.addEventListener('DOMContentLoaded', () => {
        const yearSelect = document.getElementById('birth-year');
        const monthSelect = document.getElementById('birth-month');
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