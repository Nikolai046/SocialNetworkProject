// Обработчик модального окна "Да/Отмена"
function showConfirmModal(message, confirmCallback) {
    const modal = document.getElementById('confirmModal');
    const modalText = document.getElementById('modalText');
    const confirmButton = document.getElementById('confirmButton');
    const cancelButton = document.getElementById('cancelButton');

    modalText.textContent = message;
    modal.style.display = 'flex';

    const closeModal = () => {
        modal.style.display = 'none';
        confirmButton.removeEventListener('click', confirmHandler);
        cancelButton.removeEventListener('click', closeModal);
        window.removeEventListener('keydown', handleEscape);
    };

    const confirmHandler = () => {
        confirmCallback();
        closeModal();
    };

    const handleEscape = (e) => {
        if (e.key === 'Escape') closeModal();
    };

    confirmButton.addEventListener('click', confirmHandler);
    cancelButton.addEventListener('click', closeModal);
    window.addEventListener('keydown', handleEscape);
    modal.addEventListener('click', (e) => {
        if (e.target === modal) closeModal();
    });
}