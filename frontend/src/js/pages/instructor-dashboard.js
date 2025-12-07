/**
 * Instructor Dashboard Page
 * Main page for instructor management
 */

import { InstructorHeader } from '../components/instructor/header.js';
import { InstructorSidebar } from '../components/instructor/sidebar.js';
import { requireAuth } from '../api/authApi.js';
import { isInstructor, getUserInfo } from '../utils/authHelper.js';
import { getUserFromToken } from '../utils/token.js';
import { 
  getCoursesByInstructor, 
  createCourse, 
  updateCourse, 
  deleteCourse, 
  hideCourse, 
  unhideCourse 
} from '../api/courseApi.js';
import { showCourseFormModal } from './course-form-modal.js';

// Check authentication and instructor role
async function checkAccess() {
  requireAuth();
  const instructor = await isInstructor();
  if (!instructor) {
    alert('Bạn không có quyền truy cập trang này. Vui lòng đăng ký làm giảng viên.');
    window.location.href = '/src/pages/become-instructor.html';
    return false;
  }
  return true;
}

// Initialize
window.addEventListener('DOMContentLoaded', async () => {
  const hasAccess = await checkAccess();
  if (!hasAccess) return;

  await initializeLayout();
  setupEventListeners();
  loadOverview();
});

/**
 * Initialize page layout
 */
async function initializeLayout() {
  // Get user info from API
  let userInfo = null;
  try {
    userInfo = await getUserInfo();
  } catch (error) {
    console.error('Error loading user info:', error);
  }

  // Header
  const header = new InstructorHeader({
    onMenuToggle: toggleMobileSidebar,
    userInfo: userInfo
  });

  const headerContainer = document.getElementById('header');
  if (headerContainer) {
    headerContainer.innerHTML = header.render();
    header.attachEventListeners();
  }

  // Sidebar
  const sidebar = new InstructorSidebar({
    activeItem: 'overview',
    onItemClick: handleMenuClick
  });

  const sidebarContainer = document.getElementById('sidebar-container');
  if (sidebarContainer) {
    sidebarContainer.innerHTML = sidebar.render();
    sidebar.attachEventListeners();
  }
}

/**
 * Handle menu item click
 */
function handleMenuClick(navId, sectionId) {
  // Hide all sections
  const sections = document.querySelectorAll('.content-section');
  sections.forEach(section => section.classList.remove('active'));

  // Show selected section
  const selectedSection = document.getElementById(sectionId);
  if (selectedSection) {
    selectedSection.classList.add('active');
  }

  // Load content for the selected section
  switch (navId) {
    case 'overview':
      loadOverview();
      break;
    case 'courses':
      loadCourses();
      break;
    case 'revenue':
      loadRevenue();
      break;
    case 'students':
      loadStudents();
      break;
    case 'reviews':
      loadReviews();
      break;
  }
}

/**
 * Load overview content
 */
async function loadOverview() {
  const content = document.getElementById('overview-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    const user = getUserFromToken();
    if (!user || !user.id) {
      content.innerHTML = '<p>Không thể lấy thông tin người dùng. Vui lòng đăng nhập lại.</p>';
      return;
    }

    // Convert user.id to number if it's a string
    const instructorId = typeof user.id === 'string' ? parseInt(user.id, 10) : user.id;
    if (isNaN(instructorId) || instructorId <= 0) {
      content.innerHTML = '<p>ID người dùng không hợp lệ. Vui lòng đăng nhập lại.</p>';
      return;
    }

    // Get instructor courses
    const coursesResponse = await getCoursesByInstructor(instructorId, 1, 10);
    
    let stats = {
      totalCourses: 0,
      totalStudents: 0,
      totalRevenue: 0,
      averageRating: 0
    };

    if (coursesResponse.success && coursesResponse.data) {
      stats.totalCourses = coursesResponse.data.totalCount || 0;
      // TODO: Calculate other stats from API
    }

    content.innerHTML = `
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng khóa học</div>
            <div class="stat-card__icon stat-card__icon--blue">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M10.394 2.08a1 1 0 0 0-.788 0l-7 3a1 1 0 0 0 0 1.84L5.25 8.051a.999.999 0 0 1 .356-.257l4-1.714a1 1 0 1 1 .788 1.838L7.667 9.088l1.94.831a1 1 0 0 1 .557 1.04l1.055 4.184a.5.5 0 0 0 .935 0l1.055-4.184a1 1 0 0 1 .557-1.04l1.94-.831.5.214a1 1 0 1 1-.788 1.838l-4.5-1.928a1 1 0 0 0-.356-.257l-4.25-1.82V11.5a.5.5 0 0 0 .356-.257l4-1.714a1 1 0 1 1 .788 1.838l-4 1.714a1 1 0 0 0-.356.257l-.25 1.07a1 1 0 0 1-.83.67l-.5.01a1 1 0 0 1-.83-.67l-.25-1.07a1 1 0 0 0-.356-.257l-4-1.714a1 1 0 1 1 .788-1.838l4 1.714a1 1 0 0 0 .356.257l.25 1.07a1 1 0 0 1 .83.67l.5.01a1 1 0 0 1 .83-.67l.25-1.07a1 1 0 0 0 .356-.257l4-1.714a1 1 0 1 1 .788-1.838l-4 1.714a1 1 0 0 0-.356.257l-4.25 1.82V6.5a.5.5 0 0 0-.356.257l-4 1.714a1 1 0 1 1-.788-1.838l4-1.714a1 1 0 0 0 .356-.257l7-3z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.totalCourses}</div>
        </div>

        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng học viên</div>
            <div class="stat-card__icon stat-card__icon--green">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.totalStudents}</div>
        </div>

        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng doanh thu</div>
            <div class="stat-card__icon stat-card__icon--purple">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">₫${(stats.totalRevenue / 1000000).toFixed(1)}M</div>
        </div>

        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Đánh giá trung bình</div>
            <div class="stat-card__icon stat-card__icon--yellow">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.averageRating.toFixed(1)} ⭐</div>
        </div>
      </div>
    `;
  } catch (error) {
    console.error('Error loading overview:', error);
    content.innerHTML = '<p>Có lỗi xảy ra khi tải dữ liệu.</p>';
  }
}

