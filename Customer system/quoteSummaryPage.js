// quoteSummaryPage.js - עמוד הצעת מחיר מתקדם

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    // Add page load animation
    document.body.style.opacity = '0';
    setTimeout(() => {
        document.body.style.transition = 'opacity 0.5s ease';
        document.body.style.opacity = '1';
    }, 100);
    
    // Load quote data
    loadQuote();
});

function loadQuote() {
    const data = JSON.parse(localStorage.getItem('installationData')) || {};
    const { customerDetails = {}, spaceDetails = [] } = data;
    const parquetType = localStorage.getItem('ParquetType') || '';

    console.log('📊 טוען נתוני הצעת מחיר:', { data, parquetType });

    // חישוב שטח כולל
    let totalSize = 0;
    spaceDetails.forEach(s => totalSize += Number(s.size));

    if (totalSize === 0 || !parquetType) {
        console.warn('⚠️ נתונים חסרים, משתמש ב-fallback');
        showFallbackEstimate(spaceDetails.length, parquetType);
        return;
    }

    // הצגת loading state מתקדם
    showLoadingState();

    // קריאה לAPI החדש
    const requestData = {
        totalArea: totalSize,
        parquetType: parquetType,
        roomCount: spaceDetails.length || 1
    };

    console.log('🚀 שולח בקשה ל-API:', requestData);

    ajaxCall(
        'POST',
        `${API_BASE_URL}/PriceEstimator/QuickEstimate`,
        JSON.stringify(requestData),
        function(response) {
            console.log('✅ תגובה מ-API:', response);
            displayEstimateResults(response, totalSize, parquetType, spaceDetails);
        },
        function(xhr, status, error) {
            console.error('❌ שגיאה בחישוב הערכה:', error, xhr.responseText);
            showFallbackEstimate(spaceDetails.length, parquetType, totalSize);
        }
    );
}

function showLoadingState() {
    // הצגת skeleton loading
    const summaryList = document.getElementById('summary-list');
    summaryList.innerHTML = `
        <div class="summary-item bg-gray-100 rounded-xl p-4 shimmer">
            <div class="h-4 bg-gray-200 rounded w-3/4"></div>
        </div>
        <div class="summary-item bg-gray-100 rounded-xl p-4 shimmer">
            <div class="h-4 bg-gray-200 rounded w-1/2"></div>
        </div>
        <div class="summary-item bg-gray-100 rounded-xl p-4 shimmer">
            <div class="h-4 bg-gray-200 rounded w-2/3"></div>
        </div>
    `;

    // הצגת loading במחיר
    document.getElementById('priceRange').innerHTML = `
        <div class="shimmer bg-white/20 rounded-lg h-12 w-48 mx-auto flex items-center justify-center">
            <span class="text-white/60">מחשב...</span>
        </div>
    `;

    // הצגת loading בזמן
    document.getElementById('durationRange').innerHTML = `
        <div class="shimmer bg-white/20 rounded-lg h-6 w-32 mx-auto flex items-center justify-center">
            <span class="text-white/60 text-sm">מחשב...</span>
        </div>
    `;
}

function displayEstimateResults(response, totalSize, parquetType, spaceDetails) {
    console.log('🎨 מציג תוצאות:', response);

    // עדכון רשימת הסיכום עם אנימציות
    const summaryItems = [
        `${totalSize.toFixed(1)} מ"ר פרקט ${parquetType} - כולל התקנה`,
        `${spaceDetails.length} חללים${spaceDetails.length > 1 ? ' (חלקם עשויים להיות קטנים)' : ''}`
    ];

    // הוספת פירוט נוסף אם יש
    const roomTypes = [...new Set(spaceDetails.map(s => s.floorType).filter(Boolean))];
    if (roomTypes.length > 0 && roomTypes.length <= 3) {
        summaryItems.push(`סוגי חללים: ${roomTypes.join(', ')}`);
    }

    // הוספת מידע על מורכבות אם יש
    if (response.complexityMultiplier && response.complexityMultiplier > 1.1) {
        summaryItems.push('פרויקט מורכב - דורש התאמות מיוחדות');
    }

    // הצגת הפריטים עם אנימציה
    const summaryList = document.getElementById('summary-list');
    summaryList.innerHTML = '';
    
    summaryItems.forEach((item, index) => {
        setTimeout(() => {
            const li = document.createElement('div');
            li.className = 'summary-item bg-gray-50 rounded-xl p-4 border-r-4 border-primary hover:bg-gray-100 transition-all duration-300 opacity-0 transform translate-y-4';
            li.innerHTML = `
                <div class="flex items-center">
                    <svg class="w-4 h-4 text-primary ml-3 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <span class="text-gray-700 font-medium">${item}</span>
                </div>
            `;
            summaryList.appendChild(li);
            
            // אנימציית כניסה
            setTimeout(() => {
                li.style.transition = 'all 0.5s ease';
                li.classList.remove('opacity-0', 'translate-y-4');
                li.classList.add('opacity-100', 'translate-y-0');
            }, 50);
        }, index * 200);
    });

    // הצגת מחיר עם אנימציה
    setTimeout(() => {
        const priceText = formatPrice(response.minPrice, response.maxPrice);
        const priceElement = document.getElementById('priceRange');
        priceElement.style.transition = 'all 0.5s ease';
        priceElement.innerHTML = priceText;
        priceElement.classList.remove('pulse-animation');
    }, 800);

    // הצגת זמן עם אנימציה
    setTimeout(() => {
        const durationText = formatDuration(response.minDays, response.maxDays);
        const durationElement = document.getElementById('durationRange');
        durationElement.style.transition = 'all 0.5s ease';
        durationElement.innerHTML = durationText;
    }, 1000);

    // הצגת הודעת הצלחה
    setTimeout(() => {
        showSuccessToast('הערכת המחיר חושבה בהצלחה!');
    }, 1200);
}

