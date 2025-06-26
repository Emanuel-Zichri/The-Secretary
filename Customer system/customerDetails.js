// customerDetails.js - עמוד פרטי לקוח מתקדם

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
  // Add page load animation
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
  
  // Add form validation
  setupFormValidation();
  
  // Load existing data if available
  loadExistingData();
});

function setupFormValidation() {
  const form = document.getElementById('customer-form');
  const inputs = form.querySelectorAll('input[required], textarea');
  
  inputs.forEach(input => {
    input.addEventListener('blur', validateField);
    input.addEventListener('input', clearValidationError);
  });
  
  // Phone number formatting
  const phoneInput = document.getElementById('phone');
  phoneInput.addEventListener('input', formatPhoneNumber);
}

function validateField(event) {
  const field = event.target;
  const value = field.value.trim();
  
  // Remove previous error styling
  clearValidationError(event);
  
  let isValid = true;
  let errorMessage = '';
  
  // Required field validation
  if (field.hasAttribute('required') && !value) {
    isValid = false;
    errorMessage = 'שדה זה הוא חובה';
  }
  
  // Specific field validations
  switch (field.id) {
    case 'firstName':
    case 'lastName':
      if (value && value.length < 2) {
        isValid = false;
        errorMessage = 'שם חייב להכיל לפחות 2 תווים';
      }
      break;
      
    case 'phone':
      if (value && !isValidPhoneNumber(value)) {
        isValid = false;
        errorMessage = 'מספר טלפון לא תקין';
      }
      break;
      
    case 'email':
      if (value && !isValidEmail(value)) {
        isValid = false;
        errorMessage = 'כתובת אימייל לא תקינה';
      }
      break;
      
    case 'city':
    case 'street':
      if (value && value.length < 2) {
        isValid = false;
        errorMessage = 'שדה זה חייב להכיל לפחות 2 תווים';
      }
      break;
      
    case 'houseNumber':
      if (value && !/^[0-9א-ת\s\-\/]+$/.test(value)) {
        isValid = false;
        errorMessage = 'מספר בית לא תקין';
      }
      break;
  }
  
  if (!isValid) {
    showFieldError(field, errorMessage);
  }
  
  return isValid;
}

function clearValidationError(event) {
  const field = event.target;
  const formField = field.closest('.form-field');
  
  // Remove error styling
  field.classList.remove('border-red-500', 'ring-red-500');
  field.classList.add('border-gray-300');
  
  // Remove error message
  const existingError = formField.querySelector('.error-message');
  if (existingError) {
    existingError.remove();
  }
}

function showFieldError(field, message) {
  const formField = field.closest('.form-field');
  
  // Add error styling
  field.classList.remove('border-gray-300');
  field.classList.add('border-red-500', 'ring-2', 'ring-red-500', 'ring-opacity-50');
  
  // Add error message
  const errorDiv = document.createElement('div');
  errorDiv.className = 'error-message text-red-500 text-sm mt-1 flex items-center';
  errorDiv.innerHTML = `
    <svg class="w-4 h-4 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
    </svg>
    ${message}
  `;
  
  const label = formField.querySelector('label');
  label.insertAdjacentElement('afterend', errorDiv);
}

function isValidPhoneNumber(phone) {
  // Remove all non-digits
  const digitsOnly = phone.replace(/\D/g, '');
  
  // Check if it's a valid Israeli phone number
  return /^0(5[0-9]|7[2-9]|[2-4]|8|9)[0-9]{7}$/.test(digitsOnly);
}

function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

function formatPhoneNumber(event) {
  const input = event.target;
  let value = input.value.replace(/\D/g, ''); // Remove non-digits
  
  // Format as XXX-XXXXXXX
  if (value.length >= 3) {
    value = value.substring(0, 3) + '-' + value.substring(3, 10);
  }
  
  input.value = value;
}

