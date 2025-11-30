/**
 * Course Form Modal
 * Full-featured form for creating/editing courses with chapters and lessons
 */

import { Modal } from '../components/admin/modal.js';
import { getAllCategories } from '../api/categoryApi.js';
import { apiFetch } from '../api/baseApi.js';
import { API_ENDPOINTS, API_BASE_URL } from '../config.js';
import { getAccessToken } from '../utils/token.js';
import { getCourseForEdit, updateCourseWithCurriculum } from '../api/courseApi.js';

let currentModal = null;
let categories = [];
let isEditMode = false;
let currentCourseId = null;
window.formData = {
  idDanhMuc: '',
  tenKhoaHoc: '',
  moTaNgan: '',
  moTaChiTiet: '',
  giaBan: '',
  mucDo: 'Cơ bản',
  yeuCauTruoc: '',
  hocDuoc: '',
  chuongs: []
};

/**
 * Show course form modal
 */
export async function showCourseFormModal(courseId = null) {
  try {
    isEditMode = courseId !== null;
    currentCourseId = courseId;

    // Load categories
    const categoriesResponse = await getAllCategories();
    if (categoriesResponse.success) {
      categories = Array.isArray(categoriesResponse.data) 
        ? categoriesResponse.data 
        : (categoriesResponse.data?.items || []);
    }

    if (categories.length === 0) {
      alert('Không có danh mục nào. Vui lòng tạo danh mục trước.');
      return;
    }

    // Load course data if editing
    if (isEditMode) {
      try {
        console.log('Loading course data for edit, courseId:', courseId);
        const courseResponse = await getCourseForEdit(courseId);
        console.log('Course response:', courseResponse);
        
        if (courseResponse && courseResponse.success && courseResponse.data) {
          const courseData = courseResponse.data;
          window.formData = {
            idDanhMuc: courseData.idDanhMuc,
            tenKhoaHoc: courseData.tenKhoaHoc,
            moTaNgan: courseData.moTaNgan || '',
            moTaChiTiet: courseData.moTaChiTiet || '',
            giaBan: courseData.giaBan || '',
            mucDo: courseData.mucDo || 'Cơ bản',
            yeuCauTruoc: courseData.yeuCauTruoc || '',
            hocDuoc: courseData.hocDuoc || '',
            chuongs: courseData.chuongs || []
          };
          console.log('Form data loaded:', window.formData);
        } else {
          throw new Error('Không nhận được dữ liệu khóa học từ server');
        }
      } catch (error) {
        console.error('Error loading course data:', error);
        alert('Lỗi khi tải dữ liệu khóa học: ' + error.message);
        window.closeCourseModal();
        return;
      }
    } else {
      // Reset form data for new course
      window.formData = {
        idDanhMuc: '',
        tenKhoaHoc: '',
        moTaNgan: '',
        moTaChiTiet: '',
        giaBan: '',
        mucDo: 'Cơ bản',
        yeuCauTruoc: '',
        hocDuoc: '',
        chuongs: []
      };
    }

    const formHtml = generateFormHtml();
    
    currentModal = new Modal({
      id: 'course-form-modal',
      title: isEditMode ? 'Sửa khóa học' : 'Thêm khóa học mới',
      content: formHtml,
      size: 'large',
      footer: `
        <button type="button" class="btn btn-secondary" onclick="window.closeCourseModal()">Hủy</button>
        <button type="button" class="btn btn-primary" onclick="window.submitCourseForm()">${isEditMode ? 'Cập nhật khóa học' : 'Tạo khóa học'}</button>
      `,
      onClose: () => {
        currentModal = null;
        isEditMode = false;
        currentCourseId = null;
      }
    });

    const modalRoot = document.getElementById('modal-root');
    if (modalRoot) {
      modalRoot.innerHTML = currentModal.render();
      currentModal.attachEventListeners();
      currentModal.open();
      
      setupFormEventListeners();
      
      // Populate form if editing
      if (isEditMode) {
        populateForm();
      }
    }
  } catch (error) {
    console.error('Error showing course form:', error);
    alert('Lỗi khi tải form: ' + error.message);
  }
}

/**
 * Generate form HTML
 */
