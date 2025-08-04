
    // Language dropdown functionality
    document.querySelector('.language-dropdown').addEventListener('click', function() {
            // In a real application, this would show a dropdown menu
            const currentLang = this.querySelector('span').textContent;
    const newLang = currentLang === 'EN' ? 'AR' : 'EN';
    this.querySelector('span').textContent = newLang;

    // Add direction change for Arabic
    if (newLang === 'AR') {
        document.body.style.direction = 'rtl';
    document.body.style.fontFamily = 'Arial, sans-serif';
            } else {
        document.body.style.direction = 'ltr';
    document.body.style.fontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
            }
        });

        // Post interaction animations
        document.querySelectorAll('.post-action').forEach(action => {
        action.addEventListener('click', function () {
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = 'scale(1)';
            }, 150);

            // Simulate like count increase
            if (this.querySelector('.fa-thumbs-up')) {
                const countElement = this.querySelector('span');
                const currentCount = parseInt(countElement.textContent.match(/\d+/)[0]);
                countElement.textContent = `Like (${currentCount + 1})`;
            }
        });
        });

        // Connect button functionality
        document.querySelectorAll('.connect-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            if (this.textContent === 'Connect') {
                this.textContent = 'Pending';
                this.style.background = 'var(--primary-yellow)';
                this.style.color = 'var(--primary-dark)';
            } else {
                this.textContent = 'Connect';
                this.style.background = 'var(--primary-blue)';
                this.style.color = 'white';
            }
        });
        });

    // Search functionality
    document.querySelector('.search-box').addEventListener('input', function() {
        // In a real application, this would trigger search results
        console.log('Searching for:', this.value);
        });

    // Post creation
    document.querySelector('.post-input').addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && e.ctrlKey) {
        // In a real application, this would create a new post
        console.log('Creating post:', this.value);
    this.value = '';
            }
        });

        // Sidebar item hover effects
        document.querySelectorAll('.sidebar-item').forEach(item => {
        item.addEventListener('mouseenter', function () {
            this.style.background = 'linear-gradient(135deg, rgba(27, 132, 255, 0.1), rgba(246, 192, 0, 0.1))';
        });

    item.addEventListener('mouseleave', function() {
        this.style.background = 'transparent';
            });
        });

    // Add scroll effect to posts
    const observerOptions = {
        threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.animation = 'slideIn 0.5s ease-out';
            }
        });
        }, observerOptions);

        document.querySelectorAll('.post').forEach(post => {
        observer.observe(post);
        });

