/**
 * User Form Modal
 * Modal form for creating/editing users
 */

import { Modal } from '../../components/admin/modal.js';
import { createUser, updateUser, getUserById } from '../../api/nguoiDungApi.js';
import { getAllRoles } from '../../api/vaiTroApi.js';
import { API_BASE_URL } from '../../config.js';
import { getAccessToken } from '../../utils/token.js';

let currentModal = null;
let currentUserId = null;
let onSuccessCallback = null;

/**
 * Show user form modal for create
 */
export async function showUserFormModal(onSuccess) {
  currentUserId = null;
  onSuccessCallback = onSuccess;
  await renderModal();
}

/**
 * Show user form modal for edit
 */
export async function showEditUserFormModal(userId, onSuccess) {
  currentUserId = userId;
  onSuccessCallback = onSuccess;
  await renderModal();
}

/**
 * Render modal with form
 */
async function renderModal() {
  const isEdit = currentUserId !== null;
  let userData = null;
  let userRoles = [];

  // Load roles list
  let allRoles = [];
  
  // Fallback roles based on database structure (ADMIN, GIANGVIEN, HOCVIEN, KIEMDUYET)
  const fallbackRoles = [
    { id: 2, tenVaiTro: 'GIANGVIEN', moTa: 'Giảng viên' },
    { id: 3, tenVaiTro: 'HOCVIEN', moTa: 'Học viên' },
    { id: 4, tenVaiTro: 'KIEMDUYET', moTa: 'Kiểm duyệt viên khóa học' }
  ];
  
  try {
    console.log('Loading roles from API...');
    const rolesResponse = await getAllRoles();
    console.log('Roles response:', rolesResponse);
    
    if (rolesResponse && rolesResponse.success && rolesResponse.data) {
      allRoles = Array.isArray(rolesResponse.data) ? rolesResponse.data : [];
      console.log('Loaded roles from API:', allRoles);
    } else {
      console.warn('Roles response format unexpected:', rolesResponse);
      // Use fallback if API response is unexpected
      if (allRoles.length === 0) {
        console.warn('Using fallback roles');
        allRoles = fallbackRoles;
      }
    }
  } catch (error) {
    console.error('Error loading roles from API:', error);
    console.error('Error details:', {
      message: error.message,
      stack: error.stack
    });
    // Use fallback roles if API fails
    console.warn('Using fallback roles due to API error');
    allRoles = fallbackRoles;
  }

  // Filter out ADMIN role
  let availableRoles = allRoles.filter(r => {
    if (!r) return false;
    const roleName = r.tenVaiTro || r.TenVaiTro || '';
    const isAdmin = roleName && roleName.toUpperCase() === 'ADMIN';
    return !isAdmin;
  });
  
  console.log('Available roles (excluding ADMIN):', availableRoles);
  
  // If still no roles, use fallback
  if (availableRoles.length === 0) {
    console.warn('No roles available, using fallback');
    availableRoles = fallbackRoles.filter(r => {
      if (!r) return false;
      const roleName = r.tenVaiTro || r.TenVaiTro || '';
      return roleName && roleName.toUpperCase() !== 'ADMIN';
    });
    console.log('Using fallback roles:', availableRoles);
  }

  // If editing, load user data
  if (isEdit) {
    try {
      // Load user detail with roles
      const token = getAccessToken();
      
      const detailResponse = await fetch(`${API_BASE_URL}v1/nguoidungs/${currentUserId}/detail`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (detailResponse.ok) {
        const detailData = await detailResponse.json();
        if (detailData.success && detailData.data) {
          userData = detailData.data;
          userRoles = detailData.data.vaiTros || detailData.data.VaiTros || [];
        }
      } else {
        // Fallback to regular getUserById
        const response = await getUserById(currentUserId);
        if (response.success && response.data) {
          userData = response.data;
        }
      }
    } catch (error) {
      alert('Không thể tải thông tin người dùng: ' + error.message);
      return;
    }
  }

  const formContent = `
    <form id="user-form" class="user-form">
      <div class="form-group">
        <label for="hoTen">Họ tên <span class="required">*</span></label>
        <input 
          type="text" 
          id="hoTen" 
          name="hoTen" 
          class="form-control" 
          value="${userData?.hoTen || ''}" 
          required
          placeholder="Nhập họ tên"
        />
      </div>

      <div class="form-group">
        <label for="email">Email <span class="required">*</span></label>
        <input 
          type="email" 
          id="email" 
          name="email" 
          class="form-control" 
          value="${userData?.email || ''}" 
          required
          placeholder="Nhập email"
          ${isEdit ? 'readonly' : ''}
        />
        ${isEdit ? '<small class="form-text">Email không thể thay đổi</small>' : ''}
      </div>

      <div class="form-group">
        <label for="matKhau">Mật khẩu <span class="required">*</span></label>
        <input 
          type="password" 
          id="matKhau" 
          name="matKhau" 
          class="form-control" 
          ${isEdit ? '' : 'required'}
          placeholder="${isEdit ? 'Để trống nếu không đổi mật khẩu' : 'Nhập mật khẩu'}"
          minlength="6"
        />
        ${isEdit ? '<small class="form-text">Để trống nếu không muốn thay đổi mật khẩu</small>' : ''}
      </div>

      <div class="form-group">
        <label for="soDienThoai">Số điện thoại</label>
        <input 
          type="tel" 
          id="soDienThoai" 
          name="soDienThoai" 
          class="form-control" 
          value="${userData?.soDienThoai || ''}" 
          placeholder="Nhập số điện thoại"
        />
      </div>

      <div class="form-group">
        <label for="anhDaiDien">URL Ảnh đại diện</label>
        <input 
          type="url" 
          id="anhDaiDien" 
          name="anhDaiDien" 
          class="form-control" 
          value="${userData?.anhDaiDien || ''}" 
          placeholder="Nhập URL ảnh đại diện"
        />
      </div>

      <div class="form-group">
        <label for="tieuSu">Tiểu sử</label>
        <textarea 
          id="tieuSu" 
          name="tieuSu" 
          class="form-control" 
          rows="4"
          placeholder="Nhập tiểu sử"
        >${userData?.tieuSu || ''}</textarea>
      </div>

      <div class="form-group">
        <label for="roles">Vai trò</label>
        ${availableRoles.length > 0 ? `
          <div style="position: relative;">
            <select 
              id="roles" 
              name="roles" 
              class="form-control" 
              multiple
              style="display: none;"
            >
              ${availableRoles.map(role => {
                const roleName = role.tenVaiTro || role.TenVaiTro || '';
                const isSelected = userRoles.some(ur => {
                  const userRoleName = typeof ur === 'string' ? ur : (ur.tenVaiTro || ur.TenVaiTro || '');
                  return userRoleName && userRoleName.toUpperCase() === roleName.toUpperCase();
                });
                const displayName = role.moTa || role.MoTa 
                  ? `${roleName} - ${role.moTa || role.MoTa}` 
                  : roleName;
                return `<option value="${roleName}" ${isSelected ? 'selected' : ''}>${displayName}</option>`;
              }).join('')}
            </select>
            <div id="roles-dropdown" class="roles-dropdown" style="position: relative;">
              <button 
                type="button" 
                id="roles-dropdown-toggle" 
                class="form-control roles-dropdown-toggle"
                style="text-align: left; background: white; border: 1px solid #ddd; padding: 8px 12px; cursor: pointer; border-radius: 4px;"
              >
                <span id="roles-selected-text">Chọn vai trò...</span>
                <span style="float: right;">▼</span>
              </button>
              <div id="roles-dropdown-menu" class="roles-dropdown-menu" style="display: none; position: absolute; top: 100%; left: 0; right: 0; background: white; border: 1px solid #ddd; border-radius: 4px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); z-index: 1000; max-height: 200px; overflow-y: auto; margin-top: 4px;">
                ${availableRoles.map(role => {
                  const roleName = role.tenVaiTro || role.TenVaiTro || '';
                  const isSelected = userRoles.some(ur => {
                    const userRoleName = typeof ur === 'string' ? ur : (ur.tenVaiTro || ur.TenVaiTro || '');
                    return userRoleName && userRoleName.toUpperCase() === roleName.toUpperCase();
                  });
                  const displayName = role.moTa || role.MoTa 
                    ? `${roleName} - ${role.moTa || role.MoTa}` 
                    : roleName;
                  return `
                    <label style="display: block; padding: 8px 12px; cursor: pointer; margin: 0; border-bottom: 1px solid #f0f0f0;">
                      <input 
                        type="checkbox" 
                        class="role-checkbox-item" 
                        data-role="${roleName}"
                        ${isSelected ? 'checked' : ''}
                        style="margin-right: 8px;"
                      />
                      <span>${displayName}</span>
                    </label>
                  `;
                }).join('')}
              </div>
            </div>
          </div>
          <small class="form-text" style="display: block; margin-top: 8px; color: #666;">
            Lưu ý: Khi chọn vai trò Giảng viên, hệ thống sẽ tự động thêm vai trò Học viên. Vai trò Admin không thể được gán từ đây.
          </small>
        ` : `
          <select 
            id="roles" 
            name="roles" 
            class="form-control" 
            disabled
          >
            <option disabled>Đang tải danh sách vai trò...</option>
          </select>
          <small class="form-text" style="display: block; margin-top: 8px; color: #dc3545;">
            ⚠️ Không thể tải danh sách vai trò. Vui lòng kiểm tra kết nối và thử lại.
          </small>
        `}
      </div>

      ${isEdit ? `
        <div class="form-group">
          <label>
            <input 
              type="checkbox" 
              id="trangThai" 
              name="trangThai" 
              ${userData?.trangThai ? 'checked' : ''}
            />
            Trạng thái hoạt động
          </label>
        </div>
      ` : ''}
    </form>
  `;

  const footer = `
    <button type="button" class="btn btn-secondary" id="cancel-btn">Hủy</button>
    <button type="button" class="btn btn-primary" id="save-btn">${isEdit ? 'Cập nhật' : 'Tạo mới'}</button>
  `;

  // Remove existing modal if any
  if (currentModal) {
    currentModal.destroy();
  }

  currentModal = new Modal({
    id: 'user-form-modal',
    title: isEdit ? 'Sửa người dùng' : 'Thêm người dùng mới',
    content: formContent,
    footer: footer,
    size: 'medium',
    closeOnOverlay: true
  });

  // Render modal
  const modalRoot = document.getElementById('modal-root');
  if (modalRoot) {
    modalRoot.innerHTML = currentModal.render();
    currentModal.attachEventListeners();
    currentModal.open();

    // Attach form event listeners
    attachFormListeners();
    
    // Setup roles dropdown
    setupRolesDropdown();
  }
}

/**
 * Setup roles dropdown functionality
 */
function setupRolesDropdown() {
  const toggleBtn = document.getElementById('roles-dropdown-toggle');
  const dropdownMenu = document.getElementById('roles-dropdown-menu');
  const selectedText = document.getElementById('roles-selected-text');
  const roleCheckboxes = document.querySelectorAll('.role-checkbox-item');
  const hiddenSelect = document.getElementById('roles');
  
  if (!toggleBtn || !dropdownMenu) return;
  
  // Toggle dropdown
  toggleBtn.addEventListener('click', (e) => {
    e.preventDefault();
    e.stopPropagation();
    const isOpen = dropdownMenu.style.display !== 'none';
    dropdownMenu.style.display = isOpen ? 'none' : 'block';
  });
  
  // Close dropdown when clicking outside
  document.addEventListener('click', (e) => {
    if (!dropdownMenu.contains(e.target) && !toggleBtn.contains(e.target)) {
      dropdownMenu.style.display = 'none';
    }
  });
  
  // Update selected text and hidden select when checkbox changes
  function updateSelectedRoles() {
    const selected = Array.from(roleCheckboxes)
      .filter(cb => cb.checked)
      .map(cb => {
        const roleName = cb.getAttribute('data-role');
        const label = cb.parentElement.querySelector('span').textContent;
        return { roleName, label };
      });
    
    // Update button text
    if (selected.length === 0) {
      selectedText.textContent = 'Chọn vai trò...';
    } else if (selected.length === 1) {
      selectedText.textContent = selected[0].label;
    } else {
      selectedText.textContent = `Đã chọn ${selected.length} vai trò`;
    }
    
    // Update hidden select
    if (hiddenSelect) {
      // Clear all selections first
      Array.from(hiddenSelect.options).forEach(opt => opt.selected = false);
      // Select based on checkboxes
      selected.forEach(s => {
        const option = Array.from(hiddenSelect.options).find(opt => opt.value === s.roleName);
        if (option) option.selected = true;
      });
    }
  }
  
  // Attach change listeners to checkboxes
  roleCheckboxes.forEach(cb => {
    cb.addEventListener('change', updateSelectedRoles);
  });
  
  // Initial update
  updateSelectedRoles();
  
  // Prevent dropdown from closing when clicking inside
  dropdownMenu.addEventListener('click', (e) => {
    e.stopPropagation();
  });
}

/**
 * Attach form event listeners
 */
function attachFormListeners() {
  const form = document.getElementById('user-form');
  const saveBtn = document.getElementById('save-btn');
  const cancelBtn = document.getElementById('cancel-btn');

  if (saveBtn) {
    saveBtn.addEventListener('click', handleSubmit);
  }

  if (cancelBtn) {
    cancelBtn.addEventListener('click', () => {
      if (currentModal) {
        currentModal.close();
      }
    });
  }

  if (form) {
    form.addEventListener('submit', (e) => {
      e.preventDefault();
      handleSubmit();
    });
  }
}

/**
 * Handle form submission
 */
async function handleSubmit() {
  const form = document.getElementById('user-form');
  if (!form) return;

  // Validate form
  if (!form.checkValidity()) {
    form.reportValidity();
    return;
  }

  const formData = new FormData(form);
  const userData = {
    hoTen: formData.get('hoTen'),
    email: formData.get('email'),
    soDienThoai: formData.get('soDienThoai') || null,
    anhDaiDien: formData.get('anhDaiDien') || null,
    tieuSu: formData.get('tieuSu') || null
  };

  // Add password only if provided (for edit) or required (for create)
  const matKhau = formData.get('matKhau');
  if (matKhau && matKhau.trim() !== '') {
    userData.matKhau = matKhau;
  } else if (!currentUserId) {
    // Password is required for create
    alert('Vui lòng nhập mật khẩu');
    return;
  }

  // Collect selected roles from checkboxes (custom dropdown)
  const roleCheckboxes = document.querySelectorAll('.role-checkbox-item:checked');
  if (roleCheckboxes.length > 0) {
    const selectedRoles = Array.from(roleCheckboxes).map(cb => cb.getAttribute('data-role'));
    userData.roles = selectedRoles;
  } else {
    // Fallback: try to get from hidden select
    const rolesSelect = document.getElementById('roles');
    if (rolesSelect && rolesSelect.selectedOptions) {
      const selectedRoles = Array.from(rolesSelect.selectedOptions).map(option => option.value);
      userData.roles = selectedRoles.length > 0 ? selectedRoles : null;
    } else {
      userData.roles = null;
    }
  }

  // Add trangThai for edit
  if (currentUserId) {
    const trangThaiCheckbox = document.getElementById('trangThai');
    userData.trangThai = trangThaiCheckbox ? trangThaiCheckbox.checked : true;
  }

  try {
    // Disable save button
    const saveBtn = document.getElementById('save-btn');
    if (saveBtn) {
      saveBtn.disabled = true;
      saveBtn.textContent = 'Đang xử lý...';
    }

    let response;
    if (currentUserId) {
      // Update user
      // If password is not provided, don't send it at all (let backend keep current password)
      if (!userData.matKhau || userData.matKhau.trim() === '') {
        // Remove password field if empty - backend will keep current password
        delete userData.matKhau;
      }
      response = await updateUser(currentUserId, userData);
    } else {
      // Create user - password is required
      response = await createUser(userData);
    }

    if (response.success || response.data) {
      alert(currentUserId ? 'Cập nhật người dùng thành công!' : 'Tạo người dùng thành công!');
      
      // Close modal
      if (currentModal) {
        currentModal.close();
      }

      // Call success callback
      if (onSuccessCallback) {
        onSuccessCallback();
      }
    } else {
      throw new Error(response.message || 'Có lỗi xảy ra');
    }
  } catch (error) {
    console.error('Error saving user:', error);
    alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi lưu người dùng'));
    
    // Re-enable save button
    const saveBtn = document.getElementById('save-btn');
    if (saveBtn) {
      saveBtn.disabled = false;
      saveBtn.textContent = currentUserId ? 'Cập nhật' : 'Tạo mới';
    }
  }
}

