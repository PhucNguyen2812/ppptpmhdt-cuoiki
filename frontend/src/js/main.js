// Main Application Logic

// ===== DOM Elements =====
const elements = {
    // Header
    searchInput: document.getElementById('searchInput'),
    instructorBtn: document.getElementById('instructorBtn'),
    teachBtn: document.getElementById('teachBtn'),
    cartLink: document.getElementById('cartLink'),
    cartBadge: document.getElementById('cartBadge'),
    userMenu: document.getElementById('userMenu'),
    authButtons: document.getElementById('authButtons'),
    loginBtn: document.getElementById('loginBtn'),
    signupBtn: document.getElementById('signupBtn'),
    logoutBtn: document.getElementById('logoutBtn'),
    dropdownMenu: document.getElementById('dropdownMenu'),
    userAvatar: document.getElementById('userAvatar'),
    userName: document.getElementById('userName'),
    userEmail: document.getElementById('userEmail'),
    
    // Navigation
    categoryNav: document.getElementById('categoryNav'),
    
    // Courses
    featuredCourses: document.getElementById('featuredCourses'),
    bestSellingCourses: document.getElementById('bestSellingCourses'),
    newestCourses: document.getElementById('newestCourses'),
    
    // Categories
    categoriesGrid: document.getElementById('categoriesGrid')
};

// ===== Initialize App =====
document.addEventListener('DOMContentLoaded', () => {
    initializeAuth();
    loadCategories();
    loadFeaturedCourses();
    loadBestSellingCourses();
    loadNewestCourses();
    setupEventListeners();
    loadCartCount();
});

// ===== Auth Initialization =====
function initializeAuth() {
    if (AuthHelper.isLoggedIn()) {
        const userInfo = AuthHelper.getUserInfo();
        showUserMenu(userInfo);
        
        // Show instructor button if user is instructor
        if (AuthHelper.isInstructor()) {
            elements.instructorBtn.style.display = 'block';
            elements.teachBtn.style.display = 'none';
        }
    } else {
        showAuthButtons();
    }
}

function showUserMenu(userInfo) {
    elements.userMenu.style.display = 'block';
    elements.authButtons.style.display = 'none';
    
    if (userInfo) {
        // Set user info
        elements.userName.textContent = userInfo.hoTen || userInfo.email;
        elements.userEmail.textContent = userInfo.email;
        
        // Set avatar
        if (userInfo.anhDaiDien) {
            elements.userAvatar.src = userInfo.anhDaiDien;
            document.getElementById('dropdownAvatar').src = userInfo.anhDaiDien;
        } else {
            elements.userAvatar.src = DEFAULT_IMAGES.AVATAR;
            document.getElementById('dropdownAvatar').src = DEFAULT_IMAGES.AVATAR;
        }
    }
}

function showAuthButtons() {
    elements.userMenu.style.display = 'none';
    elements.authButtons.style.display = 'flex';
}

// ===== Event Listeners =====
function setupEventListeners() {
    // Search
    elements.searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            handleSearch();
        }
    });
    
    // User menu dropdown
    if (elements.userMenu) {
        elements.userMenu.addEventListener('click', () => {
            elements.dropdownMenu.classList.toggle('show');
        });
    }
    
    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (!elements.userMenu.contains(e.target)) {
            elements.dropdownMenu.classList.remove('show');
        }
    });
    
    // Logout
    if (elements.logoutBtn) {
        elements.logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            handleLogout();
        });
    }
    
    // Login button
    if (elements.loginBtn) {
        elements.loginBtn.addEventListener('click', () => {
            window.location.href = 'login.html';
        });
    }
    
    // Signup button
    if (elements.signupBtn) {
        elements.signupBtn.addEventListener('click', () => {
            window.location.href = 'register.html';
        });
    }
    
    // Teach button
    if (elements.teachBtn) {
        elements.teachBtn.addEventListener('click', () => {
            if (AuthHelper.isLoggedIn()) {
                window.location.href = 'become-instructor.html';
            } else {
                window.location.href = 'login.html?redirect=become-instructor';
            }
        });
    }
    
    // Instructor button
    if (elements.instructorBtn) {
        elements.instructorBtn.addEventListener('click', () => {
            window.location.href = 'instructor-dashboard.html';
        });
    }
}

// ===== Search Handler =====
function handleSearch() {
    const searchTerm = elements.searchInput.value.trim();
    if (searchTerm) {
        window.location.href = `courses.html?search=${encodeURIComponent(searchTerm)}`;
    }
}

// ===== Logout Handler =====
function handleLogout() {
    if (confirm('Bạn có chắc muốn đăng xuất?')) {
        AuthHelper.logout();
    }
}

// ===== Load Categories =====
async function loadCategories() {
    try {
        const response = await API.getCategories();
        
        if (response.success && response.data) {
            renderCategoryNav(response.data);
            renderCategoriesGrid(response.data);
        }
    } catch (error) {
        console.error('Error loading categories:', error);
        elements.categoryNav.innerHTML = '<p style="color: #999;">Không thể tải danh mục</p>';
    }
}

