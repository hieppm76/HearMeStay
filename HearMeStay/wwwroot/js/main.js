// Navbar scroll effect
window.addEventListener('scroll', () => {
  const nav = document.querySelector('.navbar');
  if (nav) nav.classList.toggle('scrolled', window.scrollY > 20);
});

// Intersection Observer for animations
const observer = new IntersectionObserver((entries) => {
  entries.forEach((entry, i) => {
    if (entry.isIntersecting) {
      setTimeout(() => {
        entry.target.style.opacity = '1';
        entry.target.style.transform = 'translateY(0)';
      }, i * 80);
    }
  });
}, { threshold: 0.1 });

document.querySelectorAll('.card, .step-card, .feature-card, .testimonial-card').forEach(el => {
  el.style.opacity = '0';
  el.style.transform = 'translateY(24px)';
  el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
  observer.observe(el);
});

// Price range display
const priceRange = document.getElementById('priceRange');
const priceDisplay = document.getElementById('priceDisplay');
if (priceRange && priceDisplay) {
  priceRange.addEventListener('input', () => {
    priceDisplay.textContent = parseInt(priceRange.value).toLocaleString('vi-VN') + ' đ';
  });
}

// AI Preference Form tag preview
function updateTags() {
  const tagContainer = document.getElementById('aiTagPreview');
  if (!tagContainer) return;
  const tags = [];
  document.querySelectorAll('.pref-checkbox:checked').forEach(cb => {
    tags.push(`<span class="tag tag-${cb.dataset.type}">${cb.dataset.label}</span>`);
  });
  const freeText = document.getElementById('freeText');
  if (freeText && freeText.value.trim()) {
    tags.push(`<span class="tag tag-room">✨ AI đang phân tích...</span>`);
  }
  tagContainer.innerHTML = tags.length ? tags.join(' ') : '<span style="color:var(--gray);font-size:0.85rem;">Chọn nhu cầu để xem tags AI tạo ra</span>';
}

document.querySelectorAll('.pref-checkbox').forEach(cb => {
  cb.addEventListener('change', updateTags);
});
const freeText = document.getElementById('freeText');
if (freeText) freeText.addEventListener('input', updateTags);

// Search redirect
const searchBtn = document.getElementById('searchBtn');
if (searchBtn) {
  searchBtn.addEventListener('click', () => {
    window.location.href = 'search.html';
  });
}

// Booking form submit
const bookingForm = document.getElementById('bookingForm');
if (bookingForm) {
  bookingForm.addEventListener('submit', (e) => {
    e.preventDefault();
    window.location.href = 'preference-form.html';
  });
}

// Preference form submit
const prefForm = document.getElementById('prefForm');
if (prefForm) {
  prefForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const success = document.getElementById('successMsg');
    if (success) { success.style.display = 'block'; window.scrollTo({ top: 0, behavior: 'smooth' }); }
  });
}

// Smooth scroll
document.querySelectorAll('a[href^="#"]').forEach(a => {
  a.addEventListener('click', e => {
    e.preventDefault();
    const target = document.querySelector(a.getAttribute('href'));
    if (target) target.scrollIntoView({ behavior: 'smooth' });
  });
});
