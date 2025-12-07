/**
 * Admin Dashboard Page
 * Main page for admin management
 */

import { AdminHeader } from '../../components/admin/AdminHeader.js';
import { AdminSidebar } from '../../components/admin/AdminSidebar.js';
import { requireAuth } from '../../api/authApi.js';
import { isAdmin, getUserInfo } from '../../utils/authHelper.js';
import { getUserFromToken } from '../../utils/token.js';
import { getAllUsers, deleteUser, restoreUser } from '../../api/nguoiDungApi.js';
import { getCourses } from '../../api/courseApi.js';
import { getAllCategories, getCategories, getCategoryById, getAllCategoriesAdmin, deleteCategory, restoreCategory } from '../../api/categoryApi.js';
import { getInstructorRequests, getInstructorRequestById, approveInstructorRequest as approveRequest, rejectInstructorRequest as rejectRequest } from '../../api/instructorRequestApi.js';
import { Modal } from '../../components/admin/modal.js';
import { API_BASE_URL } from '../../config.js';
import { showUserFormModal, showEditUserFormModal } from './user-form-modal.js';
import { showAddCategoryModal, showEditCategoryModal } from './category-form-modal.js';

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
 * Show add category modal
 */
window.showAddCategoryModal = function() {
  showAddCategoryModal(() => {
    loadCategories();
  });
};

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
    case 'instructor-requests':
      loadInstructorRequests();
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
    // Load both active and inactive users
    let activeUsersResponse, inactiveUsersResponse;
    
    try {
      [activeUsersResponse, inactiveUsersResponse] = await Promise.all([
        getAllUsers({ pageNumber: 1, pageSize: 1000, active: true }),
        getAllUsers({ pageNumber: 1, pageSize: 1000, active: false })
      ]);
    } catch (apiError) {
      console.error('API Error:', apiError);
      throw new Error(apiError.message || 'Không thể kết nối đến server');
    }
    
    let allUsers = [];
    
    // Check if responses are valid
    if (activeUsersResponse && activeUsersResponse.success && activeUsersResponse.data) {
      allUsers = [...(activeUsersResponse.data.items || [])];
    } else if (activeUsersResponse && !activeUsersResponse.success) {
      console.warn('Active users response failed:', activeUsersResponse);
    }
    
    if (inactiveUsersResponse && inactiveUsersResponse.success && inactiveUsersResponse.data) {
      allUsers = [...allUsers, ...(inactiveUsersResponse.data.items || [])];
    } else if (inactiveUsersResponse && !inactiveUsersResponse.success) {
      console.warn('Inactive users response failed:', inactiveUsersResponse);
    }
    
    // Helper function to format roles
    const formatRoles = (user) => {
      const roles = user.vaiTros || user.VaiTros || user.nguoiDungVaiTros || [];
      if (Array.isArray(roles) && roles.length > 0) {
        // If roles is array of strings
        if (typeof roles[0] === 'string') {
          return roles.map(r => {
            const roleName = r.toUpperCase();
            const roleLabels = {
              'ADMIN': 'Admin',
              'GIANGVIEN': 'Giảng viên',
              'HOCVIEN': 'Học viên',
              'KIEMDUYET': 'Kiểm duyệt'
            };
            return `<span class="role-badge" style="background: #e0e7ff; color: #3730a3; padding: 2px 8px; border-radius: 12px; font-size: 12px; margin-right: 4px;">${roleLabels[roleName] || r}</span>`;
          }).join('');
        }
        // If roles is array of objects
        if (typeof roles[0] === 'object' && roles[0].tenVaiTro) {
          return roles.map(r => {
            const roleName = (r.tenVaiTro || r.TenVaiTro || '').toUpperCase();
            const roleLabels = {
              'ADMIN': 'Admin',
              'GIANGVIEN': 'Giảng viên',
              'HOCVIEN': 'Học viên',
              'KIEMDUYET': 'Kiểm duyệt'
            };
            return `<span class="role-badge" style="background: #e0e7ff; color: #3730a3; padding: 2px 8px; border-radius: 12px; font-size: 12px; margin-right: 4px;">${roleLabels[roleName] || roleName}</span>`;
          }).join('');
        }
      }
      return '<span style="color: #94a3b8;">Chưa có vai trò</span>';
    };
    
    content.innerHTML = `
      <div style="margin-bottom: 20px; display: flex; gap: 12px;">
        <button class="btn btn-primary" onclick="window.showAddUserModal()">
          <i class="fas fa-plus"></i> Thêm người dùng mới
        </button>
      </div>
      ${allUsers.length === 0 ? `
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
                <th>Vai trò</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              ${allUsers.map(user => {
                const statusBadge = user.trangThai 
                  ? '<span class="role-badge" style="background: #d1fae5; color: #065f46;">🟢 Hoạt động</span>'
                  : '<span class="role-badge" style="background: #fee2e2; color: #991b1b;">🔴 Đã khóa</span>';
                
                const rolesHtml = formatRoles(user);
                
                return `
                  <tr style="${!user.trangThai ? 'opacity: 0.7;' : ''}">
                    <td>${user.id}</td>
                    <td>${user.hoTen || 'N/A'}</td>
                    <td>${user.email || 'N/A'}</td>
                    <td>${user.soDienThoai || 'N/A'}</td>
                    <td>${rolesHtml}</td>
                    <td>${statusBadge}</td>
                    <td>
                      <div class="actions" style="display: flex; gap: 8px;">
                        <button class="btn btn-sm btn-secondary" onclick="window.showEditUserModal(${user.id})">
                          <i class="fas fa-edit"></i> Sửa
                        </button>
                        ${user.trangThai ? `
                          <button class="btn btn-sm btn-danger" onclick="window.handleDeleteUser(${user.id}, '${(user.hoTen || '').replace(/'/g, "\\'")}')">
                            <i class="fas fa-trash"></i> Xóa
                          </button>
                        ` : `
                          <button class="btn btn-sm btn-success" onclick="window.handleRestoreUser(${user.id}, '${(user.hoTen || '').replace(/'/g, "\\'")}')">
                            <i class="fas fa-undo"></i> Khôi phục
                          </button>
                        `}
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
  } catch (error) {
    console.error('Error loading users:', error);
    const errorMessage = error.message || 'Có lỗi xảy ra khi tải danh sách người dùng';
    content.innerHTML = `
      <div style="padding: 20px; background: #fee2e2; border-radius: 8px; border: 1px solid #fca5a5;">
        <h3 style="color: #991b1b; margin-bottom: 10px;">❌ Lỗi khi tải danh sách người dùng</h3>
        <p style="color: #7f1d1d; margin-bottom: 10px;">${errorMessage}</p>
        <button class="btn btn-primary" onclick="loadUsers()" style="margin-top: 10px;">
          <i class="fas fa-refresh"></i> Thử lại
        </button>
      </div>
    `;
  }
}

// Expose loadUsers to global scope for onclick handlers
window.loadUsers = loadUsers;

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
 * Show add category modal (exposed to window for onclick handlers)
 */
window.showAddCategoryModal = function() {
  showAddCategoryModal(() => {
    loadCategories();
  });
};

/**
 * Show edit category modal (exposed to window for onclick handlers)
 */
window.showEditCategoryModal = function(categoryId) {
  showEditCategoryModal(categoryId, () => {
    loadCategories();
  });
};

/**
 * Handle hide category (soft delete)
 */
window.handleHideCategory = async function(categoryId, categoryName) {
  if (!confirm(`Bạn có chắc chắn muốn ẩn danh mục "${categoryName}"?\n\nDanh mục sẽ bị ẩn khỏi trang công khai nhưng vẫn được lưu trong hệ thống.`)) {
    return;
  }

  try {
    await deleteCategory(categoryId);
    alert('Ẩn danh mục thành công!');
    loadCategories();
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi ẩn danh mục'));
  }
};

/**
 * Handle restore category
 */
window.handleRestoreCategory = async function(categoryId, categoryName) {
  if (!confirm(`Bạn có chắc chắn muốn khôi phục danh mục "${categoryName}"?\n\nDanh mục sẽ được hiển thị lại trên trang công khai.`)) {
    return;
  }

  try {
    await restoreCategory(categoryId);
    alert('Khôi phục danh mục thành công!');
    loadCategories();
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi khôi phục danh mục'));
  }
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

/**
 * Handle restore user
 */
window.handleRestoreUser = async function(userId, userName) {
  if (!confirm(`Bạn có chắc chắn muốn khôi phục người dùng "${userName}"?\n\nNgười dùng sẽ được kích hoạt lại.`)) {
    return;
  }

  try {
    await restoreUser(userId);
    alert('Khôi phục người dùng thành công!');
    loadUsers();
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi khôi phục người dùng'));
  }
};

/**
 * Load instructor requests content
 */
async function loadInstructorRequests() {
  const content = document.getElementById('instructor-requests-content');
  if (!content) return;

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    // Load pending requests by default
    const response = await getInstructorRequests({ 
      trangThai: 'Chờ duyệt',
      pageNumber: 1,
      pageSize: 50
    });
    
    if (response.success && response.data) {
      const requests = response.data.items || [];
      const totalCount = response.data.totalCount || 0;
      
      // Filter buttons
      const filterButtons = `
        <div style="margin-bottom: 20px; display: flex; gap: 12px; flex-wrap: wrap;">
          <button class="btn btn-primary" onclick="loadInstructorRequestsByStatus('Chờ duyệt')" id="filter-pending">
            <i class="fas fa-clock"></i> Chờ duyệt
          </button>
          <button class="btn btn-secondary" onclick="loadInstructorRequestsByStatus('Đã duyệt')" id="filter-approved">
            <i class="fas fa-check"></i> Đã duyệt
          </button>
          <button class="btn btn-secondary" onclick="loadInstructorRequestsByStatus('Từ chối')" id="filter-rejected">
            <i class="fas fa-times"></i> Đã từ chối
          </button>
          <button class="btn btn-secondary" onclick="loadInstructorRequestsByStatus(null)" id="filter-all">
            <i class="fas fa-list"></i> Tất cả
          </button>
        </div>
      `;
      
      if (requests.length === 0) {
        content.innerHTML = filterButtons + `
          <div class="empty-state">
            <div class="empty-state-icon">👨‍🏫</div>
            <div class="empty-state-text">Không có yêu cầu đăng ký giảng viên nào</div>
            <div class="empty-state-subtext">Tất cả yêu cầu đã được xử lý</div>
          </div>
        `;
        return;
      }
      
      content.innerHTML = filterButtons + `
        <div class="table-container">
          <table class="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Họ tên</th>
                <th>Email</th>
                <th>Ngày gửi</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              ${requests.map(request => {
                const status = request.trangThai || request.TrangThai || 'Chờ duyệt';
                const ngayGui = request.ngayGui || request.NgayGui;
                const formattedDate = ngayGui ? new Date(ngayGui).toLocaleDateString('vi-VN') : 'N/A';
                
                let statusBadge = '';
                if (status === 'Chờ duyệt' || status === 'Cho duyet') {
                  statusBadge = '<span class="role-badge" style="background: #fef3c7; color: #92400e;">⏳ Chờ duyệt</span>';
                } else if (status === 'Đã duyệt' || status === 'Da duyet') {
                  statusBadge = '<span class="role-badge" style="background: #d1fae5; color: #065f46;">✅ Đã duyệt</span>';
                } else if (status === 'Từ chối' || status === 'Tu choi') {
                  statusBadge = '<span class="role-badge" style="background: #fee2e2; color: #991b1b;">❌ Từ chối</span>';
                }
                
                const canApprove = status === 'Chờ duyệt' || status === 'Cho duyet';
                const canReject = status === 'Chờ duyệt' || status === 'Cho duyet';
                
                return `
                  <tr>
                    <td>${request.id || request.Id}</td>
                    <td><strong>${request.hoTen || request.HoTen || 'N/A'}</strong></td>
                    <td>${request.email || request.Email || 'N/A'}</td>
                    <td>${formattedDate}</td>
                    <td>${statusBadge}</td>
                    <td>
                      <div class="actions" style="display: flex; gap: 8px; flex-wrap: wrap;">
                        <button class="btn btn-sm btn-info" onclick="window.viewInstructorRequestDetail(${request.id || request.Id})">
                          <i class="fas fa-eye"></i> Chi tiết
                        </button>
                        ${canApprove ? `
                          <button class="btn btn-sm btn-success" onclick="window.approveInstructorRequest(${request.id || request.Id})">
                            <i class="fas fa-check"></i> Duyệt
                          </button>
                        ` : ''}
                        ${canReject ? `
                          <button class="btn btn-sm btn-danger" onclick="window.rejectInstructorRequest(${request.id || request.Id})">
                            <i class="fas fa-times"></i> Từ chối
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
        ${totalCount > requests.length ? `
          <div style="margin-top: 20px; text-align: center; color: #64748b;">
            Hiển thị ${requests.length} / ${totalCount} yêu cầu
          </div>
        ` : ''}
      `;
    } else {
      content.innerHTML = `
        <div style="padding: 20px; background: #fee2e2; border-radius: 8px; border: 1px solid #fca5a5;">
          <h3 style="color: #991b1b; margin-bottom: 10px;">❌ Lỗi khi tải danh sách</h3>
          <p style="color: #7f1d1d;">Không thể lấy dữ liệu từ server. Vui lòng thử lại.</p>
          <button class="btn btn-primary" onclick="loadInstructorRequests()" style="margin-top: 10px;">
            <i class="fas fa-refresh"></i> Thử lại
          </button>
        </div>
      `;
    }
  } catch (error) {
    console.error('Error loading instructor requests:', error);
    const errorMessage = error.message || 'Có lỗi xảy ra khi tải danh sách yêu cầu đăng ký giảng viên';
    content.innerHTML = `
      <div style="padding: 20px; background: #fee2e2; border-radius: 8px; border: 1px solid #fca5a5;">
        <h3 style="color: #991b1b; margin-bottom: 10px;">❌ Lỗi khi tải danh sách</h3>
        <p style="color: #7f1d1d; margin-bottom: 10px;">${errorMessage}</p>
        <button class="btn btn-primary" onclick="loadInstructorRequests()" style="margin-top: 10px;">
          <i class="fas fa-refresh"></i> Thử lại
        </button>
      </div>
    `;
  }
}

/**
 * Load instructor requests by status
 */
window.loadInstructorRequestsByStatus = async function(status) {
  const content = document.getElementById('instructor-requests-content');
  if (!content) return;

  // Update active filter button
  document.querySelectorAll('#instructor-requests-content .btn').forEach(btn => {
    if (btn.id && btn.id.startsWith('filter-')) {
      btn.classList.remove('btn-primary');
      btn.classList.add('btn-secondary');
    }
  });
  
  const filterMap = {
    'Chờ duyệt': 'filter-pending',
    'Đã duyệt': 'filter-approved',
    'Từ chối': 'filter-rejected',
    null: 'filter-all'
  };
  
  const activeFilterId = filterMap[status];
  if (activeFilterId) {
    const activeBtn = document.getElementById(activeFilterId);
    if (activeBtn) {
      activeBtn.classList.remove('btn-secondary');
      activeBtn.classList.add('btn-primary');
    }
  }

  content.innerHTML = '<div class="loading-spinner"></div> Đang tải...';

  try {
    const params = { pageNumber: 1, pageSize: 50 };
    if (status) {
      params.trangThai = status;
    }
    
    const response = await getInstructorRequests(params);
    
    if (response.success && response.data) {
      const requests = response.data.items || [];
      const totalCount = response.data.totalCount || 0;
      
      // Filter buttons
      const filterButtons = `
        <div style="margin-bottom: 20px; display: flex; gap: 12px; flex-wrap: wrap;">
          <button class="btn ${status === 'Chờ duyệt' ? 'btn-primary' : 'btn-secondary'}" onclick="loadInstructorRequestsByStatus('Chờ duyệt')" id="filter-pending">
            <i class="fas fa-clock"></i> Chờ duyệt
          </button>
          <button class="btn ${status === 'Đã duyệt' ? 'btn-primary' : 'btn-secondary'}" onclick="loadInstructorRequestsByStatus('Đã duyệt')" id="filter-approved">
            <i class="fas fa-check"></i> Đã duyệt
          </button>
          <button class="btn ${status === 'Từ chối' ? 'btn-primary' : 'btn-secondary'}" onclick="loadInstructorRequestsByStatus('Từ chối')" id="filter-rejected">
            <i class="fas fa-times"></i> Đã từ chối
          </button>
          <button class="btn ${status === null ? 'btn-primary' : 'btn-secondary'}" onclick="loadInstructorRequestsByStatus(null)" id="filter-all">
            <i class="fas fa-list"></i> Tất cả
          </button>
        </div>
      `;
      
      if (requests.length === 0) {
        content.innerHTML = filterButtons + `
          <div class="empty-state">
            <div class="empty-state-icon">👨‍🏫</div>
            <div class="empty-state-text">Không có yêu cầu đăng ký giảng viên nào</div>
            <div class="empty-state-subtext">${status ? `Không có yêu cầu ở trạng thái "${status}"` : 'Tất cả yêu cầu đã được xử lý'}</div>
          </div>
        `;
        return;
      }
      
      content.innerHTML = filterButtons + `
        <div class="table-container">
          <table class="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Họ tên</th>
                <th>Email</th>
                <th>Ngày gửi</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              ${requests.map(request => {
                const requestStatus = request.trangThai || request.TrangThai || 'Chờ duyệt';
                const ngayGui = request.ngayGui || request.NgayGui;
                const formattedDate = ngayGui ? new Date(ngayGui).toLocaleDateString('vi-VN') : 'N/A';
                
                let statusBadge = '';
                if (requestStatus === 'Chờ duyệt' || requestStatus === 'Cho duyet') {
                  statusBadge = '<span class="role-badge" style="background: #fef3c7; color: #92400e;">⏳ Chờ duyệt</span>';
                } else if (requestStatus === 'Đã duyệt' || requestStatus === 'Da duyet') {
                  statusBadge = '<span class="role-badge" style="background: #d1fae5; color: #065f46;">✅ Đã duyệt</span>';
                } else if (requestStatus === 'Từ chối' || requestStatus === 'Tu choi') {
                  statusBadge = '<span class="role-badge" style="background: #fee2e2; color: #991b1b;">❌ Từ chối</span>';
                }
                
                const canApprove = requestStatus === 'Chờ duyệt' || requestStatus === 'Cho duyet';
                const canReject = requestStatus === 'Chờ duyệt' || requestStatus === 'Cho duyet';
                
                return `
                  <tr>
                    <td>${request.id || request.Id}</td>
                    <td><strong>${request.hoTen || request.HoTen || 'N/A'}</strong></td>
                    <td>${request.email || request.Email || 'N/A'}</td>
                    <td>${formattedDate}</td>
                    <td>${statusBadge}</td>
                    <td>
                      <div class="actions" style="display: flex; gap: 8px; flex-wrap: wrap;">
                        <button class="btn btn-sm btn-info" onclick="window.viewInstructorRequestDetail(${request.id || request.Id})">
                          <i class="fas fa-eye"></i> Chi tiết
                        </button>
                        ${canApprove ? `
                          <button class="btn btn-sm btn-success" onclick="window.approveInstructorRequest(${request.id || request.Id})">
                            <i class="fas fa-check"></i> Duyệt
                          </button>
                        ` : ''}
                        ${canReject ? `
                          <button class="btn btn-sm btn-danger" onclick="window.rejectInstructorRequest(${request.id || request.Id})">
                            <i class="fas fa-times"></i> Từ chối
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
        ${totalCount > requests.length ? `
          <div style="margin-top: 20px; text-align: center; color: #64748b;">
            Hiển thị ${requests.length} / ${totalCount} yêu cầu
          </div>
        ` : ''}
      `;
    } else {
      content.innerHTML = `
        <div style="padding: 20px; background: #fee2e2; border-radius: 8px; border: 1px solid #fca5a5;">
          <h3 style="color: #991b1b; margin-bottom: 10px;">❌ Lỗi khi tải danh sách</h3>
          <p style="color: #7f1d1d;">Không thể lấy dữ liệu từ server. Vui lòng thử lại.</p>
          <button class="btn btn-primary" onclick="loadInstructorRequestsByStatus('${status || ''}')" style="margin-top: 10px;">
            <i class="fas fa-refresh"></i> Thử lại
          </button>
        </div>
      `;
    }
  } catch (error) {
    console.error('Error loading instructor requests:', error);
    const errorMessage = error.message || 'Có lỗi xảy ra khi tải danh sách yêu cầu đăng ký giảng viên';
    content.innerHTML = `
      <div style="padding: 20px; background: #fee2e2; border-radius: 8px; border: 1px solid #fca5a5;">
        <h3 style="color: #991b1b; margin-bottom: 10px;">❌ Lỗi khi tải danh sách</h3>
        <p style="color: #7f1d1d; margin-bottom: 10px;">${errorMessage}</p>
        <button class="btn btn-primary" onclick="loadInstructorRequestsByStatus('${status || ''}')" style="margin-top: 10px;">
          <i class="fas fa-refresh"></i> Thử lại
        </button>
      </div>
    `;
  }
};

/**
 * View instructor request detail
 */
window.viewInstructorRequestDetail = async function(requestId) {
  try {
    const response = await getInstructorRequestById(requestId);
    
    if (response.success && response.data) {
      const request = response.data;
      const status = request.trangThai || request.TrangThai || 'Chờ duyệt';
      const chungChiPath = request.chungChiPath || request.ChungChiPath || '';
      const API_BASE = 'http://localhost:5228';
      const chungChiUrl = chungChiPath.startsWith('http') ? chungChiPath : `${API_BASE}${chungChiPath}`;
      
      const canApprove = status === 'Chờ duyệt' || status === 'Cho duyet';
      const canReject = status === 'Chờ duyệt' || status === 'Cho duyet';
      
      const modalContent = `
        <div style="max-width: 800px; max-height: 90vh; overflow-y: auto;">
          <h2 style="margin-bottom: 20px; color: #1e293b;">Chi tiết yêu cầu đăng ký làm giảng viên</h2>
          
          <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;">
            <h3 style="margin-bottom: 15px; color: #334155;">Thông tin yêu cầu</h3>
            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
              <div>
                <strong>ID yêu cầu:</strong> ${request.id || request.Id}
              </div>
              <div>
                <strong>Trạng thái:</strong> 
                ${status === 'Chờ duyệt' ? '<span style="color: #92400e;">⏳ Chờ duyệt</span>' : ''}
                ${status === 'Đã duyệt' ? '<span style="color: #065f46;">✅ Đã duyệt</span>' : ''}
                ${status === 'Từ chối' ? '<span style="color: #991b1b;">❌ Từ chối</span>' : ''}
              </div>
              <div>
                <strong>Ngày gửi:</strong> ${request.ngayGui || request.NgayGui ? new Date(request.ngayGui || request.NgayGui).toLocaleString('vi-VN') : 'N/A'}
              </div>
              ${request.ngayDuyet || request.NgayDuyet ? `
                <div>
                  <strong>Ngày duyệt:</strong> ${new Date(request.ngayDuyet || request.NgayDuyet).toLocaleString('vi-VN')}
                </div>
              ` : ''}
              ${request.tenNguoiDuyet || request.TenNguoiDuyet ? `
                <div>
                  <strong>Người duyệt:</strong> ${request.tenNguoiDuyet || request.TenNguoiDuyet}
                </div>
              ` : ''}
            </div>
            ${request.lyDoTuChoi || request.LyDoTuChoi ? `
              <div style="margin-top: 15px; padding: 12px; background: #fee2e2; border-radius: 6px;">
                <strong>Lý do từ chối:</strong> ${request.lyDoTuChoi || request.LyDoTuChoi}
              </div>
            ` : ''}
          </div>
          
          <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;">
            <h3 style="margin-bottom: 15px; color: #334155;">Thông tin học viên</h3>
            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
              <div>
                <strong>Họ tên:</strong> ${request.hoTen || request.HoTen || 'N/A'}
              </div>
              <div>
                <strong>Email:</strong> ${request.email || request.Email || 'N/A'}
              </div>
              <div>
                <strong>ID học viên:</strong> ${request.idHocVien || request.IdHocVien || 'N/A'}
              </div>
            </div>
          </div>
          
          <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;">
            <h3 style="margin-bottom: 15px; color: #334155;">Chứng chỉ</h3>
            ${chungChiPath ? `
              <div style="margin-top: 10px;">
                ${chungChiPath.toLowerCase().endsWith('.pdf') ? `
                  <a href="${chungChiUrl}" target="_blank" class="btn btn-info" style="display: inline-block; margin-bottom: 10px;">
                    <i class="fas fa-file-pdf"></i> Xem chứng chỉ (PDF)
                  </a>
                ` : `
                  <div style="margin-bottom: 10px;">
                    <img src="${chungChiUrl}" alt="Chứng chỉ" style="max-width: 100%; border-radius: 8px; border: 1px solid #ddd;">
                  </div>
                  <a href="${chungChiUrl}" target="_blank" class="btn btn-info" style="display: inline-block;">
                    <i class="fas fa-external-link-alt"></i> Mở ảnh trong tab mới
                  </a>
                `}
              </div>
            ` : '<p style="color: #94a3b8;">Chưa có chứng chỉ</p>'}
          </div>
          
          ${request.thongTinBoSung || request.ThongTinBoSung ? `
            <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;">
              <h3 style="margin-bottom: 15px; color: #334155;">Thông tin bổ sung</h3>
              <div style="padding: 12px; background: white; border-radius: 6px; max-height: 200px; overflow-y: auto;">
                ${request.thongTinBoSung || request.ThongTinBoSung}
              </div>
            </div>
          ` : ''}
          
          <div style="display: flex; gap: 12px; justify-content: flex-end; margin-top: 20px;">
            ${canApprove ? `
              <button class="btn btn-success" onclick="window.approveInstructorRequest(${request.id || request.Id}); Modal.close();">
                <i class="fas fa-check"></i> Duyệt yêu cầu
              </button>
            ` : ''}
            ${canReject ? `
              <button class="btn btn-danger" onclick="window.rejectInstructorRequest(${request.id || request.Id}); Modal.close();">
                <i class="fas fa-times"></i> Từ chối
              </button>
            ` : ''}
            <button class="btn btn-secondary" onclick="Modal.close()">
              Đóng
            </button>
          </div>
        </div>
      `;
      
      Modal.show({
        title: 'Chi tiết yêu cầu đăng ký làm giảng viên',
        content: modalContent,
        size: 'large'
      });
    } else {
      alert('Không thể tải chi tiết yêu cầu');
    }
  } catch (error) {
    console.error('Error loading instructor request detail:', error);
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi tải chi tiết yêu cầu'));
  }
};

/**
 * Approve instructor request
 */
window.approveInstructorRequest = async function(requestId) {
  if (!confirm('Bạn có chắc chắn muốn duyệt yêu cầu đăng ký làm giảng viên này?\n\nHọc viên sẽ được cấp quyền giảng viên và có thể tạo khóa học.')) {
    return;
  }
  
  try {
    await approveRequest(requestId);
    alert('Duyệt yêu cầu thành công! Học viên đã được cấp quyền giảng viên.');
    loadInstructorRequests();
    if (Modal.isOpen()) {
      Modal.close();
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi duyệt yêu cầu'));
  }
};

/**
 * Reject instructor request
 */
window.rejectInstructorRequest = async function(requestId) {
  const lyDoTuChoi = prompt('Nhập lý do từ chối (bắt buộc):');
  if (!lyDoTuChoi || !lyDoTuChoi.trim()) {
    alert('Lý do từ chối là bắt buộc');
    return;
  }
  
  if (!confirm('Bạn có chắc chắn muốn từ chối yêu cầu này?\n\nHọc viên sẽ nhận được thông báo với lý do từ chối.')) {
    return;
  }
  
  try {
    await rejectRequest(requestId, lyDoTuChoi.trim());
    alert('Từ chối yêu cầu thành công!');
    loadInstructorRequests();
    if (Modal.isOpen()) {
      Modal.close();
    }
  } catch (error) {
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi từ chối yêu cầu'));
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
    const categoriesResponse = await getAllCategoriesAdmin();
    
    if (categoriesResponse.success && categoriesResponse.data) {
      const categories = Array.isArray(categoriesResponse.data) ? categoriesResponse.data : [];
      
      // Sort: active first, then by ID
      const sortedCategories = [...categories].sort((a, b) => {
        const aActive = a.trangThai !== false && a.trangThai !== null;
        const bActive = b.trangThai !== false && b.trangThai !== null;
        if (aActive !== bActive) {
          return aActive ? -1 : 1; // Active first
        }
        return (a.id || a.Id || 0) - (b.id || b.Id || 0);
      });
      
      content.innerHTML = `
        <div style="margin-bottom: 20px; display: flex; gap: 12px;">
          <button class="btn btn-primary" onclick="window.showAddCategoryModal()">
            <i class="fas fa-plus"></i> Thêm danh mục mới
          </button>
        </div>
        ${sortedCategories.length === 0 ? `
          <div class="empty-state">
            <div class="empty-state-icon">📁</div>
            <div class="empty-state-text">Chưa có danh mục nào</div>
            <div class="empty-state-subtext">Hãy thêm danh mục đầu tiên!</div>
          </div>
        ` : `
          <div class="table-container">
            <table class="table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Tên danh mục</th>
                  <th>Mô tả</th>
                  <th>Số khóa học</th>
                  <th>Trạng thái</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                ${sortedCategories.map(category => {
                  const isActive = category.trangThai !== false && category.trangThai !== null;
                  const statusBadge = isActive
                    ? '<span class="role-badge" style="background: #d1fae5; color: #065f46;">🟢 Hoạt động</span>'
                    : '<span class="role-badge" style="background: #fee2e2; color: #991b1b;">🔴 Đã ẩn</span>';
                  
                  const categoryId = category.id || category.Id;
                  const categoryName = category.tenDanhMuc || category.TenDanhMuc || 'N/A';
                  const soKhoaHoc = category.soKhoaHoc !== undefined ? category.soKhoaHoc : (category.SoKhoaHoc !== undefined ? category.SoKhoaHoc : 0);
                  
                  return `
                    <tr style="${!isActive ? 'opacity: 0.7;' : ''}">
                      <td>${categoryId}</td>
                      <td><strong>${categoryName}</strong></td>
                      <td>${category.moTa || category.MoTa || '<span style="color: #94a3b8;">Chưa có mô tả</span>'}</td>
                      <td>
                        <span style="font-weight: 600; color: #3b82f6;">${soKhoaHoc}</span>
                        <span style="color: #94a3b8; font-size: 12px;"> khóa học</span>
                      </td>
                      <td>${statusBadge}</td>
                      <td>
                        <div class="actions" style="display: flex; gap: 8px;">
                          <button class="btn btn-sm btn-secondary" onclick="window.showEditCategoryModal(${categoryId})">
                            <i class="fas fa-edit"></i> Sửa
                          </button>
                          ${isActive ? `
                            <button class="btn btn-sm btn-warning" onclick="window.handleHideCategory(${categoryId}, '${categoryName.replace(/'/g, "\\'")}')">
                              <i class="fas fa-eye-slash"></i> Ẩn
                            </button>
                          ` : `
                            <button class="btn btn-sm btn-success" onclick="window.handleRestoreCategory(${categoryId}, '${categoryName.replace(/'/g, "\\'")}')">
                              <i class="fas fa-undo"></i> Khôi phục
                            </button>
                          `}
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
