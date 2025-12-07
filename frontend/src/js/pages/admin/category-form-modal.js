/**
 * Category Form Modal
 * Modal form for creating/editing categories (ADMIN)
 */

import { Modal } from '../../components/admin/modal.js';
import { createCategory, updateCategory, getCategoryById } from '../../api/categoryApi.js';

let currentModal = null;
let currentCategoryId = null;
let onSuccessCallback = null;

/**
 * Show category form modal for create
 */
export function showAddCategoryModal(onSuccess) {
  currentCategoryId = null;
  onSuccessCallback = onSuccess;
  renderModal();
}

/**
 * Show category form modal for edit
 */
export async function showEditCategoryModal(categoryId, onSuccess) {
  currentCategoryId = categoryId;
  onSuccessCallback = onSuccess;
  await renderModal();
}

/**
 * Render modal with form
 */
async function renderModal() {
  const isEdit = currentCategoryId !== null;
  let categoryData = null;

  // If editing, load category data
  if (isEdit) {
    try {
      const response = await getCategoryById(currentCategoryId);
      if (response.success && response.data) {
        categoryData = response.data;
      } else {
        alert('Không thể tải thông tin danh mục: ' + (response.message || 'Lỗi không xác định'));
        return;
      }
    } catch (error) {
      alert('Không thể tải thông tin danh mục: ' + error.message);
      return;
    }
  }

  const formContent = `
    <form id="category-form" class="user-form">
      <div class="form-group">
        <label for="tenDanhMuc">Tên danh mục <span class="required">*</span></label>
        <input
          type="text"
          id="tenDanhMuc"
          name="tenDanhMuc"
          class="form-control"
          required
          maxlength="100"
          value="${categoryData ? (categoryData.tenDanhMuc || categoryData.TenDanhMuc || '') : ''}"
          placeholder="Nhập tên danh mục (tối đa 100 ký tự)"
        />
      </div>

      <div class="form-group">
        <label for="moTa">Mô tả</label>
        <textarea
          id="moTa"
          name="moTa"
          class="form-control"
          rows="3"
          maxlength="500"
          placeholder="Nhập mô tả (tối đa 500 ký tự)"
        >${categoryData ? (categoryData.moTa || categoryData.MoTa || '') : ''}</textarea>
      </div>
    </form>
  `;

  const footer = `
    <button type="button" class="btn btn-secondary" id="cancel-btn">Hủy</button>
    <button type="button" class="btn btn-primary" id="save-btn">${isEdit ? 'Cập nhật' : 'Tạo danh mục'}</button>
  `;

  if (currentModal) {
    currentModal.destroy();
  }

  currentModal = new Modal({
    id: 'category-form-modal',
    title: isEdit ? 'Sửa danh mục' : 'Thêm danh mục mới',
    content: formContent,
    footer: footer,
    size: 'medium',
    closeOnOverlay: true
  });

  const modalRoot = document.getElementById('modal-root');
  if (modalRoot) {
    modalRoot.innerHTML = currentModal.render();
    currentModal.attachEventListeners();
    currentModal.open();
    attachFormListeners();
  }
}

/**
 * Attach form event listeners
 */
function attachFormListeners() {
  const form = document.getElementById('category-form');
  const saveBtn = document.getElementById('save-btn');
  const cancelBtn = document.getElementById('cancel-btn');

  if (saveBtn) {
    saveBtn.addEventListener('click', handleSubmit);
  }

  if (cancelBtn) {
    cancelBtn.addEventListener('click', () => {
      if (currentModal) currentModal.close();
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
  const form = document.getElementById('category-form');
  if (!form) return;

  if (!form.checkValidity()) {
    form.reportValidity();
    return;
  }

  const formData = new FormData(form);
  const payload = {
    tenDanhMuc: (formData.get('tenDanhMuc') || '').trim(),
    moTa: (formData.get('moTa') || '').trim() || null
  };

  try {
    const saveBtn = document.getElementById('save-btn');
    if (saveBtn) {
      saveBtn.disabled = true;
      saveBtn.textContent = currentCategoryId ? 'Đang cập nhật...' : 'Đang tạo...';
    }

    let response;
    if (currentCategoryId) {
      // Update category
      response = await updateCategory(currentCategoryId, payload);
    } else {
      // Create category
      response = await createCategory(payload);
    }

    if (response.success || response.data) {
      alert(currentCategoryId ? 'Cập nhật danh mục thành công!' : 'Tạo danh mục thành công!');
      if (currentModal) currentModal.close();
      if (onSuccessCallback) onSuccessCallback();
    } else {
      throw new Error(response.message || (currentCategoryId ? 'Không thể cập nhật danh mục' : 'Không thể tạo danh mục'));
    }
  } catch (error) {
    console.error('Error saving category:', error);
    alert('Lỗi: ' + (error.message || (currentCategoryId ? 'Có lỗi xảy ra khi cập nhật danh mục' : 'Có lỗi xảy ra khi tạo danh mục')));
  } finally {
    const saveBtn = document.getElementById('save-btn');
    if (saveBtn) {
      saveBtn.disabled = false;
      saveBtn.textContent = currentCategoryId ? 'Cập nhật' : 'Tạo danh mục';
    }
  }
}

