/**
 * User Form Modal
 * Modal form for creating/editing users
 */

import { Modal } from '../../components/admin/modal.js';
import { createUser, updateUser, getUserById } from '../../api/nguoiDungApi.js';

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

  // If editing, load user data
  if (isEdit) {
    try {
      const response = await getUserById(currentUserId);
      if (response.success && response.data) {
        userData = response.data;
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
  }
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
      // If password is not provided, we'll send empty string and let backend handle it
      // Backend should check if password is empty and skip password update
      if (!userData.matKhau) {
        // For update without password change, send empty string
        // Backend should handle this by not updating password if empty
        userData.matKhau = '';
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

