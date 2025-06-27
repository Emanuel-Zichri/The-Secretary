let parquetData = {
  solid: [],
  laminate: [],
  fishbone: [],
};

function fetchParquetTypes() {
  const container = document.getElementById("parquet-options");
  container.innerHTML = `
    <div class="col-span-2 text-center py-12">
      <div class="animate-spin w-8 h-8 border-2 border-primary border-t-transparent rounded-full mx-auto mb-4"></div>
      <p class="text-gray-600">טוען סוגי פרקטים...</p>
    </div>
  `;
  
  ajaxCall(
    "GET",
    `${API_BASE_URL}/ParquetType/GetAll`,
    null,
    function (types) {
      if (!types || types.length === 0) {
        loadDemoData();
        return;
      }

      parquetData.solid = [];
      parquetData.laminate = [];
      parquetData.fishbone = [];

      types.forEach((type) => {
        // ננקה ונתקן את הנתונים
        const cleanType = {
          ...type,
          typeName: type.typeName || "פרקט איכותי",
          description: type.description || "פרקט איכותי ומתקדם המתאים לכל סוגי החללים",
          imageURL: (type.imageURL && type.imageURL !== 'string' && !type.imageURL.includes('example.com')) ? type.imageURL : null
        };
        
        const typeName = cleanType.typeName.toLowerCase();
        if (typeName.includes("עץ") || typeName.includes("אלון") || typeName.includes("אגוז") || typeName.includes("אורן")) {
          parquetData.solid.push(cleanType);
        } else if (typeName.includes("למינציה") || typeName.includes("למינט")) {
          parquetData.laminate.push(cleanType);
        } else if (typeName.includes("פישבון") || typeName.includes("הרינגבון")) {
          parquetData.fishbone.push(cleanType);
        } else {
          parquetData.solid.push(cleanType);
        }
      });

      loadParquetItems('solid');
    },
    function (xhr, status, error) {
      loadDemoData();
      showInfoToast("עובד במצב הדגמה - נתונים דמה בלבד");
    }
  );
}


function loadDemoData() {
  parquetData.solid = [
    {
      parquetTypeID: 1,
      typeName: "עץ מלא אלון טבעי",
      pricePerUnit: 85,
      imageURL: "etzMaleAlonTivi.jpg",
      type: "עץ מלא",
      isActive: true,
      description: "פרקט עץ מלא איכותי מעץ אלון טבעי. עמיד ויפה במיוחד, מתאים לכל סוגי החללים."
    },
    {
      parquetTypeID: 2,
      typeName: "עץ מלא אגוז טבעי",
      pricePerUnit: 95,
      imageURL: "etzMaleEgozTivi.jpg",
      type: "עץ מלא",
      isActive: true,
      description: "פרקט עץ מלא מעץ אגוז איכותי עם גוונים חמים ומרקם יפה."
    },
    {
      parquetTypeID: 3,
      typeName: "עץ מלא אורן מעובד",
      pricePerUnit: 75,
      imageURL: "etzMalegoshniTivi.jpg",
      type: "עץ מלא",
      isActive: true,
      description: "פרקט עץ מלא מעץ אורן מעובד, איכותי וחסכוני."
    }
  ];
  
  parquetData.laminate = [
    {
      parquetTypeID: 4,
      typeName: "למינציה איכותית דמוי עץ",
      pricePerUnit: 45,
      imageURL: "etzMaleAlonTivi.jpg", // השתמש בתמונה קיימת
      type: "למינציה",
      isActive: true,
      description: "למינציה איכותית עמידה במיוחד עם מראה דמוי עץ טבעי."
    }
  ];
  
  parquetData.fishbone = [
    {
      parquetTypeID: 5,
      typeName: "פרקט פישבון מעוצב",
      pricePerUnit: 120,
      imageURL: "etzMaleEgozTivi.jpg", // השתמש בתמונה קיימת
      type: "פישבון",
      isActive: true,
      description: "פרקט פישבון מעוצב במיוחד, יוצר מראה אלגנטי ומיוחד."
    }
  ];
  
  loadParquetItems('solid');
}