/**
 * Load courses content
 */
async function loadCourses() {
  const content = document.getElementById('courses-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    const user = getUserFromToken();
    if (!user || !user.id) {
      content.innerHTML = '<p>Không thể lấy thông tin người dùng. Vui lòng đăng nhập lại.</p>';
      return;
    }

    // Convert user.id to number if it's a string
    const instructorId = typeof user.id === 'string' ? parseInt(user.id, 10) : user.id;
    if (isNaN(instructorId) || instructorId <= 0) {
      content.innerHTML = '<p>ID người dùng không hợp lệ. Vui lòng đăng nhập lại.</p>';
      return;
    }

    const coursesResponse = await getCoursesByInstructor(instructorId, 1, 20);
    
    if (coursesResponse.success && coursesResponse.data) {
      const courses = coursesResponse.data.items || [];
      
      content.innerHTML = `
        <div style="margin-bottom: 20px;">
          <button class="btn btn-primary" onclick="showCourseModal()">
            <i class="fas fa-plus"></i> Thêm khóa học mới
          </button>
        </div>
        ${courses.length === 0 ? `
          <div class="empty-state">
            <div class="empty-state-icon">📚</div>
            <div class="empty-state-text">Bạn chưa có khóa học nào</div>
            <div class="empty-state-subtext">Hãy tạo khóa học đầu tiên của bạn!</div>
          </div>
        ` : `
          <div class="table-container">
            <table class="table">
              <thead>
                <tr>
                  <th>Tên khóa học</th>
                  <th>Danh mục</th>
                  <th>Học viên</th>
                  <th>Đánh giá</th>
                  <th>Trạng thái</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                ${courses.map(course => {
                  const statusBadge = getStatusBadge(course);
                  return `
                    <tr>
                      <td>${course.tenKhoaHoc || 'N/A'}</td>
                      <td>${course.tenDanhMuc || 'N/A'}</td>
                      <td>${course.soLuongHocVien || 0}</td>
                      <td>${course.diemDanhGia ? course.diemDanhGia.toFixed(1) : '0.0'} ⭐</td>
                      <td>${statusBadge}</td>
                      <td>
                        <div class="actions" style="display: flex; gap: 8px;">
                          <button class="btn btn-sm btn-primary" onclick="window.location.href='/src/pages/course-detail.html?id=${course.id}'">Xem</button>
                          <button class="btn btn-sm btn-secondary" onclick="showCourseModal(${course.id})">Sửa</button>
                          ${course.trangThai ? 
                            `<button class="btn btn-sm btn-warning" onclick="handleHideCourse(${course.id})">Ẩn</button>` :
                            `<button class="btn btn-sm btn-success" onclick="handleUnhideCourse(${course.id})">Hiển thị</button>`
                          }
                        </div>
                      </td>
                    </tr>
                  `;
                }).join('')}
              </tbody>
            </table>
          </div>
        `}
      `;
    } else {
      content.innerHTML = '<p>Không thể tải danh sách khóa học.</p>';
    }
  } catch (error) {
    console.error('Error loading courses:', error);
    content.innerHTML = '<p>Có lỗi xảy ra khi tải danh sách khóa học.</p>';
  }
}

