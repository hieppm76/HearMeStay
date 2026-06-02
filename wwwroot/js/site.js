/* HearMeStay — Animations */
document.addEventListener('DOMContentLoaded', () => {
  // Character-by-character heading animation
  document.querySelectorAll('.char-animate').forEach(el => {
    const text = el.textContent;
    el.innerHTML = '';
    let delay = 0;
    for (const line of text.split('\n')) {
      for (const char of line) {
        const span = document.createElement('span');
        span.textContent = char === ' ' ? '\u00A0' : char;
        span.style.animationDelay = `${delay}s`;
        el.appendChild(span);
        delay += 0.03;
      }
      el.appendChild(document.createElement('br'));
    }
  });

  // Fade-up on scroll
  const obs = new IntersectionObserver((entries) => {
    entries.forEach(e => { if (e.isIntersecting) { e.target.classList.add('visible'); obs.unobserve(e.target); } });
  }, { threshold: 0.1 });
  document.querySelectorAll('.fade-up').forEach(el => obs.observe(el));
});
