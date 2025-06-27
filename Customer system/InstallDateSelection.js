// InstallDateSelection.js - לוח שנה לבחירת תאריך התקנה

let currentDate = new Date();
let selectedDate = null;
let selectedSlot = null;
const monthNames = [
  'ינואר', 'פברואר', 'מרץ', 'אפריל', 'מאי', 'יוני',
  'יולי', 'אוגוסט', 'ספטמבר', 'אוקטובר', 'נובמבר', 'דצמבר'
];

// Initialize calendar on page load
document.addEventListener('DOMContentLoaded', function() {
  initializeCalendar();
  
  // Add page load animation
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
});

function initializeCalendar() {
  // Set current date to today or next month if we're close to month end
  const today = new Date();
  const daysLeftInMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate() - today.getDate();
  
  if (daysLeftInMonth < 7) {
    currentDate = new Date(today.getFullYear(), today.getMonth() + 1, 1);
  } else {
    currentDate = new Date(today.getFullYear(), today.getMonth(), 1);
  }
  
  renderCalendar();
}

function renderCalendar() {
  const monthYear = `${monthNames[currentDate.getMonth()]} ${currentDate.getFullYear()}`;
  document.getElementById('calendar-month').textContent = monthYear;
  
  const daysContainer = document.getElementById('calendar-days');
  daysContainer.innerHTML = '';
  
  const firstDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
  const lastDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  
  // Get first day of week (Sunday = 0, Monday = 1, etc.)
  // Convert to Hebrew calendar (Sunday = 0)
  let startDay = firstDay.getDay();
  
  // Add empty cells for days before month starts
  for (let i = 0; i < startDay; i++) {
    const emptyCell = document.createElement('div');
    emptyCell.className = 'h-10';
    daysContainer.appendChild(emptyCell);
  }
  
  // Add days of the month
  for (let day = 1; day <= lastDay.getDate(); day++) {
    const date = new Date(currentDate.getFullYear(), currentDate.getMonth(), day);
    const dayElement = document.createElement('div');
    
    dayElement.className = 'calendar-day h-10 flex items-center justify-center rounded-lg cursor-pointer text-sm font-medium';
    dayElement.textContent = day;
    
    // Check if date is in the past
    if (date < today) {
      dayElement.classList.add('past', 'text-gray-300');
      dayElement.style.cursor = 'not-allowed';
    }
    // Check if date is weekend (Friday = 5, Saturday = 6)
    else if (date.getDay() === 5 || date.getDay() === 6) {
      dayElement.classList.add('disabled', 'text-gray-400', 'bg-gray-100');
      dayElement.title = 'אנחנו לא עובדים בסופי שבוע';
    }
    // Available date
    else {
      dayElement.classList.add('text-gray-700', 'hover:bg-primary/10', 'hover:text-primary');
      dayElement.addEventListener('click', () => selectDate(date, dayElement));
    }
    
    // Highlight selected date
    if (selectedDate && 
        date.getFullYear() === selectedDate.getFullYear() &&
        date.getMonth() === selectedDate.getMonth() &&
        date.getDate() === selectedDate.getDate()) {
      dayElement.classList.add('selected');
    }
    
    daysContainer.appendChild(dayElement);
  }
}

function selectDate(date, element) {
  // Remove previous selection
  document.querySelectorAll('.calendar-day.selected').forEach(el => {
    el.classList.remove('selected');
  });
  
  // Add selection to clicked element
  element.classList.add('selected');
  selectedDate = date;
  
  // Show selected date
  const dateText = `${date.getDate()} ${monthNames[date.getMonth()]} ${date.getFullYear()}`;
  document.getElementById('selected-date-text').textContent = dateText;
  document.getElementById('selected-date-display').classList.remove('hidden');
  
  // Show time slots
  const timeSlotsSection = document.getElementById('time-slots-section');
  timeSlotsSection.classList.remove('hidden');
  timeSlotsSection.scrollIntoView({ behavior: 'smooth', block: 'center' });
  
  // Reset time slot selection
  selectedSlot = null;
  document.querySelectorAll('input[name="preferredSlot"]').forEach(input => {
    input.checked = false;
    const label = input.closest('label');
    label.classList.remove('border-primary', 'bg-primary/5');
    label.classList.add('border-gray-200');
    const dot = label.querySelector('.w-2');
    dot.classList.add('hidden');
  });
  
  updateSubmitButton();
  showSuccessToast('תאריך נבחר בהצלחה');
}

function selectTimeSlot(input) {
  selectedSlot = parseInt(input.value);
  
  // Update visual state of all time slots
  document.querySelectorAll('input[name="preferredSlot"]').forEach(otherInput => {
    const label = otherInput.closest('label');
    const dot = label.querySelector('.w-2');
    
    if (otherInput === input) {
      label.classList.remove('border-gray-200');
      label.classList.add('border-primary', 'bg-primary/5');
      dot.classList.remove('hidden');
    } else {
      label.classList.add('border-gray-200');
      label.classList.remove('border-primary', 'bg-primary/5');
      dot.classList.add('hidden');
    }
  });
  
  updateSubmitButton();
  showSuccessToast('שעה נבחרה בהצלחה');
}

