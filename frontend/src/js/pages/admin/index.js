/**
 * Admin Dashboard Page
 * Main page for admin management
 */

import { AdminHeader } from '../../components/admin/AdminHeader.js';
import { AdminSidebar } from '../../components/admin/AdminSidebar.js';
import { requireAuth } from '../../api/authApi.js';
import { isAdmin, getUserInfo } from '../../utils/authHelper.js';
import { getUserFromToken } from '../../utils/token.js';
import { getAllUsers, deleteUser } from '../../api/nguoiDungApi.js';
import { getCourses, getAllCourseApprovals, approveCourse, rejectCourse, hideCourseByAdmin, unhideCourseByAdmin, getCourseForReview } from '../../api/courseApi.js';
import { getAllCategories, getCategories, getCategoryById } from '../../api/categoryApi.js';
import { Modal } from '../../components/admin/modal.js';
import { API_BASE_URL } from '../../config.js';
import { showUserFormModal, showEditUserFormModal } from './user-form-modal.js';

// Check authentication and admin role
async function checkAccess() {
  requireAuth();
  const admin = await isAdmin();
  if (!admin) {
    alert('Bạn không có quyền truy cập trang này. Chỉ quản trị viên mới có thể truy cập.');
    window.location.href = '../index.html';
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
  const header = new AdminHeader({
    appTitle: 'UHIHI',
    onMenuToggle: toggleMobileSidebar,
    userInfo: userInfo
  });

  const headerContainer = document.getElementById('header');
  if (headerContainer) {
    headerContainer.innerHTML = await header.render();
    header.attachEventListeners();
  }

  // Sidebar
  const sidebar = new AdminSidebar({
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
    case 'users':
      loadUsers();
      break;
    case 'course-review':
      loadCourseReview();
      break;
    case 'categories':
      loadCategories();
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
    // Load statistics
    const [usersResponse, coursesResponse] = await Promise.all([
      getAllUsers({ pageNumber: 1, pageSize: 1 }),
      getCourses({ pageNumber: 1, pageSize: 1 })
    ]);

    let stats = {
      totalUsers: 0,
      totalCourses: 0,
      totalOrders: 0,
      totalRevenue: 0
    };

    if (usersResponse.success && usersResponse.data) {
      stats.totalUsers = usersResponse.data.totalCount || 0;
    }

    if (coursesResponse.success && coursesResponse.data) {
      stats.totalCourses = coursesResponse.data.totalCount || 0;
    }

    // TODO: Load orders and revenue from API
    stats.totalOrders = 0;
    stats.totalRevenue = 0;

    content.innerHTML = `
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng người dùng</div>
            <div class="stat-card__icon stat-card__icon--blue">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.totalUsers}</div>
        </div>

        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng khóa học</div>
            <div class="stat-card__icon stat-card__icon--green">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M10.394 2.08a1 1 0 0 0-.788 0l-7 3a1 1 0 0 0 0 1.84L5.25 8.051a.999.999 0 0 1 .356-.257l4-1.714a1 1 0 1 1 .788 1.838L7.667 9.088l1.94.831a1 1 0 0 1 .557 1.04l1.055 4.184a.5.5 0 0 0 .935 0l1.055-4.184a1 1 0 0 1 .557-1.04l1.94-.831.5.214a1 1 0 1 1-.788 1.838l-4.5-1.928a1 1 0 0 0-.356-.257l-4.25-1.82V11.5a.5.5 0 0 0 .356-.257l4-1.714a1 1 0 1 1 .788 1.838l-4 1.714a1 1 0 0 0-.356.257l-.25 1.07a1 1 0 0 1-.83.67l-.5.01a1 1 0 0 1-.83-.67l-.25-1.07a1 1 0 0 0-.356-.257l-4-1.714a1 1 0 1 1 .788-1.838l4 1.714a1 1 0 0 0 .356.257l.25 1.07a1 1 0 0 1 .83.67l.5.01a1 1 0 0 1 .83-.67l.25-1.07a1 1 0 0 0 .356-.257l4-1.714a1 1 0 1 1 .788-1.838l-4 1.714a1 1 0 0 0-.356.257l-4.25 1.82V6.5a.5.5 0 0 0-.356.257l-4 1.714a1 1 0 1 1-.788-1.838l4-1.714a1 1 0 0 0 .356-.257l7-3z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.totalCourses}</div>
        </div>

        <div class="stat-card">
          <div class="stat-card__header">
            <div class="stat-card__title">Tổng đơn hàng</div>
            <div class="stat-card__icon stat-card__icon--yellow">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                <path d="M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12.9-1.63h7.45c.75 0 1.41-.41 1.75-1.03l3.58-6.49c.08-.14.12-.31.12-.48 0-.55-.45-1-1-1H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z"/>
              </svg>
            </div>
          </div>
          <div class="stat-card__value">${stats.totalOrders}</div>
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
      </div>
    `;
  } catch (error) {
    console.error('Error loading overview:', error);
    content.innerHTML = '<p>Có lỗi xảy ra khi tải dữ liệu.</p>';
  }
}

/**
 * Load users content
 */
async function loadUsers() {
  const content = document.getElementById('users-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    const usersResponse = await getAllUsers({ pageNumber: 1, pageSize: 20 });
    
    if (usersResponse.success && usersResponse.data) {
      const users = usersResponse.data.items || [];
      
      content.innerHTML = `
        <div style="margin-bottom: 20px; display: flex; gap: 12px;">
          <button class="btn btn-primary" onclick="window.showAddUserModal()">
            <i class="fas fa-plus"></i> Thêm người dùng mới
          </button>
        </div>
        ${users.length === 0 ? `
          <div class="empty-state">
            <div class="empty-state-icon">👥</div>
            <div class="empty-state-text">Chưa có người dùng nào</div>
            <div class="empty-state-subtext">Hãy thêm người dùng đầu tiên!</div>
          </div>
        ` : `
          <div class="table-container">
            <table class="table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Họ tên</th>
                  <th>Email</th>
                  <th>Số điện thoại</th>
                  <th>Trạng thái</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                ${users.map(user => {
                  const statusBadge = user.trangThai 
                    ? '<span class="role-badge role-user">Hoạt động</span>'
                    : '<span class="role-badge role-admin">Đã khóa</span>';
                  
                  return `
                    <tr>
                      <td>${user.id}</td>
                      <td>${user.hoTen || 'N/A'}</td>
                      <td>${user.email || 'N/A'}</td>
                      <td>${user.soDienThoai || 'N/A'}</td>
                      <td>${statusBadge}</td>
                      <td>
                        <div class="actions" style="display: flex; gap: 8px;">
                          <button class="btn btn-sm btn-secondary" onclick="window.showEditUserModal(${user.id})">
                            <i class="fas fa-edit"></i> Sửa
                          </button>
                          <button class="btn btn-sm btn-danger" onclick="window.handleDeleteUser(${user.id}, '${(user.hoTen || '').replace(/'/g, "\\'")}')">
                            <i class="fas fa-trash"></i> Xóa
                          </button>
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
      content.innerHTML = '<p>Không thể tải danh sách người dùng.</p>';
    }
  } catch (error) {
    console.error('Error loading users:', error);
    content.innerHTML = '<p>Có lỗi xảy ra khi tải danh sách người dùng.</p>';
  }
}

/**
 * Show add user modal
 */
window.showAddUserModal = function() {
  showUserFormModal(() => {
    loadUsers();
  });
};

/**
 * Show edit user modal
 */
window.showEditUserModal = function(userId) {
  showEditUserFormModal(userId, () => {
    loadUsers();
  });
};

/**
 * Handle delete user (soft delete)
 */
window.handleDeleteUser = async function(userId, userName) {
  if (!confirm(`Bạn có chắc chắn muốn xóa người dùng "${userName}"?\n\nLưu ý: Đây là xóa mềm, người dùng sẽ bị vô hiệu hóa nhưng dữ liệu vẫn được giữ lại.`)) {
    return;
  }

  try {
    await deleteUser(userId);
    alert('Xóa người dùng thành công!');
    loadUsers();
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi xóa người dùng'));
  }
};

// Store current filter status
let currentStatusFilter = null;

/**
 * Load course review content
 */
async function loadCourseReview(statusFilter = null) {
  const content = document.getElementById('course-review-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    console.log('Loading course approvals with filter:', statusFilter);
    const approvalsResponse = await getAllCourseApprovals(statusFilter);
    console.log('Approvals response:', approvalsResponse);
    
    if (approvalsResponse.success && approvalsResponse.data) {
      const approvals = approvalsResponse.data || [];
      
      // Map status to Vietnamese
      const statusMap = {
        'ChoKiemDuyet': { text: 'Chờ duyệt', class: 'role-badge role-warning' },
        'DaDuyet': { text: 'Đã duyệt', class: 'role-badge role-user' },
        'TuChoi': { text: 'Từ chối', class: 'role-badge role-admin' },
        'BiAn': { text: 'Bị ẩn', class: 'role-badge role-admin' }
      };

      const getStatusBadge = (status) => {
        const statusInfo = statusMap[status] || { text: status, class: 'role-badge' };
        return `<span class="${statusInfo.class}">${statusInfo.text}</span>`;
      };
      
      content.innerHTML = `
        <div style="margin-bottom: 20px; display: flex; gap: 12px; align-items: center; flex-wrap: wrap;">
          <div style="display: flex; align-items: center; gap: 8px;">
            <label for="status-filter" style="font-weight: 600; color: #334155;">Lọc theo trạng thái:</label>
            <select id="status-filter" class="form-control" style="width: auto; min-width: 180px;" onchange="handleStatusFilterChange(this.value)">
              <option value="">Tất cả</option>
              <option value="ChoKiemDuyet" ${statusFilter === 'ChoKiemDuyet' ? 'selected' : ''}>Chờ duyệt</option>
              <option value="DaDuyet" ${statusFilter === 'DaDuyet' ? 'selected' : ''}>Đã duyệt</option>
              <option value="TuChoi" ${statusFilter === 'TuChoi' ? 'selected' : ''}>Từ chối</option>
              <option value="BiAn" ${statusFilter === 'BiAn' ? 'selected' : ''}>Bị ẩn</option>
            </select>
          </div>
          <div style="margin-left: auto; color: #64748b; font-size: 14px;">
            Tổng: ${approvals.length} khóa học
          </div>
        </div>
        ${approvals.length === 0 ? `
          <div class="empty-state">
            <div class="empty-state-icon">📚</div>
            <div class="empty-state-text">Không có khóa học nào</div>
            <div class="empty-state-subtext">${statusFilter ? 'Không có khóa học với trạng thái đã chọn' : 'Chưa có khóa học nào trong hệ thống'}</div>
          </div>
        ` : `
          <div class="table-container">
            <table class="table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Tên khóa học</th>
                  <th>Người gửi</th>
                  <th>Phiên bản</th>
                  <th>Trạng thái</th>
                  <th>Ngày gửi</th>
                  <th>Ngày kiểm duyệt</th>
                  <th>Người kiểm duyệt</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                ${approvals.map(approval => {
                  const canApprove = approval.trangThaiKiemDuyet === 'ChoKiemDuyet';
                  const canReject = approval.trangThaiKiemDuyet === 'ChoKiemDuyet';
                  const canHide = approval.trangThaiKiemDuyet === 'DaDuyet';
                  const canUnhide = approval.trangThaiKiemDuyet === 'BiAn';
                  
                  return `
                    <tr>
                      <td>${approval.idKhoaHoc}</td>
                      <td><strong>${approval.tenKhoaHoc || 'N/A'}</strong></td>
                      <td>${approval.tenNguoiGui || 'N/A'}</td>
                      <td>${approval.phienBan}</td>
                      <td>${getStatusBadge(approval.trangThaiKiemDuyet)}</td>
                      <td>${approval.ngayGui ? new Date(approval.ngayGui).toLocaleDateString('vi-VN') : 'N/A'}</td>
                      <td>${approval.ngayKiemDuyet ? new Date(approval.ngayKiemDuyet).toLocaleDateString('vi-VN') : '-'}</td>
                      <td>${approval.tenNguoiKiemDuyet || '-'}</td>
                      <td>
                        <div class="actions" style="display: flex; gap: 8px; flex-wrap: wrap;">
                          ${canApprove ? `
                            <button class="btn btn-sm btn-success" onclick="handleApproveCourse(${approval.idKhoaHoc})" title="Duyệt khóa học">
                              <i class="fas fa-check"></i> Duyệt
                            </button>
                          ` : ''}
                          ${canReject ? `
                            <button class="btn btn-sm btn-danger" onclick="handleRejectCourse(${approval.idKhoaHoc})" title="Từ chối khóa học">
                              <i class="fas fa-times"></i> Từ chối
                            </button>
                          ` : ''}
                          ${canHide ? `
                            <button class="btn btn-sm btn-warning" onclick="handleHideCourseByAdmin(${approval.idKhoaHoc})" title="Ẩn khóa học">
                              <i class="fas fa-eye-slash"></i> Ẩn
                            </button>
                          ` : ''}
                          ${canUnhide ? `
                            <button class="btn btn-sm btn-success" onclick="handleUnhideCourseByAdmin(${approval.idKhoaHoc})" title="Bỏ ẩn khóa học">
                              <i class="fas fa-eye"></i> Hiển thị
                            </button>
                          ` : ''}
                          <button class="btn btn-sm btn-primary" onclick="showCourseDetailModal(${approval.idKhoaHoc})" title="Xem chi tiết">
                            <i class="fas fa-eye"></i> Xem
                          </button>
                          ${approval.lyDoTuChoi ? `
                            <button class="btn btn-sm btn-secondary" onclick="showRejectionReason('${(approval.lyDoTuChoi || '').replace(/'/g, "\\'")}')" title="Xem lý do từ chối">
                              <i class="fas fa-info-circle"></i> Lý do
                            </button>
                          ` : ''}
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
      console.error('API response không thành công:', approvalsResponse);
      content.innerHTML = `
        <div class="empty-state">
          <div class="empty-state-icon">⚠️</div>
          <div class="empty-state-text">Không thể tải danh sách khóa học</div>
          <div class="empty-state-subtext">${approvalsResponse.message || 'Lỗi không xác định'}</div>
          <button class="btn btn-primary" onclick="loadCourseReview();" style="margin-top: 16px;">
            <i class="fas fa-redo"></i> Thử lại
          </button>
        </div>
      `;
    }
  } catch (error) {
    console.error('Error loading course review:', error);
    console.error('Error details:', {
      message: error.message,
      stack: error.stack,
      name: error.name
    });
    
    let errorMessage = error.message || 'Lỗi không xác định';
    let errorDetails = '';
    
    if (error.message && error.message.includes('403')) {
      errorMessage = 'Không có quyền truy cập';
      errorDetails = `
        <div style="margin-top: 12px; padding: 12px; background: #fef3c7; border-radius: 8px; color: #92400e;">
          <strong>⚠️ Lỗi 403 Forbidden:</strong><br>
          Bạn không có quyền truy cập endpoint này. Cần role: <strong>QUANTRIVIEN</strong> hoặc <strong>KIEMDUYETVIEN</strong>.
        </div>
      `;
    } else if (error.message && error.message.includes('401')) {
      errorMessage = 'Chưa đăng nhập hoặc token đã hết hạn';
      errorDetails = `
        <div style="margin-top: 12px; padding: 12px; background: #fee2e2; border-radius: 8px; color: #991b1b;">
          <strong>⚠️ Lỗi 401 Unauthorized:</strong><br>
          Vui lòng đăng nhập lại.
        </div>
      `;
    } else if (error.message && error.message.includes('404')) {
      errorMessage = 'Endpoint không tồn tại';
      errorDetails = `
        <div style="margin-top: 12px; padding: 12px; background: #fee2e2; border-radius: 8px; color: #991b1b;">
          <strong>⚠️ Lỗi 404 Not Found:</strong><br>
          Endpoint /api/v1/courses/approvals không tồn tại. Kiểm tra lại backend.
        </div>
      `;
    }
    
    content.innerHTML = `
      <div class="empty-state">
        <div class="empty-state-icon">❌</div>
        <div class="empty-state-text">Có lỗi xảy ra khi tải danh sách khóa học</div>
        <div class="empty-state-subtext">${errorMessage}</div>
        ${errorDetails}
        <button class="btn btn-primary" onclick="loadCourseReview(${statusFilter ? `'${statusFilter}'` : 'null'})" style="margin-top: 16px;">
          <i class="fas fa-redo"></i> Thử lại
        </button>
      </div>
    `;
  }
}

/**
 * Handle status filter change
 */
window.handleStatusFilterChange = function(status) {
  currentStatusFilter = status || null;
  loadCourseReview(currentStatusFilter);
};

/**
 * Handle approve course
 */
window.handleApproveCourse = async function(courseId) {
  const ghiChu = prompt('Ghi chú (tùy chọn):');
  if (ghiChu === null) {
    return; // User cancelled
  }

  if (!confirm('Bạn có chắc chắn muốn duyệt khóa học này?')) {
    return;
  }

  try {
    const response = await approveCourse(courseId, ghiChu || null);
    if (response.success) {
      alert('Duyệt khóa học thành công!');
      loadCourseReview(currentStatusFilter);
    } else {
      alert('Lỗi: ' + (response.message || 'Không thể duyệt khóa học'));
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi duyệt khóa học'));
  }
};

/**
 * Handle reject course
 */
window.handleRejectCourse = async function(courseId) {
  const lyDoTuChoi = prompt('Lý do từ chối (bắt buộc):');
  if (!lyDoTuChoi || lyDoTuChoi.trim() === '') {
    alert('Lý do từ chối không được để trống!');
    return;
  }

  const ghiChu = prompt('Ghi chú (tùy chọn):');
  if (ghiChu === null && lyDoTuChoi !== null) {
    return; // User cancelled ghi chú but already entered reason
  }

  if (!confirm('Bạn có chắc chắn muốn từ chối khóa học này?')) {
    return;
  }

  try {
    const response = await rejectCourse(courseId, lyDoTuChoi.trim(), ghiChu || null);
    if (response.success) {
      alert('Từ chối khóa học thành công!');
      loadCourseReview(currentStatusFilter);
    } else {
      alert('Lỗi: ' + (response.message || 'Không thể từ chối khóa học'));
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi từ chối khóa học'));
  }
};

/**
 * Show rejection reason
 */
window.showRejectionReason = function(reason) {
  alert('Lý do từ chối:\n\n' + reason);
};

/**
 * Handle hide course by admin
 */
window.handleHideCourseByAdmin = async function(courseId) {
  const ghiChu = prompt('Ghi chú (tùy chọn):');
  if (ghiChu === null) {
    return; // User cancelled
  }

  if (!confirm('Bạn có chắc chắn muốn ẩn khóa học này?')) {
    return;
  }

  try {
    const response = await hideCourseByAdmin(courseId, ghiChu || null);
    if (response.success) {
      alert('Ẩn khóa học thành công!');
      loadCourseReview(currentStatusFilter);
    } else {
      alert('Lỗi: ' + (response.message || 'Không thể ẩn khóa học'));
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi ẩn khóa học'));
  }
};

/**
 * Handle unhide course by admin
 */
window.handleUnhideCourseByAdmin = async function(courseId) {
  const ghiChu = prompt('Ghi chú (tùy chọn):');
  if (ghiChu === null) {
    return; // User cancelled
  }

  if (!confirm('Bạn có chắc chắn muốn bỏ ẩn khóa học này? Khóa học sẽ được hiển thị lại công khai.')) {
    return;
  }

  try {
    const response = await unhideCourseByAdmin(courseId, ghiChu || null);
    if (response.success) {
      alert('Bỏ ẩn khóa học thành công!');
      loadCourseReview(currentStatusFilter);
    } else {
      alert('Lỗi: ' + (response.message || 'Không thể bỏ ẩn khóa học'));
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi bỏ ẩn khóa học'));
  }
};

/**
 * Show course detail modal (read-only)
 */
window.showCourseDetailModal = async function(courseId) {
  try {
    // Show loading
    const loadingModal = new Modal({
      id: 'course-detail-loading',
      title: 'Đang tải...',
      content: '<div class="loading-spinner"></div>',
      size: 'small'
    });
    const modalRoot = document.getElementById('modal-root');
    if (modalRoot) {
      modalRoot.innerHTML = loadingModal.render();
      loadingModal.attachEventListeners();
      loadingModal.open();
    }

    // Load course data using admin/reviewer endpoint
    const courseResponse = await getCourseForReview(courseId);
    
    // Close loading modal
    loadingModal.close();
    loadingModal.destroy();

    if (!courseResponse || !courseResponse.success || !courseResponse.data) {
      alert('Không thể tải thông tin khóa học');
      return;
    }

    const courseData = courseResponse.data;
    const course = {
      id: courseId,
      tenKhoaHoc: courseData.tenKhoaHoc,
      moTaNgan: courseData.moTaNgan || '',
      moTaChiTiet: courseData.moTaChiTiet || '',
      giaBan: courseData.giaBan || 0,
      mucDo: courseData.mucDo || 'N/A',
      idDanhMuc: courseData.idDanhMuc,
      tenDanhMuc: 'N/A',
      yeuCauTruoc: courseData.yeuCauTruoc || '',
      hocDuoc: courseData.hocDuoc || '',
      giangVien: null,
      videoGioiThieu: courseData.videoGioiThieu || null
    };

    // Curriculum is included in response
    const curriculum = courseData.chuongs && courseData.chuongs.length > 0 
      ? { chuongs: courseData.chuongs } 
      : null;

    // Load category name
    if (course.idDanhMuc) {
      try {
        const categoryResponse = await getCategoryById(course.idDanhMuc);
        if (categoryResponse && categoryResponse.success && categoryResponse.data) {
          course.tenDanhMuc = categoryResponse.data.tenDanhMuc || 'N/A';
        }
      } catch (error) {
        course.tenDanhMuc = 'N/A';
      }
    }

    // Format price
    const formatPrice = (price) => {
      if (!price || price === 0) return 'Miễn phí';
      return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);
    };

    // Format duration (seconds to HH:MM:SS or MM:SS)
    const formatDuration = (seconds) => {
      if (!seconds || seconds === 0) return '';
      const hours = Math.floor(seconds / 3600);
      const minutes = Math.floor((seconds % 3600) / 60);
      const secs = seconds % 60;
      
      if (hours > 0) {
        return `${hours}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
      }
      return `${minutes}:${secs.toString().padStart(2, '0')}`;
    };

    // Generate curriculum HTML with video players
    let curriculumHtml = '';
    if (curriculum && curriculum.chuongs && curriculum.chuongs.length > 0) {
      curriculumHtml = `
        <div class="form-section">
          <h3 class="form-section-title">Nội dung khóa học</h3>
          ${curriculum.chuongs.map((chuong, chuongIndex) => `
            <div style="margin-bottom: 24px; padding: 16px; border: 1px solid #e2e8f0; border-radius: 8px;">
              <h4 style="margin: 0 0 12px 0; color: #1e293b; font-size: 18px;">
                Chương ${chuongIndex + 1}: ${chuong.tenChuong || 'N/A'}
              </h4>
              ${chuong.moTa ? `<p style="margin: 0 0 16px 0; color: #64748b; font-size: 14px;">${chuong.moTa}</p>` : ''}
              ${chuong.baiGiangs && chuong.baiGiangs.length > 0 ? `
                <div style="margin-left: 0;">
                  ${chuong.baiGiangs.map((baiGiang, baiGiangIndex) => {
                    const videoId = `video-${courseId}-${chuongIndex}-${baiGiangIndex}`;
                    
                    // Build video URL
                    let videoUrl = null;
                    if (baiGiang.duongDanVideo) {
                      const rawPath = baiGiang.duongDanVideo;
                      
                      // Debug: Log video URL info
                      console.log(`[VIDEO DEBUG] Bài giảng ${baiGiangIndex + 1} (${baiGiang.tieuDe}):`, {
                        rawPath: rawPath,
                        pathType: typeof rawPath,
                        startsWithHttp: rawPath.startsWith('http'),
                        startsWithSlash: rawPath.startsWith('/')
                      });
                      
                      // Nếu đã là full URL
                      if (rawPath.startsWith('http://') || rawPath.startsWith('https://')) {
                        videoUrl = rawPath;
                      } 
                      // Nếu là relative path (bắt đầu bằng /)
                      else if (rawPath.startsWith('/')) {
                        // Static files được serve từ root, không phải từ /api/
                        // API_BASE_URL = "http://localhost:5228/api/"
                        // Cần: "http://localhost:5228/uploads/..."
                        const baseUrl = API_BASE_URL.replace('/api/', '');
                        videoUrl = `${baseUrl}${rawPath}`;
                      }
                      // Nếu không có / đầu tiên
                      else {
                        const baseUrl = API_BASE_URL.replace('/api/', '');
                        videoUrl = `${baseUrl}/${rawPath}`;
                      }
                      
                      console.log(`[VIDEO DEBUG] Final video URL:`, videoUrl);
                      console.log(`[VIDEO DEBUG] Testing URL accessibility...`);
                      
                      // Test URL accessibility
                      fetch(videoUrl, { method: 'HEAD' })
                        .then(response => {
                          console.log(`[VIDEO DEBUG] URL test result:`, {
                            url: videoUrl,
                            status: response.status,
                            statusText: response.statusText,
                            ok: response.ok,
                            headers: {
                              contentType: response.headers.get('content-type'),
                              contentLength: response.headers.get('content-length')
                            }
                          });
                        })
                        .catch(error => {
                          console.error(`[VIDEO DEBUG] URL test error:`, {
                            url: videoUrl,
                            error: error.message
                          });
                        });
                    } else {
                      console.warn(`[VIDEO DEBUG] Bài giảng ${baiGiangIndex + 1} không có duongDanVideo`);
                    }
                    
                    return `
                    <div style="padding: 12px 0; border-bottom: 1px solid #f1f5f9;">
                      <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 8px;">
                        <i class="fas fa-play-circle" style="color: #3b82f6;"></i>
                        <span style="font-weight: 500;">Bài ${baiGiangIndex + 1}: ${baiGiang.tieuDe || 'N/A'}</span>
                        ${baiGiang.mienPhiXem ? '<span style="color: #10b981; font-size: 12px;">(Miễn phí)</span>' : ''}
                      </div>
                      ${baiGiang.moTa ? `<p style="margin: 0 0 8px 28px; color: #64748b; font-size: 13px;">${baiGiang.moTa}</p>` : ''}
                      ${videoUrl ? `
                        <div style="margin: 12px 0 0 28px; max-width: 800px;">
                          <div style="width: 100%; aspect-ratio: 16/9; background: #000; border-radius: 4px; overflow: hidden; position: relative;">
                            <video 
                              id="${videoId}"
                              controls 
                              style="width: 100%; height: 100%; object-fit: contain;"
                              preload="metadata"
                              onerror="console.error('[VIDEO ERROR] Video load failed:', {videoId: '${videoId}', url: '${videoUrl}', error: event.target.error})"
                              onloadstart="console.log('[VIDEO] Loading started:', '${videoId}')"
                              oncanplay="console.log('[VIDEO] Can play:', '${videoId}')"
                              onerror="(function(e) { 
                                console.error('[VIDEO ERROR]', {
                                  videoId: '${videoId}',
                                  url: '${videoUrl}',
                                  error: e.target.error,
                                  networkState: e.target.networkState,
                                  readyState: e.target.readyState
                                });
                                const errorDiv = document.createElement('div');
                                errorDiv.style.cssText = 'position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); color: white; text-align: center; padding: 16px; background: rgba(0,0,0,0.8); border-radius: 4px;';
                                errorDiv.innerHTML = '<i class=\"fas fa-exclamation-triangle\"></i><br>Không thể tải video<br><small style=\"font-size: 11px;\">' + '${videoUrl}' + '</small>';
                                e.target.parentElement.appendChild(errorDiv);
                              })(event)"
                            >
                              <source src="${videoUrl}" type="video/mp4">
                              <source src="${videoUrl}" type="video/webm">
                              <source src="${videoUrl}" type="video/ogg">
                              Trình duyệt của bạn không hỗ trợ video.
                            </video>
                          </div>
                          <p style="margin: 4px 0 0 0; color: #64748b; font-size: 12px;">
                            <i class="fas fa-video"></i> Video bài giảng
                            ${baiGiang.thoiLuong ? ` • ${formatDuration(baiGiang.thoiLuong)}` : ''}
                            <br><small style="color: #94a3b8; font-size: 10px; word-break: break-all;">${videoUrl}</small>
                          </p>
                        </div>
                      ` : `
                        <div style="margin: 12px 0 0 28px; padding: 12px; background: #fef3c7; border-radius: 4px; color: #92400e; font-size: 13px;">
                          <i class="fas fa-exclamation-triangle"></i> Chưa có video cho bài giảng này
                        </div>
                      `}
                    </div>
                  `;
                  }).join('')}
                </div>
              ` : '<p style="color: #94a3b8; font-size: 13px; margin-left: 16px;">Chưa có bài giảng</p>'}
            </div>
          `).join('')}
        </div>
      `;
    } else {
      curriculumHtml = '<p style="color: #94a3b8;">Chưa có nội dung khóa học</p>';
    }

    // Generate video introduction if available
    let videoIntroHtml = '';
    if (course.videoGioiThieu) {
      console.log('[VIDEO DEBUG] Video giới thiệu:', {
        raw: course.videoGioiThieu,
        startsWithHttp: course.videoGioiThieu.startsWith('http'),
        startsWithSlash: course.videoGioiThieu.startsWith('/')
      });
      
      let introVideoUrl = null;
      const rawPath = course.videoGioiThieu;
      if (rawPath.startsWith('http://') || rawPath.startsWith('https://')) {
        introVideoUrl = rawPath;
      } else if (rawPath.startsWith('/')) {
        const baseUrl = API_BASE_URL.replace('/api/', '');
        introVideoUrl = `${baseUrl}${rawPath}`;
      } else {
        const baseUrl = API_BASE_URL.replace('/api/', '');
        introVideoUrl = `${baseUrl}/${rawPath}`;
      }
      
      console.log('[VIDEO DEBUG] Final intro video URL:', introVideoUrl);
      
      videoIntroHtml = `
        <div class="form-section" style="margin-bottom: 24px;">
          <h3 class="form-section-title">Video giới thiệu khóa học</h3>
          <div style="width: 100%; max-width: 800px; aspect-ratio: 16/9; background: #000; border-radius: 8px; overflow: hidden; margin-top: 12px;">
            <video 
              id="intro-video-${courseId}"
              controls 
              style="width: 100%; height: 100%; object-fit: contain;"
              preload="metadata"
            >
              <source src="${introVideoUrl}" type="video/mp4">
              <source src="${introVideoUrl}" type="video/webm">
              <source src="${introVideoUrl}" type="video/ogg">
              Trình duyệt của bạn không hỗ trợ video.
            </video>
          </div>
        </div>
      `;
    }

    // Generate modal content
    const modalContent = `
      <div style="max-height: 70vh; overflow-y: auto;">
        ${videoIntroHtml}
        <div class="form-section">
          <h3 class="form-section-title">Thông tin cơ bản</h3>
          
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 16px;">
            <div>
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Danh mục</label>
              <p style="margin: 0; color: #1e293b;">${course.tenDanhMuc || 'N/A'}</p>
            </div>
            <div>
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Mức độ</label>
              <p style="margin: 0; color: #1e293b;">${course.mucDo || 'N/A'}</p>
            </div>
          </div>

          <div style="margin-bottom: 16px;">
            <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Tên khóa học</label>
            <p style="margin: 0; color: #1e293b; font-size: 18px; font-weight: 500;">${course.tenKhoaHoc || 'N/A'}</p>
          </div>

          ${course.moTaNgan ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Mô tả ngắn</label>
              <p style="margin: 0; color: #1e293b;">${course.moTaNgan}</p>
            </div>
          ` : ''}

          ${course.moTaChiTiet ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Mô tả chi tiết</label>
              <p style="margin: 0; color: #1e293b; white-space: pre-wrap;">${course.moTaChiTiet}</p>
            </div>
          ` : ''}

          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 16px;">
            <div>
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Giá bán</label>
              <p style="margin: 0; color: #1e293b; font-weight: 600; font-size: 18px;">${formatPrice(course.giaBan)}</p>
            </div>
            ${course.giangVien ? `
              <div>
                <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Giảng viên</label>
                <p style="margin: 0; color: #1e293b;">${course.giangVien.hoTen || 'N/A'}</p>
              </div>
            ` : ''}
          </div>

          ${course.yeuCauTruoc ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Yêu cầu trước</label>
              <p style="margin: 0; color: #1e293b; white-space: pre-wrap;">${course.yeuCauTruoc}</p>
            </div>
          ` : ''}

          ${course.hocDuoc ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Bạn sẽ học được gì</label>
              <p style="margin: 0; color: #1e293b; white-space: pre-wrap;">${course.hocDuoc}</p>
            </div>
          ` : ''}

          ${course.diemDanhGia ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Đánh giá</label>
              <p style="margin: 0; color: #1e293b;">
                ${course.diemDanhGia.toFixed(1)} ⭐ (${course.soLuongDanhGia || 0} đánh giá)
              </p>
            </div>
          ` : ''}

          ${course.soLuongHocVien !== undefined ? `
            <div style="margin-bottom: 16px;">
              <label style="display: block; font-weight: 600; margin-bottom: 4px; color: #475569;">Số lượng học viên</label>
              <p style="margin: 0; color: #1e293b;">${course.soLuongHocVien || 0} học viên</p>
            </div>
          ` : ''}
        </div>

        ${curriculumHtml}
      </div>
    `;

    // Create and show modal
    const detailModal = new Modal({
      id: 'course-detail-modal',
      title: 'Chi tiết khóa học',
      content: modalContent,
      size: 'large',
      footer: `
        <button type="button" class="btn btn-secondary" onclick="window.closeCourseDetailModal()">Đóng</button>
      `
    });

    if (modalRoot) {
      modalRoot.innerHTML = detailModal.render();
      detailModal.attachEventListeners();
      detailModal.open();
    }

    // Store modal reference for close function
    window.currentCourseDetailModal = detailModal;
  } catch (error) {
    console.error('Error loading course detail:', error);
    alert('Lỗi khi tải thông tin khóa học: ' + (error.message || 'Lỗi không xác định'));
  }
};

/**
 * Close course detail modal
 */
window.closeCourseDetailModal = function() {
  if (window.currentCourseDetailModal) {
    window.currentCourseDetailModal.close();
    window.currentCourseDetailModal.destroy();
    window.currentCourseDetailModal = null;
  }
};

/**
 * Load categories content
 */
async function loadCategories() {
  const content = document.getElementById('categories-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    // Use /all endpoint for admin to get all categories including inactive
    const categoriesResponse = await getCategories(1, 100);
    
    if (categoriesResponse.success && categoriesResponse.data) {
      const categories = categoriesResponse.data.items || [];
      
      content.innerHTML = `
        <div style="margin-bottom: 20px;">
          <button class="btn btn-primary" onclick="alert('Tính năng thêm danh mục sẽ được thêm sau')">
            <i class="fas fa-plus"></i> Thêm danh mục mới
          </button>
        </div>
        ${categories.length === 0 ? `
          <div class="empty-state">
            <div class="empty-state-icon">📁</div>
            <div class="empty-state-text">Chưa có danh mục nào</div>
          </div>
        ` : `
          <div class="table-container">
            <table class="table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Tên danh mục</th>
                  <th>Mô tả</th>
                  <th>Trạng thái</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                ${categories.map(category => {
                  const statusBadge = category.trangThai 
                    ? '<span class="role-badge role-user">Hoạt động</span>'
                    : '<span class="role-badge role-admin">Khóa</span>';
                  
                  return `
                    <tr>
                      <td>${category.id}</td>
                      <td>${category.tenDanhMuc || 'N/A'}</td>
                      <td>${category.moTa || 'N/A'}</td>
                      <td>${statusBadge}</td>
                      <td>
                        <button class="btn btn-sm btn-secondary" onclick="alert('Tính năng sửa danh mục sẽ được thêm sau')">Sửa</button>
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
      content.innerHTML = '<p>Không thể tải danh sách danh mục.</p>';
    }
  } catch (error) {
    console.error('Error loading categories:', error);
    content.innerHTML = '<p>Có lỗi xảy ra khi tải danh sách danh mục.</p>';
  }
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
