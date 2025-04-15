document.getElementById('login-form').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    // כאן תוכל להוסיף לוגיקה לאימות פרטי הכניסה
    alert(`ניסיון כניסה עם שם משתמש: ${username} וסיסמה: ${password}`);
});

document.addEventListener('DOMContentLoaded', function() {
    const installationsToday = document.getElementById('installations-today');
    const completedInstallations = document.getElementById('completed-installations');
    const attentionNeeded = document.getElementById('attention-needed');

    installationsToday.textContent = '5';
    completedInstallations.textContent = '3';
    attentionNeeded.textContent = '2';

    const calendar = document.getElementById('calendar');
    const today = new Date();
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    calendar.textContent = today.toLocaleDateString('he-IL', options);

    const notificationsList = document.getElementById('notifications-list');
    const notifications = [
        'התקנה חדשה: לקוח 123',
        'התקנה בוטלה: לקוח 456'
    ];

    notifications.forEach(notification => {
        const li = document.createElement('li');
        li.textContent = notification;
        notificationsList.appendChild(li);
    });
});