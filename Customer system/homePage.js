// homePage.js - עמוד הבית מתקדם

// Initialize page animations and interactions
document.addEventListener('DOMContentLoaded', function() {
  // Add smooth scroll behavior
  document.documentElement.style.scrollBehavior = 'smooth';
  
  // Initialize animations
  initializeAnimations();
  
  // Add intersection observer for scroll animations
  setupScrollAnimations();
});

function initializeAnimations() {
  // Add fade-in animation to page load
  document.body.style.opacity = '0';
  setTimeout(() => {
    document.body.style.transition = 'opacity 0.5s ease';
    document.body.style.opacity = '1';
  }, 100);
}

function setupScrollAnimations() {
  // Create intersection observer for scroll-triggered animations
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

  // Observe sections for scroll animations
  const sections = document.querySelectorAll('section');
  sections.forEach(section => {
    section.style.opacity = '0';
    section.style.transform = 'translateY(30px)';
    section.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    observer.observe(section);
  });
}

// Enhanced button interactions
function addButtonEffects() {
  const buttons = document.querySelectorAll('button');
  buttons.forEach(button => {
    button.addEventListener('mousedown', function() {
      this.style.transform = 'scale(0.98)';
    });
    
    button.addEventListener('mouseup', function() {
      this.style.transform = 'scale(1)';
    });
    
    button.addEventListener('mouseleave', function() {
      this.style.transform = 'scale(1)';
    });
  });
}

// Call button effects after DOM is loaded
document.addEventListener('DOMContentLoaded', addButtonEffects);

// Smooth navigation function
function navigateToParquetSelection() {
  // Add loading state
  const button = event.target.closest('button');
  const originalContent = button.innerHTML;
  
  button.innerHTML = `
    <svg class="animate-spin w-5 h-5 mx-auto" fill="none" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
  `;
  button.disabled = true;
  
  // Navigate after short delay
  setTimeout(() => {
    window.location.href = 'selectParquetPage.html';
  }, 800);
}

// Add parallax effect to hero section
function addParallaxEffect() {
  window.addEventListener('scroll', () => {
    const scrolled = window.pageYOffset;
    const parallaxElements = document.querySelectorAll('.floating');
    
    parallaxElements.forEach((element, index) => {
      const speed = 0.5 + (index * 0.1);
      element.style.transform = `translateY(${scrolled * speed}px)`;
    });
  });
}

// Initialize parallax effect
document.addEventListener('DOMContentLoaded', addParallaxEffect);