function generateFormHtml() {
  const categoryOptions = categories.map(cat => 
    `<option value="${cat.id}">${cat.tenDanhMuc || 'N/A'}</option>`
  ).join('');

  const categoryDisabled = isEditMode ? 'disabled' : '';
  const categoryReadonly = isEditMode ? 'readonly' : '';

  return `
    <form id="course-form" class="course-form">
      <!-- Step 1: Basic Info -->
      <div class="form-section">
        <h3 class="form-section-title">Thông tin cơ bản</h3>
        
        <div class="form-group">
          <label class="form-label" for="idDanhMuc">
            Danh mục <span class="required">*</span>
            ${isEditMode ? '<span style="color: #999; font-size: 0.9em;">(Không thể thay đổi khi chỉnh sửa)</span>' : ''}
          </label>
          <select class="form-select" id="idDanhMuc" name="idDanhMuc" required ${categoryDisabled}>
            <option value="">Chọn danh mục</option>
            ${categoryOptions}
          </select>
        </div>

        <div class="form-group">
          <label class="form-label" for="tenKhoaHoc">
            Tên khóa học <span class="required">*</span>
          </label>
          <input 
            type="text" 
            class="form-input" 
            id="tenKhoaHoc" 
            name="tenKhoaHoc"
            placeholder="Nhập tên khóa học"
            required
          />
        </div>

        <div class="form-group">
          <label class="form-label" for="moTaNgan">
            Mô tả ngắn
          </label>
          <textarea 
            class="form-input" 
            id="moTaNgan" 
            name="moTaNgan"
            rows="3"
            placeholder="Mô tả ngắn về khóa học"
          ></textarea>
        </div>

        <div class="form-group">
          <label class="form-label" for="moTaChiTiet">
            Mô tả chi tiết
          </label>
          <textarea 
            class="form-input" 
            id="moTaChiTiet" 
            name="moTaChiTiet"
            rows="5"
            placeholder="Mô tả chi tiết về khóa học"
          ></textarea>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label" for="giaBan">
              Giá bán (VND)
            </label>
            <input 
              type="number" 
              class="form-input" 
              id="giaBan" 
              name="giaBan"
              placeholder="0"
              min="0"
            />
          </div>

          <div class="form-group">
            <label class="form-label" for="mucDo">
              Mức độ
            </label>
            <select class="form-select" id="mucDo" name="mucDo">
              <option value="Cơ bản">Cơ bản</option>
              <option value="Trung bình">Trung bình</option>
              <option value="Nâng cao">Nâng cao</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label" for="yeuCauTruoc">
            Yêu cầu trước
          </label>
          <textarea 
            class="form-input" 
            id="yeuCauTruoc" 
            name="yeuCauTruoc"
            rows="3"
            placeholder="Những yêu cầu trước khi học khóa học này"
          ></textarea>
        </div>

        <div class="form-group">
          <label class="form-label" for="hocDuoc">
            Bạn sẽ học được gì
          </label>
          <textarea 
            class="form-input" 
            id="hocDuoc" 
            name="hocDuoc"
            rows="3"
            placeholder="Những gì học viên sẽ học được sau khóa học"
          ></textarea>
        </div>
      </div>

      <!-- Step 2: Chapters and Lessons -->
      <div class="form-section">
        <div class="form-section-header">
          <h3 class="form-section-title">Chương và bài giảng</h3>
          <button type="button" class="btn btn-sm btn-primary" onclick="window.addChapter(); return false;">
            <i class="fas fa-plus"></i> Thêm chương
          </button>
        </div>

        <div id="chapters-container">
          <!-- Chapters will be added here -->
        </div>
      </div>
    </form>
  `;
}

/**
 * Setup form event listeners
 */
