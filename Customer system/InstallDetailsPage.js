let roomCount = 0;

const roomTypeOptions = [
  "סלון",
  "חדר שינה",
  "מטבח",
  "חדר רחצה",
  "מרפסת",
  "חדר ילדים",
  "מסדרון",
  "חדר עבודה",
  "פינת אוכל"
];

function generateRoomTypeSelect() {
  return `
    <select required class="w-full p-3 border border-gray-300 rounded-xl focus:border-primary focus:ring-2 focus:ring-primary/20 transition-colors bg-white" onchange="updateHeader(this)">
      <option value="">בחר סוג חלל</option>
      ${roomTypeOptions.map(type => `<option value="${type}">${type}</option>`).join('')}
    </select>
  `;
}

function addRoom() {
  roomCount++;
  const container = document.getElementById('rooms-container');
  const div = document.createElement('div');
  div.className = 'room-card bg-white rounded-2xl shadow-sm border border-orange-100 overflow-hidden slide-in';
  div.innerHTML = `
    <div class="room-header bg-gradient-to-r from-gray-50 to-gray-100 px-6 py-4 flex items-center justify-between border-b border-gray-200">
      <div class="flex items-center">
        <div class="w-8 h-8 bg-primary/10 rounded-full flex items-center justify-center ml-3">
          <svg class="w-4 h-4 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
          </svg>
        </div>
        <span class="font-semibold text-gray-800" data-base-label="חלל ${roomCount}">חלל ${roomCount}</span>
      </div>
      <div class="flex items-center space-x-2 space-x-reverse">
        <button class="toggle-btn p-2 hover:bg-white rounded-full transition-colors" onclick="toggleRoom(this)" title="כווץ/הרחב">
          <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
          </svg>
        </button>
        <button class="remove-btn p-2 hover:bg-red-50 text-red-600 rounded-full transition-colors" onclick="removeRoom(this.closest('.room-card'))" title="מחק חלל">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
          </svg>
        </button>
      </div>
    </div>
    <div class="room-body p-6 space-y-4">
      <div class="form-group">
        <label class="block text-sm font-medium text-gray-700 mb-2">סוג החלל</label>
        ${generateRoomTypeSelect()}
      </div>
      <div class="form-group relative">
        <label class="block text-sm font-medium text-gray-700 mb-2">שטח החלל (מ"ר)</label>
        <div class="relative">
          <input type="number" step="0.1" min="0.1" placeholder="לדוגמה: 12.5" required 
                 class="w-full p-3 border border-gray-300 rounded-xl focus:border-primary focus:ring-2 focus:ring-primary/20 transition-colors pl-12" />
          <button type="button" onclick="showPopup(event)" 
                  class="absolute left-3 top-1/2 transform -translate-y-1/2 w-6 h-6 bg-primary/10 hover:bg-primary/20 rounded-full flex items-center justify-center transition-colors">
            <svg class="w-3 h-3 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </button>
        </div>
      </div>
      <div class="form-group">
        <label class="block text-sm font-medium text-gray-700 mb-2">הערות (אופציונלי)</label>
        <textarea placeholder="הערות מיוחדות על החלל..." 
                  class="w-full p-3 border border-gray-300 rounded-xl focus:border-primary focus:ring-2 focus:ring-primary/20 transition-colors resize-none h-20"></textarea>
      </div>
      <div class="form-group">
        <label class="block text-sm font-medium text-gray-700 mb-2">העלאת תמונה/סרטון (אופציונלי)</label>
        <div class="relative">
          <input type="file" accept="image/*,video/*" class="hidden" id="file-${roomCount}" onchange="handleFileUpload(this)" />
          <label for="file-${roomCount}" class="flex items-center justify-center w-full p-4 border-2 border-dashed border-gray-300 rounded-xl hover:border-primary/50 hover:bg-primary/5 transition-colors cursor-pointer">
            <div class="text-center">
              <svg class="w-8 h-8 text-gray-400 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"></path>
              </svg>
              <p class="text-sm text-gray-600">לחץ להעלאת קובץ</p>
              <p class="text-xs text-gray-400 mt-1">תמונה או סרטון עד 10MB</p>
            </div>
          </label>
        </div>
        <div class="file-preview hidden mt-2 p-3 bg-green-50 border border-green-200 rounded-lg">
          <div class="flex items-center">
            <svg class="w-5 h-5 text-green-600 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <span class="text-sm text-green-800 file-name"></span>
          </div>
        </div>
      </div>
    </div>`;
  container.appendChild(div);
  
  // Add stagger animation
  setTimeout(() => {
    div.style.opacity = '1';
    div.style.transform = 'translateY(0)';
  }, 100);
  
  // Scroll to new room
  div.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function toggleRoom(btn) {
  const room = btn.closest('.room-card');
  const body = room.querySelector('.room-body');
  const icon = btn.querySelector('svg');
  
  room.classList.toggle('collapsed');
  
  if (room.classList.contains('collapsed')) {
    body.style.display = 'none';
    icon.style.transform = 'rotate(-90deg)';
  } else {
    body.style.display = 'block';
    icon.style.transform = 'rotate(0deg)';
  }
}

function updateHeader(select) {
  const header = select.closest('.room-card').querySelector('.room-header span');
  const baseLabel = header.dataset.baseLabel || header.textContent;
  const value = select.value?.trim() || "";
  
  if (value !== "") {
    header.textContent = value;
    header.classList.add('text-primary');
  } else {
    header.textContent = baseLabel;
    header.classList.remove('text-primary');
  }
}

function removeRoom(roomDiv) {
  const container = document.getElementById('rooms-container');
  const rooms = container.querySelectorAll('.room-card');
  
  if (rooms.length > 1) {
    // Add fade out animation
    roomDiv.style.opacity = '0';
    roomDiv.style.transform = 'translateX(-100%)';
    
    setTimeout(() => {
      roomDiv.remove();
      showSuccessToast('החלל הוסר בהצלחה');
    }, 300);
  } else {
    showErrorToast("לא ניתן להסיר את החלל האחרון");
  }
}

function handleFileUpload(input) {
  const file = input.files[0];
  const preview = input.closest('.form-group').querySelector('.file-preview');
  const fileName = preview.querySelector('.file-name');
  
  if (file) {
    if (file.size > 10 * 1024 * 1024) { // 10MB limit
      showErrorToast('הקובץ גדול מדי. גודל מקסימלי: 10MB');
      input.value = '';
      return;
    }
    
    fileName.textContent = file.name;
    preview.classList.remove('hidden');
    showSuccessToast('קובץ הועלה בהצלחה');
  } else {
    preview.classList.add('hidden');
  }
}

function saveAllRooms() {
  const roomDivs = document.querySelectorAll('.room-card');
  const spaceDetails = [];
  let isValid = true;
  const parquetType = localStorage.getItem('ParquetType');

  roomDivs.forEach((div, index) => {
    const select = div.querySelector('select');
    const inputs = div.querySelectorAll('input');
    const textarea = div.querySelector('textarea');
    
    const floorType = select?.value?.trim() || "";
    const sizeInput = inputs[0];
    const size = parseFloat(sizeInput?.value.trim());
    const notes = textarea?.value.trim() || "";
    const mediaFile = inputs[1]?.files[0];

    // Reset previous error states
    div.classList.remove('ring-2', 'ring-red-500', 'ring-opacity-50');
    
    if (!floorType || isNaN(size) || size <= 0) {
      isValid = false;
      div.classList.add('ring-2', 'ring-red-500', 'ring-opacity-50');
    } else {
      spaceDetails.push({
        spaceID: index,
        requestID: 0,
        size: size,
        floorType: floorType,
        mediaURL: mediaFile ? mediaFile.name : '',
        notes: notes,
        ParquetType: parquetType
      });
    }
  });

  if (!isValid) {
    showErrorToast('אנא תקן את השגיאות המסומנות באדום');
    const firstError = document.querySelector('.room-card.ring-red-500');
    if (firstError) {
      firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
    return;
  }

  if (spaceDetails.length === 0) {
    showErrorToast('יש להוסיף לפחות חלל אחד');
    return;
  }

  // Show loading state
  const button = event.target;
  button.innerHTML = `
    <svg class="animate-spin w-5 h-5 mx-auto" fill="none" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 714 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
  `;
  button.disabled = true;

  // Save data
  const existingData = localStorage.getItem('installationData');
  const parsed = existingData ? JSON.parse(existingData) : {};
  parsed.spaceDetails = spaceDetails;

  localStorage.setItem('installationData', JSON.stringify(parsed));
  showSuccessToast('הנתונים נשמרו בהצלחה!');
  
  setTimeout(() => {
    window.location.href = 'customerDetails.html';
  }, 800);
}

function showPopup(event) {
  const popup = document.getElementById('popup');
  popup.classList.remove('hidden');
  
  // Add entrance animation
  const content = popup.querySelector('.bg-white');
  content.style.opacity = '0';
  content.style.transform = 'scale(0.95)';
  
  setTimeout(() => {
    content.style.transition = 'all 0.3s ease';
    content.style.opacity = '1';
    content.style.transform = 'scale(1)';
  }, 50);
}

function closePopup() {
  const popup = document.getElementById('popup');
  const content = popup.querySelector('.bg-white');
  
  content.style.opacity = '0';
  content.style.transform = 'scale(0.95)';
  
  setTimeout(() => {
    popup.classList.add('hidden');
  }, 300);
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

// Initialize page
window.addEventListener('DOMContentLoaded', () => {
  addRoom(); // הוסף חלל ראשון כברירת מחדל
  
  // Add page load animation
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
});