function showInfoToast(message) {
  const toast = document.createElement('div');
  toast.className = 'fixed top-20 left-1/2 transform -translate-x-1/2 bg-blue-500 text-white px-6 py-3 rounded-lg shadow-lg z-50 transition-all duration-300 translate-y-[-100px] opacity-0';
  toast.innerHTML = `
    <div class="flex items-center">
      <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
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
  }, 4000);
}

function selectTab(tabEl, type) {
  const tabs = document.querySelectorAll('.tab');
  const indicator = document.querySelector('.tab-indicator');
  const index = parseInt(tabEl.dataset.index);
  
  // נקה את כל הטאבים
  tabs.forEach(tab => {
    tab.classList.remove('text-white', 'active');
    tab.classList.add('text-gray-600');
  });
  
  // הפעל את הטאב הנבחר
  tabEl.classList.remove('text-gray-600');
  tabEl.classList.add('text-white', 'active');
  
  // זוז עם האינדיקטור
  let rightPosition;
  if (index === 0) rightPosition = '0%';            // עץ מלא
  else if (index === 1) rightPosition = '33.33%';   // למינציה  
  else if (index === 2) rightPosition = '66.67%';   // פישבון
  
  indicator.style.right = rightPosition;
  
  // עדכן כותרת
  document.getElementById("parquet-title").textContent = "פרקטי " + tabEl.textContent;
  
  // נקה בחירות קודמות
  const selectedCards = document.querySelectorAll('.parquet-card.selected');
  selectedCards.forEach(card => {
    card.classList.remove('ring-2', 'ring-primary', 'ring-opacity-50', 'border-primary', 'scale-105', 'selected');
    const overlay = card.querySelector('.selection-overlay');
    const check = card.querySelector('.selection-check');
    if (overlay) overlay.style.opacity = '0';
    if (check) {
      check.style.opacity = '0';
      check.style.transform = 'scale(0.8)';
    }
  });
  
  // טען פרקטים
  const container = document.getElementById("parquet-options");
  container.style.opacity = '0.5';
  
  setTimeout(() => {
    loadParquetItems(type);
    container.style.opacity = '1';
  }, 150);
}

function loadParquetItems(type) {
  const container = document.getElementById("parquet-options");
  const items = parquetData[type] || [];
  
  container.innerHTML = '';
  
  if (items.length === 0) {
    container.innerHTML = `
      <div class="col-span-2 text-center py-8">
        <div class="text-gray-400 mb-2">
          <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2M4 13h2m13-8V4a1 1 0 00-1-1H7a1 1 0 00-1 1v1m8 0V4a1 1 0 00-1-1H9a1 1 0 00-1 1v1m4 0h1m-5 0h1m5 0v1H8V5"></path>
          </svg>
        </div>
        <p class="text-gray-600">אין פרקטים זמינים בקטגוריה זו</p>
      </div>
    `;
    return;
  }
  
  items.forEach((item, index) => {
    const imagePath = getImagePath(item.imageURL, item.typeName);
    
    const itemHtml = `
      <div class="parquet-card bg-white rounded-xl shadow-sm border-2 border-orange-100 overflow-hidden transform transition-all duration-300 hover:shadow-xl cursor-pointer group" 
           style="animation-delay: ${index * 100}ms" 
           data-parquet='${JSON.stringify(item)}'>
        <div class="relative overflow-hidden">
          <img src="${imagePath}" alt="${item.typeName}" class="w-full h-32 object-cover transition-transform duration-300 group-hover:scale-105" 
               onerror="this.src='../picturs/logo.png'" />
          <button class="info-btn absolute top-2 right-2 w-8 h-8 bg-white/90 hover:bg-white rounded-full shadow-lg flex items-center justify-center transition-all duration-200 hover:scale-110 z-20">
            <svg class="w-4 h-4 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </button>
          <!-- Selection Overlay -->
          <div class="absolute inset-0 bg-primary/20 opacity-0 transition-opacity duration-300 selection-overlay"></div>
          <!-- Checkmark for selected state -->
          <div class="absolute top-2 left-2 w-8 h-8 bg-primary rounded-full opacity-0 transition-all duration-300 flex items-center justify-center selection-check">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
        </div>
        <div class="p-4 transition-all duration-300">
          <input type="radio" name="parquet" value="${item.typeName}" class="hidden" />
          <h3 class="font-semibold text-gray-800 text-sm mb-2 group-hover:text-primary transition-colors">${item.typeName}</h3>
          <p class="text-primary font-bold text-sm group-hover:text-primary-dark transition-colors">החל מ-${item.pricePerUnit} ש"ח למ"ר</p>
        </div>
      </div>
    `;
    container.innerHTML += itemHtml;
  });
  
  // Add stagger animation
  const cards = container.querySelectorAll('.parquet-card');
  cards.forEach((card, index) => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(20px)';
    setTimeout(() => {
      card.style.opacity = '1';
      card.style.transform = 'translateY(0)';
    }, index * 100);
  });
  
  // הוסף event listeners לכרטיסים
  cards.forEach(card => {
    card.addEventListener('click', function(e) {
      // אם הלחיצה הייתה על כפתור המידע, אל תבחר את הכרטיס
      if (e.target.closest('.info-btn')) return;
      
      selectParquetCard(this, e);
    });
  });
  
  // הוסף event listeners לכפתורי המידע
  const infoButtons = container.querySelectorAll('.info-btn');
  
  infoButtons.forEach(btn => {
    btn.addEventListener('click', function(e) {
      e.stopPropagation();
      
      // מציאת הכרטיס שבו נמצא הכפתור
      const card = this.closest('.parquet-card');
      if (card && card.dataset.parquet) {
        try {
          const item = JSON.parse(card.dataset.parquet);
          showInfoForItem(item.typeName, item.description, item.imageURL);
        } catch (error) {
          // Error handling silently
        }
      }
    });
  });
}

function getImagePath(imageURL, typeName) {
  // אם אין URL או שהוא לא תקין, השתמש במיפוי לפי השם
  if (!imageURL || imageURL === 'null' || imageURL === '' || imageURL === 'string' || imageURL.includes('example.com')) {
    return getImageByTypeName(typeName);
  }
  
  // אם זה URL מלא, החזר אותו
  if (imageURL.includes('http') || imageURL.includes('/')) {
    return imageURL;
  }
  
  // אם זה רק שם קובץ, הוסף את הנתיב המלא
  return `../picturs/parquetsTypes/${imageURL}`;
}

function getImageByTypeName(typeName) {
  if (!typeName) return '../picturs/logo.png';
  
  const lowerName = typeName.toLowerCase();
  
  const imageMap = {
    'אלון': 'etzMaleAlonTivi.jpg',
    'אגוז': 'etzMaleEgozTivi.jpg', 
    'אורן': 'etzMalegoshniTivi.jpg',
    'עץ מלא אלון': 'etzMaleAlonTivi.jpg',
    'עץ מלא אגוז': 'etzMaleEgozTivi.jpg',
    'עץ מלא אורן': 'etzMalegoshniTivi.jpg'
  };
  
  for (const [key, filename] of Object.entries(imageMap)) {
    if (lowerName.includes(key.toLowerCase())) {
      return `../picturs/parquetsTypes/${filename}`;
    }
  }
  
  if (lowerName.includes('עץ') || lowerName.includes('אלון')) {
    return '../picturs/parquetsTypes/etzMaleAlonTivi.jpg';
  }
  
  return '../picturs/logo.png';
}

function selectParquetCard(cardElement, event) {
  const radio = cardElement.querySelector('input[type="radio"]');
  const allCards = document.querySelectorAll('.parquet-card');
  
  // הסר בחירה מכל הכרטיסים
  allCards.forEach(card => {
    card.classList.remove('ring-2', 'ring-primary', 'ring-opacity-50', 'border-primary', 'scale-105', 'selected');
    const overlay = card.querySelector('.selection-overlay');
    const check = card.querySelector('.selection-check');
    if (overlay) overlay.style.opacity = '0';
    if (check) {
      check.style.opacity = '0';
      check.style.transform = 'scale(0.8)';
    }
  });
  
  // בחר את הכרטיס הנוכחי
  radio.checked = true;
  cardElement.classList.add('ring-2', 'ring-primary', 'ring-opacity-50', 'border-primary', 'scale-105', 'selected');
  
  // הוסף אפקטים עיצוביים
  const overlay = cardElement.querySelector('.selection-overlay');
  const check = cardElement.querySelector('.selection-check');
  
  if (overlay) overlay.style.opacity = '1';
  if (check) {
    check.style.opacity = '1';
    check.style.transform = 'scale(1)';
  }
  
  // הוסף רטט קל לאפקט
  cardElement.style.transform = 'scale(1.08)';
  setTimeout(() => {
    cardElement.style.transform = 'scale(1.05)';
  }, 150);
  
  showSuccessToast(`${radio.value} נבחר בהצלחה!`);
}

function updateSelection(radio) {}

// הצגת מידע על פרקט ספציפי
function showInfoForItem(name, description, imageURL) {
  const modal = document.getElementById("info-modal");
  const content = document.getElementById("modal-content");
  
  if (!modal) {
    return;
  }
  
  const titleEl = document.getElementById("info-title");
  const descEl = document.getElementById("info-description");
  const imageEl = document.getElementById("info-image");
  
  if (titleEl) titleEl.textContent = name;
  if (descEl) descEl.textContent = description || "פרקט איכותי ומתקדם המתאים לכל סוגי החללים. עמיד ויפה לאורך שנים.";
  
  if (imageEl) {
    const imagePath = getImagePath(imageURL, name);
    imageEl.src = imagePath;
    imageEl.onerror = function() {
      this.src = '../picturs/logo.png';
    };
  }
  
  modal.classList.remove("hidden");
  
  if (content) {
    setTimeout(() => {
      content.classList.remove("scale-95", "opacity-0");
      content.classList.add("scale-100", "opacity-100");
    }, 50);
  }
}

function showInfo(item) {
  const modal = document.getElementById("info-modal");
  const content = document.getElementById("modal-content");
  
  document.getElementById("info-title").textContent = item.typeName;
  document.getElementById("info-description").textContent = item.description || "פרקט איכותי ומתקדם המתאים לכל סוגי החללים. עמיד ויפה לאורך שנים.";
  
  const imagePath = getImagePath(item.imageURL, item.typeName);
  const infoImage = document.getElementById("info-image");
  infoImage.src = imagePath;
  infoImage.onerror = function() {
    this.src = '../picturs/logo.png';
  };
  
  modal.classList.remove("hidden");
  setTimeout(() => {
    content.classList.remove("scale-95", "opacity-0");
    content.classList.add("scale-100", "opacity-100");
  }, 50);
}

function closeInfoModal() {
  const modal = document.getElementById("info-modal");
  const content = document.getElementById("modal-content");
  
  content.classList.remove("scale-100", "opacity-100");
  content.classList.add("scale-95", "opacity-0");
  
  setTimeout(() => {
    modal.classList.add("hidden");
  }, 300);
}

function validateSelection() {
  const selected = document.querySelector('input[name="parquet"]:checked')?.value;
  if (!selected) {
    showErrorMessage("אנא בחרו פרקט לפני מעבר לשלב הבא");
    
    // Shake animation for emphasis
    const button = event.target;
    button.classList.add('animate-pulse');
    setTimeout(() => button.classList.remove('animate-pulse'), 1000);
    
    return;
  }
  
  // Show loading state
  const button = event.target;
  const originalText = button.innerHTML;
  button.innerHTML = `
    <svg class="animate-spin w-5 h-5 mx-auto" fill="none" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
  `;
  button.disabled = true;
  
  localStorage.setItem("ParquetType", selected);
  
  // Simulate loading and redirect
  setTimeout(() => {
    window.location.href = "InstallDetailsPage.html";
  }, 800);
}

function showSuccessToast(message) {
  // Create toast notification
  const toast = document.createElement('div');
  toast.className = 'fixed top-20 left-1/2 transform -translate-x-1/2 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg z-50 transition-all duration-300 translate-y-[-100px] opacity-0';
  toast.textContent = message;
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

function showErrorMessage(message) {
  // Create error toast
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

function goBack() {
  window.history.back();
}

$(document).ready(function () {
  fetchParquetTypes();
  
  setTimeout(() => {
    const firstTab = document.querySelector('.tab[data-index="0"]');
    const indicator = document.querySelector('.tab-indicator');
    
    if (firstTab && indicator) {
      firstTab.classList.add('text-white', 'active');
      firstTab.classList.remove('text-gray-600');
      indicator.style.right = '0%';
    }
  }, 200);
  
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
});