function setupFormEventListeners() {
  // Ngăn form submit khi nhấn Enter hoặc các event khác
  const form = document.getElementById('course-form');
  if (form) {
    // Ngăn form submit tự động - QUAN TRỌNG để không refresh trang
    form.addEventListener('submit', function(e) {
      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();
      console.log('Form submit prevented');
      return false;
    }, { capture: true });
    
    // Ngăn tất cả button/file input trong form trigger submit (nhưng không chặn onclick handlers)
    form.addEventListener('click', function(e) {
      // Chỉ stop propagation cho file input, không chặn button với onclick
      if (e.target.type === 'file') {
        e.stopPropagation();
      }
      // Không chặn button với onclick attribute
    }, { capture: true });
    
    // Lưu dữ liệu form khi input thay đổi
    form.addEventListener('input', (e) => {
      if (e.target.name) {
        window.formData[e.target.name] = e.target.value;
      }
    });
    
    // Đảm bảo modal không bị đóng khi có interaction trong form
    form.addEventListener('focusin', function() {
      if (currentModal) {
        currentModal.isOpen = true;
      }
    });
  }

  // Add first chapter by default (only for new course)
  if (!isEditMode) {
    window.addChapter();
  } else if (window.formData.chuongs && window.formData.chuongs.length > 0) {
    // Load existing chapters and lessons
    // Store original length to prevent infinite loop
    const originalChaptersCount = window.formData.chuongs.length;
    
    window.formData.chuongs.forEach((chuong, chapterIndex) => {
      // Add chapter - this will calculate index based on DOM
      window.addChapter();
      
      // Wait for chapter to be added to DOM
      setTimeout(() => {
        try {
          const chaptersContainer = document.getElementById('chapters-container');
          if (!chaptersContainer) {
            console.error('chapters-container not found');
            return;
          }
          
          const allChapters = chaptersContainer.querySelectorAll('.chapter-card');
          const addedChapterCard = allChapters[allChapters.length - 1];
          
          if (!addedChapterCard) {
            console.error('Could not find added chapter card for index:', chapterIndex);
            return;
          }
          
          // Update the data-chapter-index to match our expected index
          addedChapterCard.setAttribute('data-chapter-index', chapterIndex);
          const header = addedChapterCard.querySelector('h4');
          if (header) {
            header.textContent = `Chương ${chapterIndex + 1}`;
          }
          
          // Update all data attributes in this chapter to use correct index
          addedChapterCard.querySelectorAll('[data-chapter-index]').forEach(el => {
            el.setAttribute('data-chapter-index', chapterIndex);
          });
          
          // Update remove button (only chapter header button)
          const chapterHeader = addedChapterCard.querySelector('.chapter-header');
          if (chapterHeader) {
            const removeBtn = chapterHeader.querySelector('.btn-danger');
            if (removeBtn) {
              removeBtn.setAttribute('onclick', `window.removeChapter(${chapterIndex}); return false;`);
            }
          }
          
          // Update add lesson button
          const addLessonBtn = addedChapterCard.querySelector('button[onclick*="addLesson"]');
          if (addLessonBtn) {
            addLessonBtn.setAttribute('onclick', `window.addLesson(${chapterIndex}); return false;`);
          }
          
          // Populate chapter data
          const chapterNameInput = addedChapterCard.querySelector('.chapter-name');
          const chapterDescInput = addedChapterCard.querySelector('.chapter-description');
          if (chapterNameInput) {
            chapterNameInput.value = chuong.tenChuong || '';
            chapterNameInput.setAttribute('data-chapter-index', chapterIndex);
          }
          if (chapterDescInput) {
            chapterDescInput.value = chuong.moTa || '';
            chapterDescInput.setAttribute('data-chapter-index', chapterIndex);
          }
          
          // Populate lessons
          if (chuong.baiGiangs && chuong.baiGiangs.length > 0) {
            // Clear the first lesson that was auto-added by addChapter
            const lessonsContainer = addedChapterCard.querySelector('.lessons-container');
            if (lessonsContainer) {
              lessonsContainer.innerHTML = ''; // Clear all auto-added lessons
            }
            
            // Clear formData lessons for this chapter
            if (window.formData.chuongs[chapterIndex]) {
              window.formData.chuongs[chapterIndex].baiGiangs = [];
            }
            
            // Add all lessons from data
            chuong.baiGiangs.forEach((baiGiang, lessonIndex) => {
              window.addLesson(chapterIndex);
            });
            
            // Wait for all lessons to be added, then populate data
            setTimeout(() => {
              chuong.baiGiangs.forEach((baiGiang, lessonIndex) => {
                const lessonCard = addedChapterCard.querySelector(`.lesson-card[data-lesson-index="${lessonIndex}"]`);
                if (!lessonCard) {
                  console.warn(`Lesson card not found for chapter ${chapterIndex}, lesson ${lessonIndex}`);
                  return;
                }
                
                // Update lesson card data attributes
                lessonCard.setAttribute('data-chapter-index', chapterIndex);
                
                const lessonTitleInput = lessonCard.querySelector(`.lesson-title`);
                const lessonDescInput = lessonCard.querySelector(`.lesson-description`);
                const lessonFreeCheckbox = lessonCard.querySelector(`.lesson-free-preview`);
                const statusDiv = lessonCard.querySelector(`.video-upload-status`);
                
                if (lessonTitleInput) {
                  lessonTitleInput.value = baiGiang.tieuDe || '';
                  lessonTitleInput.setAttribute('data-chapter-index', chapterIndex);
                }
                if (lessonDescInput) {
                  lessonDescInput.value = baiGiang.moTa || '';
                  lessonDescInput.setAttribute('data-chapter-index', chapterIndex);
                }
                if (lessonFreeCheckbox) {
                  lessonFreeCheckbox.checked = baiGiang.mienPhiXem || false;
                  lessonFreeCheckbox.setAttribute('data-chapter-index', chapterIndex);
                }
                
                // Update all lesson inputs data attributes
                lessonCard.querySelectorAll('[data-chapter-index]').forEach(el => {
                  el.setAttribute('data-chapter-index', chapterIndex);
                });
                
                // Update remove lesson button
                const lessonHeader = lessonCard.querySelector('.lesson-header');
                if (lessonHeader) {
                  const removeLessonBtn = lessonHeader.querySelector('.btn-danger');
                  if (removeLessonBtn) {
                    removeLessonBtn.setAttribute('onclick', `window.removeLesson(${chapterIndex}, ${lessonIndex}); return false;`);
                  }
                }
                
                // Update video status if video exists
                if (baiGiang.duongDanVideo && statusDiv) {
                  const fileName = baiGiang.duongDanVideo.split('/').pop() || baiGiang.duongDanVideo;
                  statusDiv.innerHTML = `<span class="upload-status-text" style="color: green;"><i class="fas fa-check"></i> Video hiện tại: ${fileName}</span>`;
                  
                  // Store existing video URL in formData
                  if (!window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex]) {
                    window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex] = {};
                  }
                  window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex].duongDanVideo = baiGiang.duongDanVideo;
                } else if (statusDiv && !baiGiang.duongDanVideo) {
                  // If no video, keep the default message
                  statusDiv.innerHTML = '<span class="upload-status-text">Chưa chọn video (bắt buộc)</span>';
                }
              });
            }, 200);
          } else {
            // No lessons in data, but addChapter already added one, so clear it
            const lessonsContainer = addedChapterCard.querySelector('.lessons-container');
            if (lessonsContainer) {
              lessonsContainer.innerHTML = '';
            }
            if (window.formData.chuongs[chapterIndex]) {
              window.formData.chuongs[chapterIndex].baiGiangs = [];
            }
          }
        } catch (error) {
          console.error('Error loading chapter:', chapterIndex, error);
        }
      }, chapterIndex * 200); // Stagger timeouts to prevent race conditions
    });
  } else {
    window.addChapter();
  }
}

/**
 * Populate form with existing data
 */
function populateForm() {
  // Populate basic fields
  const idDanhMucSelect = document.getElementById('idDanhMuc');
  const tenKhoaHocInput = document.getElementById('tenKhoaHoc');
  const moTaNganInput = document.getElementById('moTaNgan');
  const moTaChiTietInput = document.getElementById('moTaChiTiet');
  const giaBanInput = document.getElementById('giaBan');
  const mucDoSelect = document.getElementById('mucDo');
  const yeuCauTruocInput = document.getElementById('yeuCauTruoc');
  const hocDuocInput = document.getElementById('hocDuoc');

  if (idDanhMucSelect) idDanhMucSelect.value = window.formData.idDanhMuc;
  if (tenKhoaHocInput) tenKhoaHocInput.value = window.formData.tenKhoaHoc;
  if (moTaNganInput) moTaNganInput.value = window.formData.moTaNgan || '';
  if (moTaChiTietInput) moTaChiTietInput.value = window.formData.moTaChiTiet || '';
  if (giaBanInput) giaBanInput.value = window.formData.giaBan || '';
  if (mucDoSelect) mucDoSelect.value = window.formData.mucDo || 'Cơ bản';
  if (yeuCauTruocInput) yeuCauTruocInput.value = window.formData.yeuCauTruoc || '';
  if (hocDuocInput) hocDuocInput.value = window.formData.hocDuoc || '';
}

/**
 * Add a new chapter
 */