/**
 * Get status badge HTML
 */
function getStatusBadge(course) {
  // Use TrangThai to determine status
  return course.trangThai 
    ? '<span class="role-badge role-user">Hiển thị</span>'
    : '<span class="role-badge role-warning">Ẩn</span>';
}

/**
 * Show course modal for add/edit
 */
window.showCourseModal = async function(courseId = null) {
  await showCourseFormModal(courseId);
};

/**
 * Handle delete course
 */
window.handleDeleteCourse = async function(courseId, courseName) {
  if (!confirm(`Bạn có chắc chắn muốn xóa khóa học "${courseName}"?`)) {
    return;
  }

  try {
    const response = await deleteCourse(courseId);
    if (response.success) {
      alert('Xóa khóa học thành công');
      loadCourses();
    }
  } catch (error) {
    alert('Lỗi: ' + error.message);
  }
};

/**
 * Handle hide course
 */
window.handleHideCourse = async function(courseId) {
  if (!confirm('Bạn có chắc chắn muốn ẩn khóa học này? Học viên đã đăng ký vẫn có thể xem.')) {
    return;
  }

  try {
    const response = await hideCourse(courseId);
    if (response.success) {
      alert('Ẩn khóa học thành công');
      loadCourses();
    }
  } catch (error) {
    alert('Lỗi: ' + error.message);
  }
};

/**
 * Handle unhide course
 */
window.handleUnhideCourse = async function(courseId) {
  if (!confirm('Bạn có chắc chắn muốn hiển thị lại khóa học này? Khóa học sẽ cần được duyệt lại.')) {
    return;
  }

  try {
    const response = await unhideCourse(courseId);
    if (response.success) {
      alert('Yêu cầu hiển thị lại khóa học đã được gửi. Đang chờ duyệt.');
      loadCourses();
    }
  } catch (error) {
    alert('Lỗi: ' + error.message);
  }
};

/**
 * Load revenue content
 */
function loadRevenue() {
  const content = document.getElementById('revenue-content');
  if (!content) return;

  content.innerHTML = `
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-card__header">
          <div class="stat-card__title">Doanh thu tháng này</div>
          <div class="stat-card__icon stat-card__icon--green">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
              <path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/>
            </svg>
          </div>
        </div>
        <div class="stat-card__value">₫0M</div>
        <div class="stat-card__change stat-card__change--up">
          <span>↑ 0%</span>
          <span>so với tháng trước</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-card__header">
          <div class="stat-card__title">Tổng doanh thu</div>
          <div class="stat-card__icon stat-card__icon--purple">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
              <path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/>
            </svg>
          </div>
        </div>
        <div class="stat-card__value">₫0M</div>
      </div>
    </div>

    <div class="chart-card" style="margin-top: 24px;">
      <h2 class="chart-card__title">Biểu đồ doanh thu</h2>
      <div class="chart-placeholder">
        📊 Biểu đồ doanh thu theo thời gian
      </div>
    </div>
  `;
}

/**
 * Load students content
 */
function loadStudents() {
  const content = document.getElementById('students-content');
  if (!content) return;

  content.innerHTML = `
    <div class="empty-state">
      <div class="empty-state-icon">👥</div>
      <div class="empty-state-text">Danh sách học viên</div>
      <div class="empty-state-subtext">Theo dõi tiến độ học tập của học viên sẽ được hiển thị ở đây</div>
    </div>
  `;
}

/**
 * Load reviews content
 */
function loadReviews() {
  const content = document.getElementById('reviews-content');
  if (!content) return;

  content.innerHTML = `
    <div class="empty-state">
      <div class="empty-state-icon">⭐</div>
      <div class="empty-state-text">Đánh giá từ học viên</div>
      <div class="empty-state-subtext">Các đánh giá về khóa học của bạn sẽ được hiển thị ở đây</div>
    </div>
  `;
}

/**
 * Toggle mobile sidebar
 */
function toggleMobileSidebar() {
  const sidebar = document.getElementById('sidebar');
  const overlay = document.getElementById('sidebar-overlay');

  if (sidebar) {
    sidebar.classList.toggle('mobile-open');
  }
  if (overlay) {
    overlay.classList.toggle('active');
  }
}

/**
 * Setup event listeners
 */
function setupEventListeners() {
  const sidebarOverlay = document.getElementById('sidebar-overlay');
  if (sidebarOverlay) {
    sidebarOverlay.addEventListener('click', toggleMobileSidebar);
  }
}

