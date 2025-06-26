let parquetData = {
  solid: [],
  laminate: [],
  fishbone: [],
};

function fetchParquetTypes() {
  ajaxCall(
    "GET",
    `${API_BASE_URL}/ParquetType/GetAll`,
    null,
    function (types) {
      console.log("תוצאה מהשרת:", types);

      types.forEach((type) => {
        if (type.typeName.includes("עץ")) parquetData.solid.push(type);
        else if (type.typeName.includes("למינציה"))
          parquetData.laminate.push(type);
        else if (type.typeName.includes("פישבון"))
          parquetData.fishbone.push(type);
      });

      // טען את הטאב הראשון
      loadParquetItems('solid');
    },
    function (xhr, status, error) {
      console.error("שגיאה בטעינת סוגי פרקטים:", error);
      showErrorMessage("אירעה שגיאה בטעינת סוגי הפרקטים מהשרת.");
    }
  );
}

function selectTab(tabEl, type) {
  const tabs = document.querySelectorAll('.tab');
  const indicator = document.querySelector('.tab-indicator');
  const index = parseInt(tabEl.dataset.index);
  
  // Update active states
  tabs.forEach(tab => {
    tab.classList.remove('text-white');
    tab.classList.add('text-gray-600');
  });
  tabEl.classList.remove('text-gray-600');
  tabEl.classList.add('text-white');
  
  // Move indicator with smooth animation
  const percentage = 33.33;
  indicator.style.right = `${(2 - index) * percentage}%`;
  
  // Update title
  document.getElementById("parquet-title").textContent = "פרקטי " + tabEl.textContent;
  
  // Load parquet items with fade effect
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
    const itemHtml = `
      <div class="parquet-card bg-white rounded-xl shadow-sm border border-orange-100 overflow-hidden transform transition-all duration-300 hover:shadow-lg" style="animation-delay: ${index * 100}ms">
        <div class="relative">
          <img src="../picturs/parquetsTypes/${item.imageURL}" alt="${item.typeName}" class="w-full h-32 object-cover" />
          <button onclick='showInfo(${JSON.stringify(item).replace(/'/g, "\\'")})'
                  class="absolute top-2 right-2 w-8 h-8 bg-white/90 hover:bg-white rounded-full shadow-lg flex items-center justify-center transition-colors">
            <svg class="w-4 h-4 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </button>
        </div>
        <div class="p-4">
          <h3 class="font-semibold text-gray-800 text-sm mb-2">${item.typeName}</h3>
          <p class="text-primary font-bold text-sm mb-3">החל מ-${item.pricePerUnit} ש"ח למ"ר</p>
          <label class="flex items-center justify-center cursor-pointer group">
            <input type="radio" name="parquet" value="${item.typeName}" class="sr-only peer" onchange="updateSelection(this)" />
            <div class="w-5 h-5 border-2 border-gray-300 rounded-full peer-checked:border-primary peer-checked:bg-primary flex items-center justify-center transition-all duration-200 group-hover:border-primary-light">
              <div class="w-2 h-2 bg-white rounded-full opacity-0 peer-checked:opacity-100 transition-opacity"></div>
            </div>
            <span class="mr-2 text-sm text-gray-600 group-hover:text-primary transition-colors">בחר</span>
          </label>
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
}

function updateSelection(radio) {
  // Add visual feedback when selection changes
  const allCards = document.querySelectorAll('.parquet-card');
  allCards.forEach(card => {
    card.classList.remove('ring-2', 'ring-primary', 'ring-opacity-50');
  });
  
  if (radio.checked) {
    const selectedCard = radio.closest('.parquet-card');
    selectedCard.classList.add('ring-2', 'ring-primary', 'ring-opacity-50');
    
    // Show success feedback
    showSuccessToast('פרקט נבחר בהצלחה!');
  }
}

function showInfo(item) {
  const modal = document.getElementById("info-modal");
  const content = document.getElementById("modal-content");
  
  document.getElementById("info-title").textContent = item.typeName;
  document.getElementById("info-description").textContent = item.description || "פרקט איכותי ומתקדם המתאים לכל סוגי החללים. עמיד ויפה לאורך שנים.";
  document.getElementById("info-image").src = `../picturs/parquetsTypes/${item.imageURL}`;
  
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

// Initialize when page loads
$(document).ready(function () {
  fetchParquetTypes();
  
  // Add loading animation to page
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
});
