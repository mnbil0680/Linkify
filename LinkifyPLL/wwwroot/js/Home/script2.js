
// Initialize the application
document.addEventListener('DOMContentLoaded', function () {
    initializeApp();
});

function initializeApp() {
    setupTheme();
    setupSearch();
    setupDropdowns();
    setupPostInteractions();
    setupModals();
    setupInfiniteScroll();
    setupMobileOptimizations();
    setupAccessibility();
    setupNotifications();
    hideLoadingScreen();
}

function setupTheme() {
    const themeToggle = document.getElementById('themeToggle');
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);

    themeToggle?.addEventListener('click', () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        showToast(`Switched to ${newTheme} mode`, 'info');
    });
}

function hideLoadingScreen() {
    const loadingScreen = document.getElementById('loadingScreen');
    if (!loadingScreen) return;

    loadingScreen.style.transition = 'opacity 0.3s ease-out';
    loadingScreen.style.opacity = '0';

    setTimeout(() => {
        loadingScreen.style.display = 'none';
    }, 300);
}



///////////////////////////////////////////////////////////////////
        // Theme Toggle
        const themeToggle = document.getElementById('themeToggle');
        const currentTheme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-theme', currentTheme);

        themeToggle.addEventListener('click', () => {
            const theme = document.documentElement.getAttribute('data-theme');
            const newTheme = theme === 'light' ? 'dark' : 'light';
            document.documentElement.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
        });

        // Loading Screen
        window.addEventListener('load', () => {
            setTimeout(() => {
                document.getElementById('loadingScreen').style.opacity = '0';
                setTimeout(() => {
                    document.getElementById('loadingScreen').style.display = 'none';
                }, 100);
            }, 100);
        });

        // Search Functionality
        const searchBox = document.getElementById('main-search');
        const searchSuggestions = document.getElementById('search-suggestions');

        searchBox.addEventListener('focus', () => {
            searchSuggestions.style.display = 'block';
            searchSuggestions.setAttribute('aria-hidden', 'false');
        });

        searchBox.addEventListener('blur', () => {
            setTimeout(() => {
                searchSuggestions.style.display = 'none';
                searchSuggestions.setAttribute('aria-hidden', 'true');
            }, 200);
        });

        // Dropdown Menus
        document.querySelectorAll('.dropdown').forEach(dropdown => {
            const button = dropdown.querySelector('button, .dropdown-toggle');
            const content = dropdown.querySelector('.dropdown-content');
            
            if (button && content) {
                button.addEventListener('click', (e) => {
                    e.stopPropagation();
                    dropdown.classList.toggle('active');
                });
            }
        });

        // Close dropdowns when clicking outside
        document.addEventListener('click', (e) => {
            document.querySelectorAll('.dropdown.active').forEach(dropdown => {
                if (!dropdown.contains(e.target)) {
                    dropdown.classList.remove('active');
                }
            });
        });

        // Post Actions
        document.querySelectorAll('.like-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                this.classList.toggle('liked');
                const icon = this.querySelector('i');
                if (this.classList.contains('liked')) {
                    icon.className = 'fas fa-thumbs-up';
                    this.style.color = 'var(--primary-blue)';
                } else {
                    icon.className = 'far fa-thumbs-up';
                    this.style.color = '';
                }
            });
        });

        // Comment Input
        document.querySelectorAll('.comment-input').forEach(input => {
            const submitBtn = input.nextElementSibling;
            input.addEventListener('input', () => {
                submitBtn.disabled = input.value.trim() === '';
            });
        });

        // Modal Functions
        function openModal(modalId) {
            const modal = document.getElementById(modalId);
            modal.classList.add('active');
            document.body.style.overflow = 'hidden';
        }

        function closeModal(modalId) {
            const modal = document.getElementById(modalId);
            modal.classList.remove('active');
            document.body.style.overflow = '';
        }

        function triggerMediaUpload() {
            document.getElementById('mediaUpload').click();
        }

        function publishPost() {
            const content = document.querySelector('.post-composer').value;
            if (content.trim()) {
                showToast('Post published successfully!', 'success');
                closeModal('createPostModal');
                document.querySelector('.post-composer').value = '';
            } else {
                showToast('Please write something before posting.', 'error');
            }
        }

        // Toast Notifications
        function showToast(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `toast toast-${type}`;
            toast.innerHTML = `
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
                <span>${message}</span>
                <button onclick="this.parentElement.remove()"><i class="fas fa-times"></i></button>
            `;
            
            document.getElementById('toastContainer').appendChild(toast);
            
            setTimeout(() => {
                toast.style.opacity = '0';
                setTimeout(() => toast.remove(), 300);
            }, 5000);
        }

        // Infinite Scroll
        let loading = false;
        window.addEventListener('scroll', () => {
            if (loading) return;
            
            if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 1000) {
                loading = true;
                setTimeout(() => {
                    // Simulate loading more posts
                    console.log('Loading more posts...');
                    loading = false;
                }, 1000);
            }
        });

        // Mobile Responsiveness
        function checkMobile() {
            const fab = document.getElementById('mobileFab');
            if (window.innerWidth <= 768) {
                fab.style.display = 'flex';
            } else {
                fab.style.display = 'none';
            }
        }

        window.addEventListener('resize', checkMobile);
        checkMobile();

        // Initialize tooltips
        document.querySelectorAll('[data-tooltip]').forEach(element => {
            element.addEventListener('mouseenter', function() {
                this.classList.add('tooltip');
            });
        });

        // Smooth animations on scroll
        function handleScrollAnimations() {
            const elements = document.querySelectorAll('.post, .widget, .sidebar-item');
            elements.forEach(element => {
                const elementTop = element.getBoundingClientRect().top;
                const elementVisible = 150;
                
                if (elementTop < window.innerHeight - elementVisible) {
                    element.style.animation = 'slideIn 0.6s ease-out forwards';
                }
            });
        }

        window.addEventListener('scroll', handleScrollAnimations);
        handleScrollAnimations(); // Run on load

        // Enhanced interaction feedback
        document.querySelectorAll('.interactive-card, .post-action, .connect-btn, .follow-btn').forEach(element => {
            element.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                ripple.className = 'ripple';
                ripple.style.left = e.offsetX + 'px';
                ripple.style.top = e.offsetY + 'px';
                this.appendChild(ripple);
                
                setTimeout(() => ripple.remove(), 600);
            });
        });


