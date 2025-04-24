src="https://code.jquery.com/jquery-3.6.0.min.js"
src="/ajaxCall.js"

function saveCustomerData() {
    const customer = {
      customerID: 0,
      firstName: document.getElementById('firstName').value,
      lastName: document.getElementById('lastName').value,
      phone: document.getElementById('phone').value,
      email: document.getElementById('email').value,
      createdAt: new Date().toISOString(),
      city: document.getElementById('city').value,
      street: document.getElementById('street').value,
      number: document.getElementById('number').value,
      notes: document.getElementById('notes').value
    };

    const existingData = localStorage.getItem('installationData');
    const parsed = existingData ? JSON.parse(existingData) : {};
    parsed.customerDetails = customer;
    localStorage.setItem('installationData', JSON.stringify(parsed));
    window.location.href = 'quoteSummaryPage.html';
  }