// Main Application Logic
import AuthHelper from './utils/authHelper.js';
import { API } from './api/index.js';
import { DEFAULT_IMAGES } from './config.js';
import { getStars, formatPrice } from './utils/courseHelper.js';

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
document.addEventListener('DOMContentLoaded', async () => {
    // Initialize auth first (which will show/hide appropriate UI)
    await initializeAuth();
    
    // Then setup event listeners
    setupEventListeners();
    
    // Load cart count
    loadCartCount();
    
    // Only load these if elements exist (not on course-detail page)
    if (elements.categoryNav) {
        loadCategories();
    }
    if (elements.featuredCourses) {
        loadFeaturedCourses();
    }
    if (elements.bestSellingCourses) {
        loadBestSellingCourses();
    }
    if (elements.newestCourses) {
        loadNewestCourses();
    }
    if (elements.categoriesGrid) {
        // Categories grid will be loaded in loadCategories if categoryNav exists
    }
});

// ===== Auth Initialization =====
async function initializeAuth() {
    try {
        if (AuthHelper.isLoggedIn()) {
            const userInfo = await AuthHelper.getUserInfo();
            if (userInfo) {
                showUserMenu(userInfo);
                
                // Check if user is instructor and update buttons accordingly
                const isInstructor = await AuthHelper.isInstructor();
                updateInstructorButtons(isInstructor);
            } else {
                // Failed to get user info, show auth buttons
                showAuthButtons();
                updateInstructorButtons(false);
            }
        } else {
            // Not logged in, show auth buttons
            showAuthButtons();
            updateInstructorButtons(false);
        }
    } catch (error) {
        console.error('Error initializing auth:', error);
        // On error, show auth buttons as fallback
        showAuthButtons();
        updateInstructorButtons(false);
    }
}

// Helper function to update instructor/teach buttons visibility
function updateInstructorButtons(isInstructor) {
    if (elements.instructorBtn && elements.teachBtn) {
        if (isInstructor) {
            // User has GIANGVIEN role (có thể có cả HOCVIEN và GIANGVIEN): 
            // show instructor button, hide teach button
            elements.instructorBtn.style.display = 'block';
            elements.teachBtn.style.display = 'none';
        } else {
            // User only has HOCVIEN role (chỉ có role HOCVIEN): 
            // hide instructor button, show teach button
            elements.instructorBtn.style.display = 'none';
            elements.teachBtn.style.display = 'block';
        }
    }
}

function showUserMenu(userInfo) {
    // Hide auth buttons first
    if (elements.authButtons) {
        elements.authButtons.style.display = 'none';
    }
    
    // Show user menu
    if (elements.userMenu) {
        elements.userMenu.style.display = 'block';
    }
    
    if (userInfo) {
        // Set user info
        if (elements.userName) {
            elements.userName.textContent = userInfo.hoTen || userInfo.email || 'Người dùng';
        }
        if (elements.userEmail) {
            elements.userEmail.textContent = userInfo.email || '';
        }
        
        // Set avatar
        const avatarUrl = userInfo.anhDaiDien || DEFAULT_IMAGES.AVATAR;
        if (elements.userAvatar) {
            elements.userAvatar.src = avatarUrl;
            elements.userAvatar.onerror = function() {
                this.src = DEFAULT_IMAGES.AVATAR;
            };
        }
        const dropdownAvatar = document.getElementById('dropdownAvatar');
        if (dropdownAvatar) {
            dropdownAvatar.src = avatarUrl;
            dropdownAvatar.onerror = function() {
                this.src = DEFAULT_IMAGES.AVATAR;
            };
        }
    }
}

function showAuthButtons() {
    // Hide user menu first
    if (elements.userMenu) {
        elements.userMenu.style.display = 'none';
    }
    
    // Hide dropdown if it's open
    if (elements.dropdownMenu) {
        elements.dropdownMenu.classList.remove('show');
    }
    
    // Show auth buttons
    if (elements.authButtons) {
        elements.authButtons.style.display = 'flex';
    }
}