function updateSubmitButton() {
  const submitBtn = document.getElementById('submit-btn');
  
  if (selectedDate && selectedSlot) {
    submitBtn.disabled = false;
    submitBtn.className = 'w-full bg-primary hover:bg-primary-dark text-white font-semibold py-4 px-6 rounded-xl shadow-lg transition-all duration-300 transform hover:scale-105 active:scale-95';
  } else {
    submitBtn.disabled = true;
    submitBtn.className = 'w-full bg-gray-300 text-gray-500 font-semibold py-4 px-6 rounded-xl transition-all duration-300 disabled:cursor-not-allowed';
  }
}

function previousMonth() {
  const today = new Date();
  const newDate = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
  
  // Don't allow going to past months
  if (newDate.getFullYear() < today.getFullYear() || 
      (newDate.getFullYear() === today.getFullYear() && newDate.getMonth() < today.getMonth())) {
    showErrorToast('לא ניתן לבחור תאריכים מהעבר');
    return;
  }
  
  currentDate = newDate;
  renderCalendar();
}

function nextMonth() {
  // Limit to 6 months in advance
  const maxDate = new Date();
  maxDate.setMonth(maxDate.getMonth() + 6);
  
  const newDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 1);
  
  if (newDate > maxDate) {
    showErrorToast('ניתן לתאם התקנה עד 6 חודשים מראש');
    return;
  }
  
  currentDate = newDate;
  renderCalendar();
}

function submitAllData() {
  if (!selectedDate || !selectedSlot) {
    showErrorToast('אנא בחרו תאריך ושעה');
    return;
  }

  // Show loading state
  const button = document.getElementById('submit-btn');
  const originalContent = button.innerHTML;
  button.innerHTML = `
    <svg class="animate-spin w-6 h-6 mx-auto" fill="none" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
  `;
  button.disabled = true;

  // Get all data from localStorage
  const fullData = JSON.parse(localStorage.getItem('installationData')) || {};
  const parquet = localStorage.getItem('ParquetType') || '';

  // Add date and slot to data
  fullData.selectedParquet = parquet;
  fullData.preferredDate = selectedDate.toISOString();
  fullData.preferredSlot = selectedSlot;

  // Build request data for API
  const requestData = {
    customerDetails: fullData.customerDetails || {},
    spaceDetails: fullData.spaceDetails || [],
    selectedParquet: parquet,
    preferredDate: selectedDate.toISOString(),
    preferredSlot: selectedSlot
  };

  // Register new request
  ajaxCall(
    'POST',
    `${API_BASE_URL}/NewRequest/RegisterNewRequest`,
    JSON.stringify(requestData),
    function(response) {
      // Handle different response formats
      let requestID = null;
      
      if (typeof response === 'number') {
        requestID = response;
      } else if (response && (response.requestID || response.RequestID)) {
        requestID = response.requestID || response.RequestID;
      } else if (response && response.NewCustomerID) {
        requestID = response.NewCustomerID;
      }
      
      if (requestID && requestID > 0) {
        setTimeout(() => {
          calculateAndSaveEstimate(requestID);
        }, 1000);
      } else {
        showErrorToast('שגיאה בשליחת הבקשה - לא התקבל מספר בקשה');
        button.innerHTML = originalContent;
        button.disabled = false;
      }
    },
    function(xhr, status, error) {
      showErrorToast('שגיאה בשליחת הבקשה. אנא נסו שוב.');
      button.innerHTML = originalContent;
      button.disabled = false;
    }
  );
}



function calculateAndSaveEstimate(requestID) {
  const fullData = JSON.parse(localStorage.getItem('installationData')) || {};
  const parquetType = localStorage.getItem('ParquetType') || '';
  
  // Calculate total area
  const totalArea = fullData.spaceDetails?.reduce((sum, space) => sum + (Number(space.size) || 0), 0) || 0;
  const roomCount = fullData.spaceDetails?.length || 1;
  
  const estimateData = {
    RequestID: requestID,
    TotalArea: totalArea,
    ParquetType: parquetType,
    RoomCount: roomCount,
    SpaceDetails: fullData.spaceDetails || [],
    HasSpecialRequirements: fullData.spaceDetails?.some(space => space.notes?.trim()) || false,
    HasMultipleFloorTypes: new Set(fullData.spaceDetails?.map(space => space.floorType)).size > 1
  };

  ajaxCall(
    'POST',
    `${API_BASE_URL}/PriceEstimator/CalculateAndSave`,
    JSON.stringify(estimateData),
    function(response) {
      showSuccessModal();
    },
    function(xhr, status, error) {
      showErrorToast('שגיאה בשמירת הערכת מחיר. הבקשה נשמרה אך ההערכה לא חושבה.');
      setTimeout(() => {
        showSuccessModal();
      }, 2000);
    }
  );
}

function showSuccessModal() {
  const modal = document.getElementById('success-modal');
  modal.classList.remove('hidden');
  
  // Add entrance animation
  const content = modal.querySelector('.bg-white');
  content.style.opacity = '0';
  content.style.transform = 'scale(0.95)';
  
  setTimeout(() => {
    content.style.transition = 'all 0.3s ease';
    content.style.opacity = '1';
    content.style.transform = 'scale(1)';
  }, 50);
}

function goToThankYou() {
  // Clear localStorage
  localStorage.removeItem('installationData');
  localStorage.removeItem('ParquetType');
  
  window.location.href = 'thankyou.html';
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