function formatPrice(minPrice, maxPrice) {
    const min = Math.round(minPrice);
    const max = Math.round(maxPrice);
    
    if (min === max) {
        return `<span class="text-4xl font-bold">${min.toLocaleString()}</span> <span class="text-xl">ש"ח</span>`;
    } else {
        return `<span class="text-3xl font-bold">${min.toLocaleString()}</span> <span class="text-lg">-</span> <span class="text-3xl font-bold">${max.toLocaleString()}</span> <span class="text-xl">ש"ח</span>`;
    }
}

function formatDuration(minDays, maxDays) {
    if (minDays === maxDays) {
        return `${minDays} ימי עבודה`;
    } else {
        return `${minDays} - ${maxDays} ימי עבודה`;
    }
}

function showFallbackEstimate(roomCount, parquetType, totalSize = null) {
    console.warn('⚠️ משתמש בחישוב fallback');
    
    // חישוב בסיסי אם API לא עובד
    let pricePerM = 55; // ברירת מחדל
    
    if (parquetType.includes('עץ מלא') || parquetType.includes('אלון') || parquetType.includes('אגוז')) {
        pricePerM = 80;
    } else if (parquetType.includes('פישבון')) {
        pricePerM = 95;
    } else if (parquetType.includes('עץ') || parquetType.includes('הנדסי')) {
        pricePerM = 70;
    } else if (parquetType.includes('למינציה')) {
        pricePerM = 45;
    } else if (parquetType.includes('ויניל')) {
        pricePerM = 40;
    }

    const summaryList = document.getElementById('summary-list');
    const priceElement = document.getElementById('priceRange');
    const durationElement = document.getElementById('durationRange');

    if (totalSize) {
        const estimatedPrice = totalSize * pricePerM;
        const low = Math.round(estimatedPrice * 0.85);
        const high = Math.round(estimatedPrice * 1.15);
        const days = Math.ceil(totalSize / 18);

        // עדכון סיכום
        summaryList.innerHTML = `
            <div class="summary-item bg-gray-50 rounded-xl p-4 border-r-4 border-primary">
                <div class="flex items-center">
                    <svg class="w-4 h-4 text-primary ml-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <span>${totalSize.toFixed(1)} מ"ר פרקט ${parquetType} - כולל התקנה</span>
                </div>
            </div>
            <div class="summary-item bg-gray-50 rounded-xl p-4 border-r-4 border-primary">
                <div class="flex items-center">
                    <svg class="w-4 h-4 text-primary ml-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <span>${roomCount} חללים</span>
                </div>
            </div>
        `;
        
        priceElement.innerHTML = formatPrice(low, high);
        durationElement.innerHTML = formatDuration(Math.max(1, days - 1), days + 1);
    } else {
        // אם אין נתונים כלל
        summaryList.innerHTML = `
            <div class="summary-item bg-gray-50 rounded-xl p-4 border-r-4 border-primary">
                <div class="flex items-center">
                    <svg class="w-4 h-4 text-primary ml-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <span>פרקט ${parquetType} - כולל התקנה</span>
                </div>
            </div>
            <div class="summary-item bg-gray-50 rounded-xl p-4 border-r-4 border-primary">
                <div class="flex items-center">
                    <svg class="w-4 h-4 text-primary ml-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <span>${roomCount} חללים</span>
                </div>
            </div>
        `;
        
        priceElement.innerHTML = `<span class="text-3xl font-bold">החל מ-${pricePerM}</span> <span class="text-xl">ש"ח למ"ר</span>`;
        durationElement.innerHTML = 'יחושב לפי שטח סופי';
    }
    
    // הסרת אנימציות loading
    priceElement.classList.remove('pulse-animation');
    
    showWarningToast('הערכה בסיסית - המחיר הסופי יחושב לפי מדידה מדויקת');
}

function goNext() {
    // הוספת אפקט loading לכפתור
    const button = document.getElementById('continue-btn');
    const originalContent = button.innerHTML;
    
    button.innerHTML = `
        <svg class="animate-spin w-6 h-6 mx-auto" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
    `;
    button.disabled = true;
    
    setTimeout(() => {
        window.location.href = 'InstallDateSelection.html';
    }, 500);
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
    }, 3000);
}

function showWarningToast(message) {
    const toast = document.createElement('div');
    toast.className = 'fixed top-20 left-1/2 transform -translate-x-1/2 bg-amber-500 text-white px-6 py-3 rounded-lg shadow-lg z-50 transition-all duration-300 translate-y-[-100px] opacity-0';
    toast.innerHTML = `
        <div class="flex items-center">
            <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-2.694-.833-3.464 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
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