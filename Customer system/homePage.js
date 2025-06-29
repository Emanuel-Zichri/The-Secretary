// homePage.js - עמוד הבית עם טעינת פרטי עסק דינמית

// משתנים גלובליים לפרטי העסק
let businessInfo = {
  businessName: 'דוד פרקטים',
  businessPhone: '050-1234567',
  businessEmail: 'info@davidparquet.co.il',
  workingHours: 'א׳-ה׳: 8:00-18:00 | ו׳: 8:00-14:00',
  aboutUs: 'אנחנו דוד פרקטים - חברה מובילה בתחום התקנת פרקט עם יותר מ-15 שנות ניסיון.',
  businessLogo: '../picturs/logo.png',
  introVideoURL: null
};

document.addEventListener('DOMContentLoaded', function() {
  // Add smooth scroll behavior
  document.documentElement.style.scrollBehavior = 'smooth';
  
  // Initialize page animation
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);

  // טעינת פרטי העסק מהשרת
  loadBusinessInfo();
});

/**
 * טעינת פרטי העסק מה-API ועדכון הדף בהתאם
 */
function loadBusinessInfo() {
  console.log('🏢 טוען פרטי עסק...');
  
  ajaxCall('GET', `${API_BASE_URL}/User/GetBusinessInfo`, null,
    function(data) {
      console.log('✅ פרטי עסק נטענו:', data);
      
      // עדכון המשתנה הגלובלי
      businessInfo = {
        businessName: data.businessName || businessInfo.businessName,
        businessPhone: data.businessPhone || businessInfo.businessPhone,
        businessEmail: data.businessEmail || businessInfo.businessEmail,
        workingHours: data.workingHours || businessInfo.workingHours,
        aboutUs: data.aboutUs || businessInfo.aboutUs,
        businessLogo: data.businessLogo || businessInfo.businessLogo,
        introVideoURL: data.introVideoURL || businessInfo.introVideoURL
      };

      // עדכון התוכן בדף
      updatePageContent();
    },
    function(error) {
      console.warn('⚠️ לא ניתן לטעון פרטי עסק מהשרת, נשתמש בנתונים דיפולטיים:', error);
      // עדכון התוכן עם הנתונים הדיפולטיים
      updatePageContent();
    }
  );
}

/**
 * עדכון התוכן בדף בהתאם לפרטי העסק
 */
function updatePageContent() {
  console.log('🔄 מעדכן תוכן דף עם פרטי עסק:', businessInfo);

  try {
    // עדכון כותרות ושמות
    const businessNameElements = document.querySelectorAll('[data-business-name]');
    businessNameElements.forEach(el => {
      el.textContent = businessInfo.businessName;
    });

    // עדכון לוגו
    const logoElements = document.querySelectorAll('[data-business-logo]');
    logoElements.forEach(el => {
      el.src = businessInfo.businessLogo;
      el.alt = businessInfo.businessName;
    });

    // עדכון טלפון
    const phoneElements = document.querySelectorAll('[data-business-phone]');
    phoneElements.forEach(el => {
      if (el.tagName === 'A') {
        el.href = `tel:${businessInfo.businessPhone}`;
      } else {
        el.textContent = businessInfo.businessPhone;
      }
    });

    // עדכון שעות פעילות
    const hoursElements = document.querySelectorAll('[data-working-hours]');
    hoursElements.forEach(el => {
      el.textContent = businessInfo.workingHours;
    });

    // עדכון אודות העסק
    const aboutElements = document.querySelectorAll('[data-about-us]');
    aboutElements.forEach(el => {
      el.textContent = businessInfo.aboutUs;
    });

    // טיפול בסרטון הכירות
    updateIntroVideo();

    console.log('✅ תוכן הדף עודכן בהצלחה');
  } catch (error) {
    console.error('❌ שגיאה בעדכון תוכן הדף:', error);
  }
}

/**
 * עדכון סרטון ההכירות
 */
function updateIntroVideo() {
  const videoSection = document.querySelector('[data-intro-video]');
  if (!videoSection) return;

  if (businessInfo.introVideoURL) {
    // יש סרטון - הצגת נגן
    videoSection.innerHTML = `
      <div class="bg-gray-100 rounded-2xl overflow-hidden">
        <div class="aspect-video">
          <iframe 
            src="${getEmbedURL(businessInfo.introVideoURL)}"
            frameborder="0" 
            allowfullscreen
            class="w-full h-full">
          </iframe>
        </div>
        <div class="p-4 text-center">
          <h3 class="text-lg font-semibold text-gray-800 mb-1">סרטון הכירות</h3>
          <p class="text-gray-600 text-sm">באו להכיר אותנו ואת העבודה המקצועית שלנו</p>
        </div>
      </div>
    `;
  } else {
    // אין סרטון - הצגת placeholder
    videoSection.innerHTML = `
      <div class="bg-gray-100 rounded-2xl p-6 text-center">
        <div class="bg-primary/10 rounded-xl p-8 mb-4">
          <svg class="w-16 h-16 text-primary mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1m-6 4h8m2-10v.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
          </svg>
          <h3 class="text-xl font-semibold text-gray-800 mb-2">סרטון הכירות</h3>
          <p class="text-gray-600 text-sm">באו להכיר אותנו ואת העבודה המקצועית שלנו</p>
        </div>
      </div>
    `;
  }
}

/**
 * המרת URL רגיל של YouTube ל-embed URL
 */
function getEmbedURL(url) {
  if (url.includes('youtube.com/watch')) {
    const videoId = url.split('v=')[1]?.split('&')[0];
    return `https://www.youtube.com/embed/${videoId}`;
  } else if (url.includes('youtu.be/')) {
    const videoId = url.split('youtu.be/')[1]?.split('?')[0];
    return `https://www.youtube.com/embed/${videoId}`;
  }
  return url; // אם זה כבר embed URL או סוג אחר
}

function setupScrollAnimations() {
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.style.opacity = '1';
        entry.target.style.transform = 'translateY(0)';
      }
    });
  }, {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
  });

  const sections = document.querySelectorAll('section');
  sections.forEach(section => {
    section.style.opacity = '0';
    section.style.transform = 'translateY(30px)';
    section.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    observer.observe(section);
  });
}