// ===== Event Listeners =====
function setupEventListeners() {
    // Search
    if (elements.searchInput) {
        elements.searchInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                handleSearch();
            }
        });
    }
    
    // User menu dropdown
    if (elements.userMenu && elements.dropdownMenu) {
        elements.userMenu.addEventListener('click', (e) => {
            e.stopPropagation();
            elements.dropdownMenu.classList.toggle('show');
        });
    }
    
    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
        if (elements.userMenu && elements.dropdownMenu) {
            if (!elements.userMenu.contains(e.target) && !elements.dropdownMenu.contains(e.target)) {
                elements.dropdownMenu.classList.remove('show');
            }
        }
    });
    
    // Logout
    if (elements.logoutBtn) {
        elements.logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            handleLogout();
        });
    }
    
    // Login button - only navigate if button is visible and user is not logged in
    if (elements.loginBtn) {
        elements.loginBtn.addEventListener('click', (e) => {
            // Only navigate if user is not logged in
            // Check if auth buttons container is visible (not display: none)
            const authButtonsVisible = elements.authButtons && 
                window.getComputedStyle(elements.authButtons).display !== 'none';
            
            if (authButtonsVisible && !AuthHelper.isLoggedIn()) {
                window.location.href = 'login.html';
            }
        });
    }
    
    // Signup button - only navigate if button is visible and user is not logged in
    if (elements.signupBtn) {
        elements.signupBtn.addEventListener('click', (e) => {
            // Only navigate if user is not logged in
            // Check if auth buttons container is visible (not display: none)
            const authButtonsVisible = elements.authButtons && 
                window.getComputedStyle(elements.authButtons).display !== 'none';
            
            if (authButtonsVisible && !AuthHelper.isLoggedIn()) {
                window.location.href = 'register.html';
            }
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
    
    // Cart link
    if (elements.cartLink) {
        elements.cartLink.addEventListener('click', (e) => {
            e.preventDefault();
            if (AuthHelper.isLoggedIn()) {
                window.location.href = 'my-cart.html';
            } else {
                window.location.href = 'login.html?redirect=my-cart';
            }
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
    if (!elements.categoryNav) return;
    
    const html = categories.slice(0, 10).map(cat => `
        <a href="courses.html?categoryId=${cat.id}">${cat.tenDanhMuc}</a>
    `).join('');
    
    elements.categoryNav.innerHTML = html;
}

function renderCategoriesGrid(categories) {
    if (!elements.categoriesGrid) return;
    
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
    if (!elements.featuredCourses) return;
    
    try {
        const response = await API.getFeaturedCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.featuredCourses);
        }
    } catch (error) {
        console.error('Error loading featured courses:', error);
        if (elements.featuredCourses) {
            elements.featuredCourses.innerHTML = '<p>Không thể tải khóa học nổi bật</p>';
        }
    }
}

// ===== Load Best Selling Courses =====
async function loadBestSellingCourses() {
    if (!elements.bestSellingCourses) return;
    
    try {
        const response = await API.getBestSellingCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.bestSellingCourses);
        }
    } catch (error) {
        console.error('Error loading best-selling courses:', error);
        if (elements.bestSellingCourses) {
            elements.bestSellingCourses.innerHTML = '<p>Không thể tải khóa học bán chạy</p>';
        }
    }
}

// ===== Load Newest Courses =====
async function loadNewestCourses() {
    if (!elements.newestCourses) return;
    
    try {
        const response = await API.getNewestCourses(8);
        
        if (response.success && response.data) {
            renderCourses(response.data, elements.newestCourses);
        }
    } catch (error) {
        console.error('Error loading newest courses:', error);
        if (elements.newestCourses) {
            elements.newestCourses.innerHTML = '<p>Không thể tải khóa học mới nhất</p>';
        }
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
    if (!elements.cartBadge) return;
    
    try {
        if (AuthHelper.isLoggedIn()) {
            const { getCartCount } = await import('./api/cartApi.js');
            const response = await getCartCount();
            if (response.success) {
                elements.cartBadge.textContent = response.data || 0;
            }
        } else {
            elements.cartBadge.textContent = 0;
        }
    } catch (error) {
        console.error('Error loading cart count:', error);
        if (elements.cartBadge) {
            elements.cartBadge.textContent = 0;
        }
    }
}

// Export loadCartCount for use in other pages
window.loadCartCount = loadCartCount;

// ===== Error Handler =====
window.addEventListener('error', (e) => {
    console.error('Global error:', e.error);
});

// ===== Unhandled Promise Rejection =====
window.addEventListener('unhandledrejection', (e) => {
    console.error('Unhandled promise rejection:', e.reason);
});
