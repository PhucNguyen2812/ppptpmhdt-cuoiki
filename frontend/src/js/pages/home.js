import { getAccessToken, getUserFromToken, removeAccessToken } from "../utils/token.js";
import { getAllCategories } from "../api/categoryApi.js";
import { 
  getFeaturedCourses, 
  getBestSellingCourses, 
  getNewestCourses 
} from "../api/courseApi.js";
import { 
  formatPrice, 
  getStars, 
  formatStudentCount,
  formatRatingCount,
  getCourseUrl,
  getCategoryCoursesUrl,
  getSearchUrl
} from ".../utils/courseHelper.js";
import { DEFAULT_IMAGES } from "../config.js";

// ===== DOM Elements =====
const elements = {
  // Header
  searchInput: document.getElementById('searchInput'),
  instructorBtn: document.getElementById('instructorBtn'),
  teachBtn: document.getElementById('teachBtn'),
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

// ===== Initialize =====
document.addEventListener('DOMContentLoaded', () => {
  initializeAuth();
  loadCategories();
  loadFeaturedCourses();
  loadBestSellingCourses();
  loadNewestCourses();
  setupEventListeners();
});

// ===== Auth Initialization =====
function initializeAuth() {
  const token = getAccessToken();
  
  if (token) {
    const user = getUserFromToken();
    if (user) {
      showUserMenu(user);
      
      // Check if user is instructor
      const role = user.role;
      if (role && (role === 'INSTRUCTOR' || role.toUpperCase() === 'INSTRUCTOR')) {
        elements.instructorBtn.style.display = 'block';
        elements.teachBtn.style.display = 'none';
      }
    } else {
      showAuthButtons();
    }
  } else {
    showAuthButtons();
  }
}

function showUserMenu(user) {
  elements.userMenu.style.display = 'block';
  elements.authButtons.style.display = 'none';
  
  // Set user info
  elements.userName.textContent = user.name || user.email;
  elements.userEmail.textContent = user.email;
  
  // Set avatar (placeholder for now)
  elements.userAvatar.src = DEFAULT_IMAGES.AVATAR;
  document.getElementById('dropdownAvatar').src = DEFAULT_IMAGES.AVATAR;
}

function showAuthButtons() {
  elements.userMenu.style.display = 'none';
  elements.authButtons.style.display = 'flex';
}

// ===== Event Listeners =====
function setupEventListeners() {
  // Search
  elements.searchInput?.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  });
  
  // User menu dropdown
  elements.userMenu?.addEventListener('click', (e) => {
    e.stopPropagation();
    elements.dropdownMenu?.classList.toggle('show');
  });
  
  // Close dropdown when clicking outside
  document.addEventListener('click', (e) => {
    if (!elements.userMenu?.contains(e.target)) {
      elements.dropdownMenu?.classList.remove('show');
    }
  });
  
  // Logout
  elements.logoutBtn?.addEventListener('click', (e) => {
    e.preventDefault();
    handleLogout();
  });
  
  // Login button
  elements.loginBtn?.addEventListener('click', () => {
    window.location.href = '/pages/login.html';
  });
  
  // Signup button  
  elements.signupBtn?.addEventListener('click', () => {
    window.location.href = '/pages/register.html';
  });
  
  // Teach button
  elements.teachBtn?.addEventListener('click', () => {
    if (getAccessToken()) {
      window.location.href = '/pages/become-instructor.html';
    } else {
      window.location.href = '/pages/login.html?redirect=become-instructor';
    }
  });
  
  // Instructor button
  elements.instructorBtn?.addEventListener('click', () => {
    window.location.href = '/pages/instructor-dashboard.html';
  });
}

// ===== Search =====
function handleSearch() {
  const searchTerm = elements.searchInput.value.trim();
  if (searchTerm) {
    window.location.href = getSearchUrl(searchTerm);
  }
}

// ===== Logout =====
function handleLogout() {
  if (confirm('Bạn có chắc muốn đăng xuất?')) {
    removeAccessToken();
    window.location.href = '/pages/index.html';
  }
}

// ===== Load Categories =====
async function loadCategories() {
  try {
    const response = await getAllCategories();
    
    if (response.success && response.data) {
      renderCategoryNav(response.data);
      renderCategoriesGrid(response.data);
    }
  } catch (error) {
    console.error('Error loading categories:', error);
    if (elements.categoryNav) {
      elements.categoryNav.innerHTML = '<p style="color: #999;">Không thể tải danh mục</p>';
    }
  }
}

function renderCategoryNav(categories) {
  if (!elements.categoryNav) return;
  
  const html = categories.slice(0, 10).map(cat => `
    <a href="${getCategoryCoursesUrl(cat.id)}">${cat.tenDanhMuc}</a>
  `).join('');
  
  elements.categoryNav.innerHTML = html;
}

function renderCategoriesGrid(categories) {
  if (!elements.categoriesGrid) return;
  
  const icons = ['📚', '💼', '💰', '💻', '📊', '🎯', '🎨', '📱', '🏃', '🎵'];
  
  const html = categories.map((cat, index) => `
    <div class="category-card" onclick="window.location.href='${getCategoryCoursesUrl(cat.id)}'">
      <div class="category-icon">${icons[index % icons.length]}</div>
      <div class="category-name">${cat.tenDanhMuc}</div>
      <div class="category-count">${cat.soKhoaHoc || 0} khóa học</div>
    </div>
  `).join('');
  
  elements.categoriesGrid.innerHTML = html;
}

// ===== Load Courses =====
async function loadFeaturedCourses() {
  try {
    const response = await getFeaturedCourses(8);
    
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

async function loadBestSellingCourses() {
  try {
    const response = await getBestSellingCourses(8);
    
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

async function loadNewestCourses() {
  try {
    const response = await getNewestCourses(8);
    
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
  if (!container) return;
  
  if (!courses || courses.length === 0) {
    container.innerHTML = '<p>Không có khóa học nào</p>';
    return;
  }
  
  const html = courses.map(course => `
    <div class="course-card" onclick="window.location.href='${getCourseUrl(course.id)}'">
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
          <span class="rating-count">${formatRatingCount(course.soLuongDanhGia)}</span>
        </div>
        ` : ''}
        
        <p class="course-stats">
          ${formatStudentCount(course.soLuongHocVien)}
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