window.addChapter = function() {
  const chaptersContainer = document.getElementById('chapters-container');
  if (!chaptersContainer) {
    console.error('chapters-container not found');
    return;
  }

  // Calculate index based on existing chapters in DOM to ensure correct numbering
  // This prevents index mismatch when loading edit data or when chapters are deleted
  const existingChapters = chaptersContainer.querySelectorAll('.chapter-card');
  const chapterIndex = existingChapters.length;
  
  // Only push to formData if not already exists (for edit mode where data is pre-loaded)
  // This prevents duplicate entries when loading existing chapters
  if (chapterIndex >= window.formData.chuongs.length) {
    window.formData.chuongs.push({
      tenChuong: '',
      moTa: '',
      baiGiangs: []
    });
  }
  
  // Ensure we have a chapter object at this index
  if (!window.formData.chuongs[chapterIndex]) {
    window.formData.chuongs[chapterIndex] = {
      tenChuong: '',
      moTa: '',
      baiGiangs: []
    };
  }

  const chapterHtml = `
    <div class="chapter-card" data-chapter-index="${chapterIndex}">
      <div class="chapter-header">
        <h4>Chương ${chapterIndex + 1}</h4>
        <button type="button" class="btn btn-sm btn-danger" onclick="window.removeChapter(${chapterIndex}); return false;">
          <i class="fas fa-trash"></i> Xóa
        </button>
      </div>
      
      <div class="form-group">
        <label class="form-label">
          Tên chương <span class="required">*</span>
        </label>
        <input 
          type="text" 
          class="form-input chapter-name" 
          data-chapter-index="${chapterIndex}"
          placeholder="Nhập tên chương"
          required
        />
      </div>

      <div class="form-group">
        <label class="form-label">Mô tả chương</label>
        <textarea 
          class="form-input chapter-description" 
          data-chapter-index="${chapterIndex}"
          rows="2"
          placeholder="Mô tả về chương này"
        ></textarea>
      </div>

      <div class="lessons-container" data-chapter-index="${chapterIndex}">
        <!-- Lessons will be added here -->
      </div>

      <button type="button" class="btn btn-sm btn-secondary" onclick="window.addLesson(${chapterIndex}); return false;">
        <i class="fas fa-plus"></i> Thêm bài giảng
      </button>
    </div>
  `;

  chaptersContainer.insertAdjacentHTML('beforeend', chapterHtml);

  // Add first lesson to this chapter
  addLesson(chapterIndex);

  // Setup chapter input listeners
  const chapterNameInput = chaptersContainer.querySelector(`.chapter-name[data-chapter-index="${chapterIndex}"]`);
  const chapterDescInput = chaptersContainer.querySelector(`.chapter-description[data-chapter-index="${chapterIndex}"]`);
  
  if (chapterNameInput) {
    chapterNameInput._inputHandler = (e) => {
      window.formData.chuongs[chapterIndex].tenChuong = e.target.value;
    };
    chapterNameInput.addEventListener('input', chapterNameInput._inputHandler);
  }
  
  if (chapterDescInput) {
    chapterDescInput._inputHandler = (e) => {
      window.formData.chuongs[chapterIndex].moTa = e.target.value;
    };
    chapterDescInput.addEventListener('input', chapterDescInput._inputHandler);
  }
};

/**
 * Remove a chapter
 */
window.removeChapter = function(chapterIndex) {
  if (!confirm('Bạn có chắc chắn muốn xóa chương này? Tất cả bài giảng trong chương sẽ bị xóa.')) {
    return;
  }

  window.formData.chuongs.splice(chapterIndex, 1);
  
  const chaptersContainer = document.getElementById('chapters-container');
  if (chaptersContainer) {
    const chapterCard = chaptersContainer.querySelector(`.chapter-card[data-chapter-index="${chapterIndex}"]`);
    if (chapterCard) {
      chapterCard.remove();
    }
    
    // Re-index remaining chapters - IMPORTANT: Use sequential index based on position in DOM
    const remainingChapters = chaptersContainer.querySelectorAll('.chapter-card');
    remainingChapters.forEach((card, index) => {
      card.setAttribute('data-chapter-index', index);
      const header = card.querySelector('h4');
      if (header) {
        header.textContent = `Chương ${index + 1}`;
      }
      
      // Update remove button onclick (only the chapter header button, not lesson buttons)
      const chapterHeader = card.querySelector('.chapter-header');
      if (chapterHeader) {
        const removeBtn = chapterHeader.querySelector('.btn-danger');
        if (removeBtn) {
          removeBtn.setAttribute('onclick', `window.removeChapter(${index}); return false;`);
        }
      }
      
      // Update chapter inputs
      const chapterNameInput = card.querySelector('.chapter-name');
      const chapterDescInput = card.querySelector('.chapter-description');
      if (chapterNameInput) {
        chapterNameInput.setAttribute('data-chapter-index', index);
        // Update event listener
        chapterNameInput.removeEventListener('input', chapterNameInput._inputHandler);
        chapterNameInput._inputHandler = (e) => {
          window.formData.chuongs[index].tenChuong = e.target.value;
        };
        chapterNameInput.addEventListener('input', chapterNameInput._inputHandler);
      }
      if (chapterDescInput) {
        chapterDescInput.setAttribute('data-chapter-index', index);
        // Update event listener
        chapterDescInput.removeEventListener('input', chapterDescInput._inputHandler);
        chapterDescInput._inputHandler = (e) => {
          window.formData.chuongs[index].moTa = e.target.value;
        };
        chapterDescInput.addEventListener('input', chapterDescInput._inputHandler);
      }
      
      // Update lessons container
      const lessonsContainer = card.querySelector('.lessons-container');
      if (lessonsContainer) {
        lessonsContainer.setAttribute('data-chapter-index', index);
      }
      
      // Update add lesson button
      const addLessonBtn = card.querySelector('button[onclick*="addLesson"]');
      if (addLessonBtn) {
        addLessonBtn.setAttribute('onclick', `window.addLesson(${index})`);
      }
      
      // Re-index lessons
      const lessons = card.querySelectorAll('.lesson-card');
      lessons.forEach((lessonCard, lessonIndex) => {
        lessonCard.setAttribute('data-chapter-index', index);
        lessonCard.setAttribute('data-lesson-index', lessonIndex);
        
        const lessonHeader = lessonCard.querySelector('h5');
        if (lessonHeader) {
          lessonHeader.textContent = `Bài giảng ${lessonIndex + 1}`;
        }
        
        // Update remove lesson button
        const removeLessonBtn = lessonCard.querySelector('.btn-danger');
        if (removeLessonBtn) {
          removeLessonBtn.setAttribute('onclick', `window.removeLesson(${index}, ${lessonIndex})`);
        }
        
        // Update lesson inputs
        const lessonInputs = lessonCard.querySelectorAll('[data-chapter-index], [data-lesson-index]');
        lessonInputs.forEach(input => {
          input.setAttribute('data-chapter-index', index);
          input.setAttribute('data-lesson-index', lessonIndex);
        });
      });
    });
  }
};

