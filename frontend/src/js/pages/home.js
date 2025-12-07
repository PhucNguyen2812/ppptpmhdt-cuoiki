import AuthHelper from "../utils/authHelper.js";
import { getAllCategories } from "../api/categoryApi.js";
import { 
  getFeaturedCourses, 
  getBestSellingCourses, 
  getNewestCourses 
} from "../api/courseApi.js";
import { getCartCount } from "../api/cartApi.js";
import { 
  formatPrice, 
  getStars, 
  formatStudentCount,
  formatRatingCount,
  getCourseUrl,
  getCategoryCoursesUrl,
  getSearchUrl
} from "../utils/courseHelper.js";
import { DEFAULT_IMAGES } from "../config.js";
import { initNotificationComponent } from "../components/NotificationComponent.js";

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
document.addEventListener('DOMContentLoaded', async () => {
  await initializeAuth();
  setupEventListeners();
  await loadCartCount();
  loadCategories();
  loadFeaturedCourses();
  loadBestSellingCourses();
  loadNewestCourses();
  
  // Initialize notification component
  try {
    initNotificationComponent();
  } catch (error) {
    console.error('Error initializing notification component:', error);
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

function hideUserMenu() {
  if (elements.userMenu) {
    elements.userMenu.style.display = 'none';
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

function hideAuthButtons() {
  if (elements.authButtons) {
    elements.authButtons.style.display = 'none';
  }
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
    if (elements.userMenu && elements.dropdownMenu) {
      if (!elements.userMenu.contains(e.target) && !elements.dropdownMenu.contains(e.target)) {
        elements.dropdownMenu.classList.remove('show');
      }
    }
  });
  
  // Logout
  elements.logoutBtn?.addEventListener('click', (e) => {
    e.preventDefault();
    handleLogout();
  });
  
  // Login button - only navigate if button is visible and user is not logged in
  elements.loginBtn?.addEventListener('click', () => {
    // Only navigate if user is not logged in
    const authButtonsVisible = elements.authButtons && 
      window.getComputedStyle(elements.authButtons).display !== 'none';
    
    if (authButtonsVisible && !AuthHelper.isLoggedIn()) {
      window.location.href = 'login.html';
    }
  });
  
  // Signup button - only navigate if button is visible and user is not logged in
  elements.signupBtn?.addEventListener('click', () => {
    // Only navigate if user is not logged in
    const authButtonsVisible = elements.authButtons && 
      window.getComputedStyle(elements.authButtons).display !== 'none';
    
    if (authButtonsVisible && !AuthHelper.isLoggedIn()) {
      window.location.href = 'register.html';
    }
  });
  
  // Teach button
  elements.teachBtn?.addEventListener('click', () => {
    if (AuthHelper.isLoggedIn()) {
      window.location.href = 'become-instructor.html';
    } else {
      window.location.href = 'login.html?redirect=become-instructor';
    }
  });
  
  // Instructor button
  elements.instructorBtn?.addEventListener('click', () => {
    window.location.href = 'instructor-dashboard.html';
  });
  
  // Cart link
  const cartLink = document.getElementById('cartLink');
  if (cartLink) {
    cartLink.addEventListener('click', (e) => {
      e.preventDefault();
      if (AuthHelper.isLoggedIn()) {
        window.location.href = 'my-cart.html';
      } else {
        window.location.href = 'login.html?redirect=my-cart';
      }
    });
  }
}

// ===== Search =====
function handleSearch() {
  const searchTerm = elements.searchInput.value.trim();
  if (searchTerm) {
    window.location.href = getSearchUrl(searchTerm);
  }
}

// ===== Logout =====
async function handleLogout() {
  if (confirm('Bạn có chắc muốn đăng xuất?')) {
    await AuthHelper.logout();
  }
}

// ===== Load Categories =====
async function loadCategories() {
  try {
    console.log('Loading categories...');
    const response = await getAllCategories();
    console.log('Categories response:', response);
    
    // Handle ApiResponse structure: { success, message, data }
    let categories = [];
    if (response && response.success !== undefined) {
      // ApiResponse format
      if (response.success && response.data) {
        categories = Array.isArray(response.data) ? response.data : [];
      }
    } else if (Array.isArray(response)) {
      // Direct array response
      categories = response;
    }
    
    if (categories.length > 0) {
      console.log(`Rendering ${categories.length} categories`);
      renderCategoryNav(categories);
      renderCategoriesGrid(categories);
    } else {
      console.warn('No categories found');
      if (elements.categoryNav) {
        elements.categoryNav.innerHTML = '<p style="color: #999; padding: 12px 0;">Không có danh mục nào</p>';
      }
    }
  } catch (error) {
    console.error('Error loading categories:', error);
    if (elements.categoryNav) {
      elements.categoryNav.innerHTML = '<p style="color: #999; padding: 12px 0;">Không thể tải danh mục</p>';
    }
  }
}

function renderCategoryNav(categories) {
  if (!elements.categoryNav) return;
  
  if (!categories || categories.length === 0) {
    elements.categoryNav.innerHTML = '<p style="color: #999; padding: 12px 0;">Không có danh mục</p>';
    return;
  }
  
  // Show up to 12 categories in navigation
  const html = categories.slice(0, 12).map(cat => {
    if (!cat) return '';
    const categoryName = cat.tenDanhMuc || cat.TenDanhMuc || cat.name || 'Danh mục';
    const categoryId = cat.id || cat.Id;
    if (!categoryId) return '';
    return `<a href="${getCategoryCoursesUrl(categoryId)}">${categoryName}</a>`;
  }).filter(html => html !== '').join('');
  
  elements.categoryNav.innerHTML = html || '<p style="color: #999; padding: 12px 0;">Không có danh mục hợp lệ</p>';
}

function renderCategoriesGrid(categories) {
  if (!elements.categoriesGrid) return;
  
  if (!categories || categories.length === 0) {
    elements.categoriesGrid.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Không có danh mục nào</p>';
    return;
  }
  
  const icons = ['📚', '💼', '💰', '💻', '📊', '🎯', '🎨', '📱', '🏃', '🎵'];
  
  const html = categories.map((cat, index) => {
    if (!cat) return '';
    const categoryId = cat.id || cat.Id;
    const categoryName = cat.tenDanhMuc || cat.TenDanhMuc || 'Danh mục';
    const courseCount = cat.soKhoaHoc !== undefined ? cat.soKhoaHoc : (cat.SoKhoaHoc !== undefined ? cat.SoKhoaHoc : 0);
    
    if (!categoryId) return '';
    
    return `
      <div class="category-card" onclick="window.location.href='${getCategoryCoursesUrl(categoryId)}'">
        <div class="category-icon">${icons[index % icons.length]}</div>
        <div class="category-name">${categoryName}</div>
        <div class="category-count">${courseCount} khóa học</div>
      </div>
    `;
  }).filter(html => html !== '').join('');
  
  elements.categoriesGrid.innerHTML = html || '<p style="text-align: center; padding: 20px; color: #666;">Không có danh mục hợp lệ</p>';
}

// ===== Load Cart Count =====
async function loadCartCount() {
  if (!elements.cartBadge) return;
  
  try {
    if (AuthHelper.isLoggedIn()) {
      const response = await getCartCount();
      
      // Handle ApiResponse structure
      if (response && response.success !== undefined) {
        // ApiResponse format: { success, message, data }
        if (response.success && response.data !== undefined) {
          elements.cartBadge.textContent = response.data || 0;
        } else {
          elements.cartBadge.textContent = 0;
        }
      } else if (typeof response === 'number') {
        // Direct number response
        elements.cartBadge.textContent = response;
      } else {
        elements.cartBadge.textContent = 0;
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

// ===== Load Courses =====
async function loadFeaturedCourses() {
  try {
    console.log('Loading featured courses...');
    const response = await getFeaturedCourses(8);
    console.log('Featured courses response:', response);
    
    // Handle ApiResponse structure: { success, message, data }
    let courses = [];
    if (response && response.success !== undefined) {
      // ApiResponse format
      if (response.success && response.data) {
        courses = Array.isArray(response.data) ? response.data : [];
      }
    } else if (Array.isArray(response)) {
      // Direct array response
      courses = response;
    }
    
    if (courses.length > 0) {
      console.log(`Rendering ${courses.length} featured courses`);
      renderCourses(courses, elements.featuredCourses);
    } else {
      console.warn('No featured courses found');
      if (elements.featuredCourses) {
        elements.featuredCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Chưa có khóa học nổi bật</p>';
      }
    }
  } catch (error) {
    console.error('Error loading featured courses:', error);
    if (elements.featuredCourses) {
      elements.featuredCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #dc3545;">Không thể tải khóa học nổi bật. Vui lòng thử lại sau.</p>';
    }
  }
}

async function loadBestSellingCourses() {
  try {
    console.log('Loading best-selling courses...');
    const response = await getBestSellingCourses(8);
    console.log('Best-selling courses response:', response);
    
    // Handle ApiResponse structure: { success, message, data }
    let courses = [];
    if (response && response.success !== undefined) {
      // ApiResponse format
      if (response.success && response.data) {
        courses = Array.isArray(response.data) ? response.data : [];
      }
    } else if (Array.isArray(response)) {
      // Direct array response
      courses = response;
    }
    
    if (courses.length > 0) {
      console.log(`Rendering ${courses.length} best-selling courses`);
      renderCourses(courses, elements.bestSellingCourses);
    } else {
      console.warn('No best-selling courses found');
      if (elements.bestSellingCourses) {
        elements.bestSellingCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Chưa có khóa học bán chạy</p>';
      }
    }
  } catch (error) {
    console.error('Error loading best-selling courses:', error);
    if (elements.bestSellingCourses) {
      elements.bestSellingCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #dc3545;">Không thể tải khóa học bán chạy. Vui lòng thử lại sau.</p>';
    }
  }
}

async function loadNewestCourses() {
  try {
    console.log('Loading newest courses...');
    const response = await getNewestCourses(8);
    console.log('Newest courses response:', response);
    
    // Handle ApiResponse structure: { success, message, data }
    let courses = [];
    if (response && response.success !== undefined) {
      // ApiResponse format
      if (response.success && response.data) {
        courses = Array.isArray(response.data) ? response.data : [];
      }
    } else if (Array.isArray(response)) {
      // Direct array response
      courses = response;
    }
    
    if (courses.length > 0) {
      console.log(`Rendering ${courses.length} newest courses`);
      renderCourses(courses, elements.newestCourses);
    } else {
      console.warn('No newest courses found');
      if (elements.newestCourses) {
        elements.newestCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Chưa có khóa học mới</p>';
      }
    }
  } catch (error) {
    console.error('Error loading newest courses:', error);
    if (elements.newestCourses) {
      elements.newestCourses.innerHTML = '<p style="text-align: center; padding: 20px; color: #dc3545;">Không thể tải khóa học mới nhất. Vui lòng thử lại sau.</p>';
    }
  }
}

// ===== Render Courses =====
function renderCourses(courses, container) {
  if (!container) {
    console.warn('Container not found for rendering courses');
    return;
  }
  
  if (!courses || courses.length === 0) {
    container.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Không có khóa học nào</p>';
    return;
  }
  
  const html = courses.map(course => {
    // Validate course data
    if (!course) {
      console.warn('Invalid course data:', course);
      return '';
    }
    
    const courseId = course.id || course.Id;
    const courseTitle = course.tenKhoaHoc || course.TenKhoaHoc || 'Khóa học';
    const courseImage = course.hinhDaiDien || course.HinhDaiDien || DEFAULT_IMAGES.COURSE;
    const instructorName = course.tenGiangVien || course.TenGiangVien || 'Giảng viên';
    const rating = course.diemDanhGia || course.DiemDanhGia || 0;
    const ratingCount = course.soLuongDanhGia || course.SoLuongDanhGia || 0;
    const studentCount = course.soLuongHocVien || course.SoLuongHocVien || 0;
    const price = course.giaBan !== undefined ? course.giaBan : (course.GiaBan !== undefined ? course.GiaBan : 0);
    const level = course.mucDo || course.MucDo || '';
    
    // Build image URL - handle relative paths
    let imageUrl = courseImage;
    if (courseImage && !courseImage.startsWith('http') && !courseImage.startsWith('data:')) {
      // If it's a relative path, prepend API base URL or handle accordingly
      if (courseImage.startsWith('/')) {
        imageUrl = `http://localhost:5228${courseImage}`;
      } else {
        imageUrl = `http://localhost:5228/${courseImage}`;
      }
    }
    
    return `
      <div class="course-card" onclick="window.location.href='${getCourseUrl(courseId)}'">
        <img src="${imageUrl}" 
             alt="${courseTitle}" 
             class="course-image"
             onerror="this.src='${DEFAULT_IMAGES.COURSE}'">
        <div class="course-body">
          <h3 class="course-title">${courseTitle}</h3>
          <p class="course-instructor">${instructorName}</p>
          
          ${rating > 0 ? `
          <div class="course-rating">
            <span class="rating-number">${rating.toFixed(1)}</span>
            <span class="stars">${getStars(rating)}</span>
            <span class="rating-count">${formatRatingCount(ratingCount)}</span>
          </div>
          ` : '<div class="course-rating"><span class="rating-count">Chưa có đánh giá</span></div>'}
          
          <p class="course-stats">
            ${formatStudentCount(studentCount)}
          </p>
          
          <div class="course-price">
            ${formatPrice(price)}
          </div>
          
          ${level ? `
          <span class="course-level">${level}</span>
          ` : ''}
        </div>
      </div>
    `;
  }).filter(html => html !== '').join('');
  
  if (html) {
    container.innerHTML = html;
  } else {
    container.innerHTML = '<p style="text-align: center; padding: 20px; color: #666;">Không có khóa học hợp lệ để hiển thị</p>';
  }
}