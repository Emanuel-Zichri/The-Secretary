src="https://code.jquery.com/jquery-3.6.0.min.js"
src="/ajaxCall.js"

function submitAllData() {
    const date = document.getElementById('dateInput').value;
    if (!date) {
      alert('אנא בחר תאריך התקנה מועדף');
      return;
    }

    // קבלת כל הנתונים ששמורים בלוקלסטורג'
    const fullData = JSON.parse(localStorage.getItem('installationData')) || {};
    const parquet = localStorage.getItem('selectedParquet') || '';

    // הוספת הפרטים החסרים לאובייקט
    fullData.selectedParquet = parquet;
    fullData.preferredDate = date;

    // בניית אובייקט ה-JSON כפי ש-API מצפה
    const requestData = {
      customerDetails: fullData.customerDetails || {},
      spaceDetails: fullData.spaceDetails || [],
      selectedParquet: parquet,
      preferredDate: date
    };

    ajaxCall(
      'POST',
      `${API_BASE_URL}api/newRequest/RegisterNewRequest`,
      JSON.stringify(requestData),
      function (res) {
        alert('ההזמנה נשלחה בהצלחה!');
        window.location.href = 'thankyou.html';
      },
      function (err) {
        console.error('שגיאה בשליחה:', err);
        console.log('סטטוס:', err.status);
        console.log('תגובה מהשרת:', err.responseText);
        alert('אירעה שגיאה בשליחת הנתונים.');
      }
    );
  }