function renderCategoryNav(categories) {
    const html = categories.slice(0, 10).map(cat => `
        <a href="courses.html?categoryId=${cat.id}">${cat.tenDanhMuc}</a>
    `).join('');
    
    elements.categoryNav.innerHTML = html;
}

function renderCategoriesGrid(categories) {
    const icons = ['📚', '💼', '💰', '💻', '📊', '🎯', '🎨', '📱', '🏃', '🎵'];
    
    const html = categories.map((cat, index) => `
        <div class="category-card" onclick="window.location.href='courses.html?categoryId=${cat.id}'">
            <div class="category-icon">${icons[index % icons.length]}</div>
            <div class="category-name">${cat.tenDanhMuc}</div>
            <div class="category-count">${cat.soKhoaHoc || 0} khóa học</div>
        </div>
    `).join('');
    
    elements.categoriesGrid.innerHTML = html;
}

// ===== Load Featured Courses =====
async function loadFeaturedCourses() {
    try {
        const response = await API.getFeaturedCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.featuredCourses);
        }
    } catch (error) {
        console.error('Error loading featured courses:', error);
        elements.featuredCourses.innerHTML = '<p>Không thể tải khóa học nổi bật</p>';
    }
}

// ===== Load Best Selling Courses =====
async function loadBestSellingCourses() {
    try {
        const response = await API.getBestSellingCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.bestSellingCourses);
        }
    } catch (error) {
        console.error('Error loading best-selling courses:', error);
        elements.bestSellingCourses.innerHTML = '<p>Không thể tải khóa học bán chạy</p>';
    }
}

// ===== Load Newest Courses =====
async function loadNewestCourses() {
    try {
        const response = await API.getNewestCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.newestCourses);
        }
    } catch (error) {
        console.error('Error loading newest courses:', error);
        elements.newestCourses.innerHTML = '<p>Không thể tải khóa học mới nhất</p>';
    }
}

// ===== Render Courses =====
function renderCourses(courses, container) {
    if (!courses || courses.length === 0) {
        container.innerHTML = '<p>Không có khóa học nào</p>';
        return;
    }
    
    const html = courses.map(course => `
        <div class="course-card" onclick="window.location.href='course-detail.html?id=${course.id}'">
            <img src="${course.hinhDaiDien || DEFAULT_IMAGES.COURSE}" 
                 alt="${course.tenKhoaHoc}" 
                 class="course-image"
                 onerror="this.src='${DEFAULT_IMAGES.COURSE}'">
            <div class="course-body">
                <h3 class="course-title">${course.tenKhoaHoc}</h3>
                <p class="course-instructor">${course.tenGiangVien || 'Giảng viên'}</p>
                
                ${course.diemDanhGia ? `
                <div class="course-rating">
                    <span class="rating-number">${course.diemDanhGia.toFixed(1)}</span>
                    <span class="stars">${getStars(course.diemDanhGia)}</span>
                    <span class="rating-count">(${course.soLuongDanhGia || 0})</span>
                </div>
                ` : ''}
                
                <p class="course-stats">
                    ${course.soLuongHocVien || 0} học viên
                </p>
                
                <div class="course-price">
                    ${formatPrice(course.giaBan)}
                </div>
                
                ${course.mucDo ? `
                <span class="course-level">${course.mucDo}</span>
                ` : ''}
            </div>
        </div>
    `).join('');
    
    container.innerHTML = html;
}

// ===== Load Cart Count =====
async function loadCartCount() {
    try {
        if (AuthHelper.isLoggedIn()) {
            const response = await API.getCartCount();
            if (response.success) {
                elements.cartBadge.textContent = response.data || 0;
            }
        } else {
            elements.cartBadge.textContent = 0;
        }
    } catch (error) {
        console.error('Error loading cart count:', error);
        elements.cartBadge.textContent = 0;
    }
}

// ===== Helper Functions =====
function getStars(rating) {
    const fullStars = Math.floor(rating);
    const halfStar = rating % 1 >= 0.5 ? 1 : 0;
    const emptyStars = 5 - fullStars - halfStar;
    
    return '★'.repeat(fullStars) + 
           (halfStar ? '☆' : '') + 
           '☆'.repeat(emptyStars);
}

function formatPrice(price) {
    if (!price) return 'Miễn phí';
    return new Intl.NumberFormat('vi-VN', { 
        style: 'currency', 
        currency: 'VND' 
    }).format(price);
}

// ===== Error Handler =====
window.addEventListener('error', (e) => {
    console.error('Global error:', e.error);
});

// ===== Unhandled Promise Rejection =====
window.addEventListener('unhandledrejection', (e) => {
    console.error('Unhandled promise rejection:', e.reason);
});