/**
 * Add a lesson to a chapter
 */
window.addLesson = function(chapterIndex) {
  const chapter = window.formData.chuongs[chapterIndex];
  if (!chapter) return;

  const lessonIndex = chapter.baiGiangs.length;
  const lessonId = `lesson-${chapterIndex}-${lessonIndex}`;

  chapter.baiGiangs.push({
    tieuDe: '',
    moTa: '',
    duongDanVideo: '',
    thoiLuong: null,
    mienPhiXem: false
  });

  const lessonsContainer = document.querySelector(`.lessons-container[data-chapter-index="${chapterIndex}"]`);
  if (!lessonsContainer) return;

  const lessonHtml = `
    <div class="lesson-card" data-chapter-index="${chapterIndex}" data-lesson-index="${lessonIndex}">
      <div class="lesson-header">
        <h5>Bài giảng ${lessonIndex + 1}</h5>
        <button type="button" class="btn btn-sm btn-danger" onclick="window.removeLesson(${chapterIndex}, ${lessonIndex})">
          <i class="fas fa-trash"></i>
        </button>
      </div>

      <div class="form-group">
        <label class="form-label">
          Tiêu đề bài giảng <span class="required">*</span>
        </label>
        <input 
          type="text" 
          class="form-input lesson-title" 
          data-chapter-index="${chapterIndex}"
          data-lesson-index="${lessonIndex}"
          placeholder="Nhập tiêu đề bài giảng"
          required
        />
      </div>

      <div class="form-group">
        <label class="form-label">Mô tả bài giảng</label>
        <textarea 
          class="form-input lesson-description" 
          data-chapter-index="${chapterIndex}"
          data-lesson-index="${lessonIndex}"
          rows="2"
          placeholder="Mô tả về bài giảng này"
        ></textarea>
      </div>

      <div class="form-group">
        <label class="form-label">
          Video bài giảng <span class="required">*</span>
        </label>
        <div class="video-upload-container">
          <input 
            type="file" 
            class="form-input video-file-input" 
            data-chapter-index="${chapterIndex}"
            data-lesson-index="${lessonIndex}"
            accept="video/*"
          />
          <div class="video-upload-status" data-chapter-index="${chapterIndex}" data-lesson-index="${lessonIndex}">
            <span class="upload-status-text">Chưa chọn video (bắt buộc)</span>
          </div>
        </div>
      </div>

      <div class="form-group">
        <label class="form-label">
          <input 
            type="checkbox" 
            class="lesson-free-preview" 
            data-chapter-index="${chapterIndex}"
            data-lesson-index="${lessonIndex}"
          />
          Cho phép xem thử miễn phí
        </label>
      </div>
    </div>
  `;

  lessonsContainer.insertAdjacentHTML('beforeend', lessonHtml);

  // Setup lesson input listeners
  const lessonTitleInput = lessonsContainer.querySelector(`.lesson-title[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  const lessonDescInput = lessonsContainer.querySelector(`.lesson-description[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  const lessonFreeCheckbox = lessonsContainer.querySelector(`.lesson-free-preview[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  const videoFileInput = lessonsContainer.querySelector(`.video-file-input[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  
  // Setup video file input - ngăn form submit và tự động upload
  if (videoFileInput) {
    videoFileInput.addEventListener('change', async function(e) {
      // Ngăn mọi event propagation
      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();
      
      // Đảm bảo modal không bị đóng
      if (currentModal) {
        currentModal.isOpen = true; // Force keep modal open
      }
      
      try {
        await window.handleVideoFileSelect(chapterIndex, lessonIndex, this, e);
      } catch (error) {
        console.error('Error in video file select:', error);
        // Không làm gì, chỉ log error
      }
      
      return false;
    }, { capture: true }); // Use capture phase to catch early
  }
  
  if (lessonTitleInput) {
    lessonTitleInput.addEventListener('input', (e) => {
      chapter.baiGiangs[lessonIndex].tieuDe = e.target.value;
    });
  }
  
  if (lessonDescInput) {
    lessonDescInput.addEventListener('input', (e) => {
      chapter.baiGiangs[lessonIndex].moTa = e.target.value;
    });
  }
  
  if (lessonFreeCheckbox) {
    lessonFreeCheckbox.addEventListener('change', (e) => {
      chapter.baiGiangs[lessonIndex].mienPhiXem = e.target.checked;
    });
  }
};

/**
 * Remove a lesson
 */
window.removeLesson = function(chapterIndex, lessonIndex) {
  const chapter = window.formData.chuongs[chapterIndex];
  if (!chapter) return;

  if (chapter.baiGiangs.length <= 1) {
    alert('Mỗi chương phải có ít nhất một bài giảng');
    return;
  }

  chapter.baiGiangs.splice(lessonIndex, 1);
  
  const lessonsContainer = document.querySelector(`.lessons-container[data-chapter-index="${chapterIndex}"]`);
  if (lessonsContainer) {
    const lessonCard = lessonsContainer.querySelector(`[data-lesson-index="${lessonIndex}"]`);
    if (lessonCard) {
      lessonCard.remove();
    }
    
    // Re-index remaining lessons
    const remainingLessons = lessonsContainer.querySelectorAll('.lesson-card');
    remainingLessons.forEach((card, index) => {
      card.setAttribute('data-lesson-index', index);
      card.querySelector('h5').textContent = `Bài giảng ${index + 1}`;
    });
  }
};

/**
 * Handle video file selection - Chỉ validate file, không upload ngay
 * File sẽ được upload khi submit form
 */
window.handleVideoFileSelect = async function(chapterIndex, lessonIndex, fileInput) {
  // Ngăn mọi event bubbling
  if (event) {
    event.preventDefault();
    event.stopPropagation();
    event.stopImmediatePropagation();
  }
  
  if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
    return false;
  }

  const file = fileInput.files[0];
  const statusDiv = document.querySelector(`.video-upload-status[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  
  // Validate file size (500MB max)
  const maxSize = 500 * 1024 * 1024;
  if (file.size > maxSize) {
    if (statusDiv) {
      statusDiv.innerHTML = `<span class="upload-status-text" style="color: red;">File quá lớn (tối đa 500MB)</span>`;
    }
    fileInput.value = ''; // Clear selection
    return false;
  }
  
  // Validate file extension
  const allowedExtensions = ['.mp4', '.avi', '.mov', '.wmv', '.flv', '.webm', '.mkv'];
  const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
  if (!allowedExtensions.includes(fileExtension)) {
    if (statusDiv) {
      statusDiv.innerHTML = `<span class="upload-status-text" style="color: red;">Định dạng không được hỗ trợ</span>`;
    }
    fileInput.value = ''; // Clear selection
    return false;
  }

  // Chỉ hiển thị thông tin file đã chọn, không upload
  if (statusDiv) {
    const fileSizeMB = (file.size / 1024 / 1024).toFixed(2);
    statusDiv.innerHTML = `<span class="upload-status-text" style="color: green;"><i class="fas fa-check"></i> Đã chọn: ${file.name} (${fileSizeMB}MB)</span>`;
  }

  // Lưu file reference vào formData để upload sau
  if (!window.formData.chuongs) {
    window.formData.chuongs = [];
  }
  if (!window.formData.chuongs[chapterIndex]) {
    window.formData.chuongs[chapterIndex] = { baiGiangs: [] };
  }
  if (!window.formData.chuongs[chapterIndex].baiGiangs) {
    window.formData.chuongs[chapterIndex].baiGiangs = [];
  }
  if (!window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex]) {
    window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex] = {};
  }
  
  // Lưu file object để upload sau
  window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex]._fileInput = fileInput;
  window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex]._file = file;

  return true;
};

/**
 * Upload video for a lesson - Đơn giản và dễ hiểu
 */
window.uploadVideo = async function(chapterIndex, lessonIndex, event) {
  // Ngăn event bubbling để không đóng modal
  if (event) {
    event.preventDefault();
    event.stopPropagation();
    event.stopImmediatePropagation();
  }
  
  // Đảm bảo modal không bị đóng
  if (currentModal) {
    currentModal.isOpen = true;
  }
  
  // 1. Kiểm tra file đã chọn chưa
  const fileInput = document.querySelector(`.video-file-input[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
    const statusDiv = document.querySelector(`.video-upload-status[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
    if (statusDiv) {
      statusDiv.innerHTML = '<span class="upload-status-text" style="color: red;">Vui lòng chọn file video</span>';
    }
    return false;
  }

  const file = fileInput.files[0];
  const statusDiv = document.querySelector(`.video-upload-status[data-chapter-index="${chapterIndex}"][data-lesson-index="${lessonIndex}"]`);
  
  // 2. Validate file size (500MB max)
  const maxSize = 500 * 1024 * 1024;
  if (file.size > maxSize) {
    if (statusDiv) {
      statusDiv.innerHTML = `<span class="upload-status-text" style="color: red;">File quá lớn (${(file.size / 1024 / 1024).toFixed(2)}MB). Tối đa 500MB</span>`;
    }
    return false;
  }
  
  // 3. Validate file extension
  const allowedExtensions = ['.mp4', '.avi', '.mov', '.wmv', '.flv', '.webm', '.mkv'];
  const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
  if (!allowedExtensions.includes(fileExtension)) {
    if (statusDiv) {
      statusDiv.innerHTML = '<span class="upload-status-text" style="color: red;">Định dạng không được hỗ trợ</span>';
    }
    return false;
  }
  
  // 4. Update UI - hiển thị đang upload
  if (statusDiv) {
    statusDiv.innerHTML = '<span class="upload-status-text">Đang upload...</span>';
  }

  try {
    // 6. Kiểm tra token
    const token = getAccessToken();
    if (!token) {
      throw new Error('Bạn cần đăng nhập để upload video. Vui lòng đăng nhập lại.');
    }

    // 7. Tạo FormData
    const uploadFormData = new FormData();
    uploadFormData.append('video', file);

    // 8. Upload video
    const uploadUrl = `${API_BASE_URL}${API_ENDPOINTS.COURSES}/upload-video?folder=videos`;
    
    const response = await fetch(uploadUrl, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
        // KHÔNG set Content-Type cho FormData, browser sẽ tự set với boundary
      },
      body: uploadFormData
    });

    // 9. Xử lý response
    if (!response.ok) {
      const errorText = await response.text();
      let errorMessage = 'Upload video thất bại';
      
      try {
        const errorJson = JSON.parse(errorText);
        errorMessage = errorJson.message || errorMessage;
      } catch (e) {
        errorMessage = errorText || `HTTP ${response.status}: ${response.statusText}`;
      }
      
      // Xử lý các lỗi cụ thể
      if (response.status === 401) {
        errorMessage = 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.';
      } else if (response.status === 403) {
        errorMessage = 'Bạn không có quyền upload video. Vui lòng đảm bảo bạn đã đăng nhập.';
      } else if (response.status === 413) {
        errorMessage = 'File video quá lớn. Vui lòng chọn file nhỏ hơn (tối đa 500MB).';
      }
      
      throw new Error(errorMessage);
    }

    // 10. Parse response
    const result = await response.json();
    
    if (!result.success || !result.data) {
      throw new Error(result.message || 'Upload video thất bại hoặc không nhận được URL');
    }

    const videoUrl = result.data;
    
    // 11. Lưu video URL vào formData
    if (!window.formData.chuongs) {
      window.formData.chuongs = [];
    }
    if (!window.formData.chuongs[chapterIndex]) {
      window.formData.chuongs[chapterIndex] = { baiGiangs: [] };
    }
    if (!window.formData.chuongs[chapterIndex].baiGiangs) {
      window.formData.chuongs[chapterIndex].baiGiangs = [];
    }
    if (!window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex]) {
      window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex] = {};
    }
    
    window.formData.chuongs[chapterIndex].baiGiangs[lessonIndex].duongDanVideo = videoUrl;

    // 12. Update UI - thành công
    if (statusDiv) {
      const fileName = videoUrl.split('/').pop() || videoUrl;
      statusDiv.innerHTML = `<span class="upload-status-text" style="color: green;"><i class="fas fa-check"></i> Đã upload: ${fileName}</span>`;
    }
    
    if (fileInput) {
      fileInput.disabled = true;
    }

    // Đảm bảo modal vẫn mở sau khi upload thành công
    if (currentModal) {
      currentModal.isOpen = true;
      const overlay = document.getElementById('course-form-modal-overlay');
      if (overlay) {
        overlay.classList.add('active');
      }
    }

    return true; // Upload thành công

  } catch (error) {
    console.error('Error uploading video:', error);
    
    // 13. Xử lý lỗi và hiển thị thông báo
    let errorMessage = error.message;
    
    if (error.message.includes('Failed to fetch') || error.name === 'TypeError') {
      errorMessage = 'Không thể kết nối đến server backend. Vui lòng kiểm tra backend có đang chạy không.';
    }
    
    if (statusDiv) {
      statusDiv.innerHTML = `<span class="upload-status-text" style="color: red;"><i class="fas fa-times"></i> ${errorMessage}</span>`;
    }
    
    // KHÔNG hiển thị alert để không làm gián đoạn workflow và không đóng modal
    console.error('Lỗi upload video:', errorMessage);
    
    // Đảm bảo modal vẫn mở
    if (currentModal) {
      currentModal.isOpen = true;
      const overlay = document.getElementById('course-form-modal-overlay');
      if (overlay) {
        overlay.classList.add('active');
      }
    }
    
    if (fileInput) {
      fileInput.disabled = false;
      fileInput.value = ''; // Clear selection on error
    }
    
    return false; // Upload thất bại
  }
};

/**
 * Close modal
 */
window.closeCourseModal = function() {
  if (currentModal) {
    currentModal.close();
    currentModal = null;
  }
};

/**
 * Submit course form
 */
window.submitCourseForm = async function() {
  // Debug: Log formData trước khi validate
  console.log('FormData before validation:', JSON.stringify(window.formData, null, 2));
  
  // Validate form
  if (!validateForm()) {
    return;
  }
  
  // Disable submit button để tránh double submit
  const submitButton = document.querySelector('button[onclick="window.submitCourseForm()"]');
  if (submitButton) {
    submitButton.disabled = true;
    submitButton.textContent = 'Đang tạo khóa học...';
  }

  try {
    // Bước 1: Upload tất cả video trước (chỉ upload video mới, giữ nguyên video cũ)
    const token = getAccessToken();
    if (!token) {
      throw new Error('Bạn cần đăng nhập để tạo khóa học. Vui lòng đăng nhập lại.');
    }

    // Upload tất cả video mới (chỉ upload nếu có file mới)
    for (let i = 0; i < window.formData.chuongs.length; i++) {
      const chuong = window.formData.chuongs[i];
      
      for (let j = 0; j < chuong.baiGiangs.length; j++) {
        const baiGiang = chuong.baiGiangs[j];
        
        // Kiểm tra nếu có file mới cần upload
        if (baiGiang._file && baiGiang._fileInput) {
          const statusDiv = document.querySelector(`.video-upload-status[data-chapter-index="${i}"][data-lesson-index="${j}"]`);
          
          if (statusDiv) {
            statusDiv.innerHTML = '<span class="upload-status-text">Đang upload video...</span>';
          }

          try {
            // Upload video
            const uploadFormData = new FormData();
            uploadFormData.append('video', baiGiang._file);

            const uploadUrl = `${API_BASE_URL}${API_ENDPOINTS.COURSES}/upload-video?folder=videos`;
            
            const uploadResponse = await fetch(uploadUrl, {
              method: 'POST',
              headers: {
                'Authorization': `Bearer ${token}`
              },
              body: uploadFormData
            });

            if (!uploadResponse.ok) {
              const errorText = await uploadResponse.text();
              let errorMessage = 'Upload video thất bại';
              
              try {
                const errorJson = JSON.parse(errorText);
                errorMessage = errorJson.message || errorMessage;
              } catch (e) {
                errorMessage = errorText || `HTTP ${uploadResponse.status}: ${uploadResponse.statusText}`;
              }
              
              throw new Error(`Chương "${chuong.tenChuong}", Bài giảng "${baiGiang.tieuDe}": ${errorMessage}`);
            }

            const uploadResult = await uploadResponse.json();
            
            if (!uploadResult.success || !uploadResult.data) {
              throw new Error(`Chương "${chuong.tenChuong}", Bài giảng "${baiGiang.tieuDe}": Upload video thất bại hoặc không nhận được URL`);
            }

            // Lưu video URL vào formData
            baiGiang.duongDanVideo = uploadResult.data;

            if (statusDiv) {
              const fileName = uploadResult.data.split('/').pop() || uploadResult.data;
              statusDiv.innerHTML = `<span class="upload-status-text" style="color: green;"><i class="fas fa-check"></i> Đã upload: ${fileName}</span>`;
            }

            // Xóa file reference sau khi upload thành công
            delete baiGiang._file;
            delete baiGiang._fileInput;
          } catch (error) {
            console.error(`Error uploading video for chapter ${i}, lesson ${j}:`, error);
            if (statusDiv) {
              statusDiv.innerHTML = `<span class="upload-status-text" style="color: red;"><i class="fas fa-times"></i> ${error.message}</span>`;
            }
            throw error;
          }
        } else if (isEditMode && baiGiang.duongDanVideo) {
          // Trong edit mode, nếu không có file mới, giữ nguyên video URL cũ
          // Không cần làm gì, duongDanVideo đã có sẵn
        }
      }
    }

    // Bước 2: Tạo hoặc cập nhật khóa học với dữ liệu đã có video URL
  // Debug: Log formData sau khi upload video
    console.log('FormData after video upload:', JSON.stringify(window.formData, null, 2));

    // Prepare data
    const courseData = {
      idDanhMuc: parseInt(window.formData.idDanhMuc),
      tenKhoaHoc: window.formData.tenKhoaHoc,
      moTaNgan: window.formData.moTaNgan || '',
      moTaChiTiet: window.formData.moTaChiTiet || '',
      giaBan: parseFloat(window.formData.giaBan) || 0,
      mucDo: window.formData.mucDo || 'Cơ bản',
      yeuCauTruoc: window.formData.yeuCauTruoc || '',
      hocDuoc: window.formData.hocDuoc || '',
      chuongs: window.formData.chuongs.map(chuong => ({
        tenChuong: chuong.tenChuong,
        moTa: chuong.moTa || '',
        baiGiangs: chuong.baiGiangs.map(baiGiang => ({
          tieuDe: baiGiang.tieuDe,
          moTa: baiGiang.moTa || '',
          duongDanVideo: baiGiang.duongDanVideo,
          thoiLuong: baiGiang.thoiLuong || null,
          mienPhiXem: baiGiang.mienPhiXem || false
        }))
      }))
    };

    // Call appropriate API based on mode
    let response;
    if (isEditMode) {
      response = await updateCourseWithCurriculum(currentCourseId, courseData);
    } else {
      const fetchResponse = await fetch(`${API_BASE_URL}${API_ENDPOINTS.COURSES}/with-curriculum`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(courseData)
      });
      response = await fetchResponse.json();
    }

    if (response.success) {
      alert(isEditMode 
        ? 'Cập nhật khóa học thành công! Khóa học đang chờ duyệt lại.' 
        : 'Tạo khóa học thành công! Khóa học đang chờ duyệt.');
      window.closeCourseModal();
      
      // Reload courses list
      if (typeof loadCourses === 'function') {
        loadCourses();
      }
    } else {
      throw new Error(response.message || (isEditMode ? 'Cập nhật khóa học thất bại' : 'Tạo khóa học thất bại'));
    }
  } catch (error) {
    console.error('Error creating course:', error);
    alert('Lỗi: ' + error.message);
    
    // Re-enable submit button
    if (submitButton) {
      submitButton.disabled = false;
      submitButton.textContent = 'Tạo khóa học';
    }
  }
};

/**
 * Validate form
 */
function validateForm() {
  // Validate basic info
  if (!window.formData.idDanhMuc) {
    alert('Vui lòng chọn danh mục');
    return false;
  }

  if (!window.formData.tenKhoaHoc || window.formData.tenKhoaHoc.trim() === '') {
    alert('Vui lòng nhập tên khóa học');
    return false;
  }

  // Validate chapters
  if (window.formData.chuongs.length === 0) {
    alert('Khóa học phải có ít nhất một chương');
    return false;
  }

  for (let i = 0; i < window.formData.chuongs.length; i++) {
    const chuong = window.formData.chuongs[i];
    
    if (!chuong.tenChuong || chuong.tenChuong.trim() === '') {
      alert(`Chương ${i + 1}: Vui lòng nhập tên chương`);
      return false;
    }

    if (!chuong.baiGiangs || chuong.baiGiangs.length === 0) {
      alert(`Chương "${chuong.tenChuong}": Phải có ít nhất một bài giảng`);
      return false;
    }

    for (let j = 0; j < chuong.baiGiangs.length; j++) {
      const baiGiang = chuong.baiGiangs[j];
      
      if (!baiGiang.tieuDe || baiGiang.tieuDe.trim() === '') {
        alert(`Chương "${chuong.tenChuong}", Bài giảng ${j + 1}: Vui lòng nhập tiêu đề`);
        return false;
      }

      // Kiểm tra file video đã chọn - BẮT BUỘC phải có file
      const fileInput = document.querySelector(`.video-file-input[data-chapter-index="${i}"][data-lesson-index="${j}"]`);
      if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
        alert(`Chương "${chuong.tenChuong}", Bài giảng "${baiGiang.tieuDe}": Phải chọn video. Vui lòng chọn file video.`);
        return false;
      }
    }
  }

  return true;
}