function loadExistingData() {
  const existingData = localStorage.getItem('installationData');
  if (existingData) {
    try {
      const data = JSON.parse(existingData);
      const customerDetails = data.customerDetails || {};
      
      // Fill form fields
      const fields = ['firstName', 'lastName', 'phone', 'email', 'city', 'street', 'notes'];
      fields.forEach(field => {
        const element = document.getElementById(field);
        if (element && customerDetails[field]) {
          element.value = customerDetails[field];
        }
      });
      
      // Handle house number (stored as 'number' in data)
      const houseNumberField = document.getElementById('houseNumber');
      if (houseNumberField && customerDetails.number) {
        houseNumberField.value = customerDetails.number;
      }
      
    } catch (error) {
      console.error('שגיאה בטעינת נתונים קיימים:', error);
    }
  }
}

function saveCustomerData() {
  const form = document.getElementById('customer-form');
  const formData = new FormData(form);
  
  // Validate all fields
  const inputs = form.querySelectorAll('input[required]');
  let isValid = true;
  
  inputs.forEach(input => {
    const fieldValid = validateField({ target: input });
    if (!fieldValid) {
      isValid = false;
    }
  });
  
  if (!isValid) {
    showErrorToast('אנא תקנו את השגיאות המסומנות באדום');
    
    // Scroll to first error
    const firstError = form.querySelector('.border-red-500');
    if (firstError) {
      firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
    return;
  }
  
  // Show loading state
  const button = document.getElementById('save-btn');
  const originalContent = button.innerHTML;
  button.innerHTML = `
    <svg class="animate-spin w-6 h-6 mx-auto" fill="none" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
  `;
  button.disabled = true;
  
  // Collect form data
  const customerDetails = {
    firstName: document.getElementById('firstName').value.trim(),
    lastName: document.getElementById('lastName').value.trim(),
    phone: document.getElementById('phone').value.trim(),
    email: document.getElementById('email').value.trim(),
    city: document.getElementById('city').value.trim(),
    street: document.getElementById('street').value.trim(),
    number: document.getElementById('houseNumber').value.trim(),
    notes: document.getElementById('notes').value.trim()
  };
  
  console.log('💾 שמירת פרטי לקוח:', customerDetails);
  
  // Get existing data and add customer details
  const existingData = localStorage.getItem('installationData');
  const fullData = existingData ? JSON.parse(existingData) : {};
  fullData.customerDetails = customerDetails;
  
  // Save to localStorage
  localStorage.setItem('installationData', JSON.stringify(fullData));
  
  showSuccessToast('פרטי הלקוח נשמרו בהצלחה!');
  
  // Navigate to next page after delay
  setTimeout(() => {
    window.location.href = 'quoteSummaryPage.html';
  }, 800);
}

function showSuccessToast(message) {
  const toast = document.createElement('div');
  toast.className = 'fixed top-20 left-1/2 transform -translate-x-1/2 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg z-50 transition-all duration-300 translate-y-[-100px] opacity-0';
  toast.innerHTML = `
    <div class="flex items-center">
      <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
      </svg>
      ${message}
    </div>
  `;
  document.body.appendChild(toast);
  
  setTimeout(() => {
    toast.classList.remove('translate-y-[-100px]', 'opacity-0');
    toast.classList.add('translate-y-0', 'opacity-100');
  }, 100);
  
  setTimeout(() => {
    toast.classList.add('translate-y-[-100px]', 'opacity-0');
    setTimeout(() => document.body.removeChild(toast), 300);
  }, 2000);
}

function showErrorToast(message) {
  const toast = document.createElement('div');
  toast.className = 'fixed top-20 left-1/2 transform -translate-x-1/2 bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg z-50 transition-all duration-300 translate-y-[-100px] opacity-0';
  toast.innerHTML = `
    <div class="flex items-center">
      <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
      </svg>
      ${message}
    </div>
  `;
  document.body.appendChild(toast);
  
  setTimeout(() => {
    toast.classList.remove('translate-y-[-100px]', 'opacity-0');
    toast.classList.add('translate-y-0', 'opacity-100');
  }, 100);
  
  setTimeout(() => {
    toast.classList.add('translate-y-[-100px]', 'opacity-0');
    setTimeout(() => document.body.removeChild(toast), 300);
  }, 3000);
}