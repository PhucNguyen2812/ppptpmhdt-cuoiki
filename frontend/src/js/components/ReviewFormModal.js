/**
 * Review Form Modal Component
 * Modal form để tạo hoặc chỉnh sửa đánh giá khóa học
 */

import { createReview, updateReview, getMyReview } from '../api/reviewApi.js';

class ReviewFormModal {
  constructor(courseId, onSuccess) {
    this.courseId = courseId;
    this.onSuccess = onSuccess;
    this.currentReview = null;
    this.modal = null;
  }

  async show() {
    // Kiểm tra xem đã có đánh giá chưa
    try {
      this.currentReview = await getMyReview(this.courseId);
    } catch (error) {
      console.error('Error checking existing review:', error);
      // Nếu lỗi (có thể chưa đăng nhập), vẫn hiển thị form
    }

    const isEditMode = this.currentReview !== null;
    const modalHtml = this.generateModalHtml(isEditMode);
    
    // Tạo modal element
    const modalContainer = document.createElement('div');
    modalContainer.className = 'review-form-modal';
    modalContainer.id = 'reviewFormModal';
    modalContainer.innerHTML = modalHtml;
    document.body.appendChild(modalContainer);

    this.modal = modalContainer;
    this.attachEventListeners();
    this.loadExistingReview();
  }

  generateModalHtml(isEditMode) {
    return `
      <div class="review-form-modal-overlay">
        <div class="review-form-modal-content">
          <div class="review-form-modal-header">
            <h2>${isEditMode ? 'Chỉnh sửa đánh giá' : 'Đánh giá khóa học'}</h2>
            <button class="review-form-modal-close" id="closeReviewFormBtn">
              <i class="fas fa-times"></i>
            </button>
          </div>
          <div class="review-form-modal-body">
            <form id="reviewForm">
              <div class="form-group">
                <label class="form-label">Đánh giá của bạn <span class="required">*</span></label>
                <div class="rating-input">
                  <input type="radio" id="rating5" name="rating" value="5" required>
                  <label for="rating5" class="star-label" data-rating="5">
                    <i class="fas fa-star"></i>
                  </label>
                  <input type="radio" id="rating4" name="rating" value="4">
                  <label for="rating4" class="star-label" data-rating="4">
                    <i class="fas fa-star"></i>
                  </label>
                  <input type="radio" id="rating3" name="rating" value="3">
                  <label for="rating3" class="star-label" data-rating="3">
                    <i class="fas fa-star"></i>
                  </label>
                  <input type="radio" id="rating2" name="rating" value="2">
                  <label for="rating2" class="star-label" data-rating="2">
                    <i class="fas fa-star"></i>
                  </label>
                  <input type="radio" id="rating1" name="rating" value="1">
                  <label for="rating1" class="star-label" data-rating="1">
                    <i class="fas fa-star"></i>
                  </label>
                </div>
                <div class="rating-text" id="ratingText">Chọn số sao</div>
              </div>

              <div class="form-group">
                <label for="reviewComment" class="form-label">Bình luận (tùy chọn)</label>
                <textarea 
                  id="reviewComment" 
                  name="comment" 
                  class="form-control" 
                  rows="5" 
                  maxlength="2000"
                  placeholder="Chia sẻ trải nghiệm của bạn về khóa học này..."
                ></textarea>
                <div class="char-count">
                  <span id="charCount">0</span>/2000 ký tự
                </div>
              </div>

              <div class="form-actions">
                <button type="button" class="btn btn-secondary" id="cancelReviewBtn">Hủy</button>
                <button type="submit" class="btn btn-primary" id="submitReviewBtn">
                  ${isEditMode ? 'Cập nhật đánh giá' : 'Gửi đánh giá'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    `;
  }

  attachEventListeners() {
    // Close button
    const closeBtn = this.modal.querySelector('#closeReviewFormBtn');
    const cancelBtn = this.modal.querySelector('#cancelReviewBtn');
    const overlay = this.modal.querySelector('.review-form-modal-overlay');

    [closeBtn, cancelBtn, overlay].forEach(element => {
      if (element) {
        element.addEventListener('click', (e) => {
          if (e.target === element || e.target.closest('#closeReviewFormBtn') || e.target.closest('#cancelReviewBtn')) {
            this.close();
          }
        });
      }
    });

    // Star rating interaction
    const starLabels = this.modal.querySelectorAll('.star-label');
    const ratingInputs = this.modal.querySelectorAll('input[name="rating"]');
    const ratingText = this.modal.querySelector('#ratingText');

    starLabels.forEach((label, index) => {
      label.addEventListener('click', () => {
        const rating = parseInt(label.getAttribute('data-rating'));
        this.updateStarDisplay(rating);
        this.updateRatingText(rating);
      });

      label.addEventListener('mouseenter', () => {
        const rating = parseInt(label.getAttribute('data-rating'));
        this.updateStarDisplay(rating, true);
        this.updateRatingText(rating);
      });
    });

    // Reset stars on mouse leave
    const ratingInput = this.modal.querySelector('.rating-input');
    if (ratingInput) {
      ratingInput.addEventListener('mouseleave', () => {
        const selectedRating = this.modal.querySelector('input[name="rating"]:checked');
        if (selectedRating) {
          const rating = parseInt(selectedRating.value);
          this.updateStarDisplay(rating);
          this.updateRatingText(rating);
        } else {
          this.updateStarDisplay(0);
          this.updateRatingText(0);
        }
      });
    }

    // Character count
    const commentTextarea = this.modal.querySelector('#reviewComment');
    if (commentTextarea) {
      commentTextarea.addEventListener('input', () => {
        const charCount = this.modal.querySelector('#charCount');
        if (charCount) {
          charCount.textContent = commentTextarea.value.length;
        }
      });
    }

    // Form submission
    const form = this.modal.querySelector('#reviewForm');
    if (form) {
      form.addEventListener('submit', (e) => {
        e.preventDefault();
        this.handleSubmit();
      });
    }
  }

  updateStarDisplay(rating, isHover = false) {
    const starLabels = this.modal.querySelectorAll('.star-label');
    starLabels.forEach((label, index) => {
      const starRating = parseInt(label.getAttribute('data-rating'));
      const icon = label.querySelector('i');
      if (starRating <= rating) {
        icon.classList.remove('far');
        icon.classList.add('fas');
        label.classList.add('active');
      } else {
        icon.classList.remove('fas');
        icon.classList.add('far');
        label.classList.remove('active');
      }
    });
  }

  updateRatingText(rating) {
    const ratingText = this.modal.querySelector('#ratingText');
    if (!ratingText) return;

    const texts = {
      0: 'Chọn số sao',
      1: 'Rất tệ',
      2: 'Tệ',
      3: 'Bình thường',
      4: 'Tốt',
      5: 'Rất tốt'
    };

    ratingText.textContent = texts[rating] || 'Chọn số sao';
  }

  loadExistingReview() {
    if (this.currentReview) {
      // Set rating
      const ratingInput = this.modal.querySelector(`#rating${this.currentReview.diemDanhGia}`);
      if (ratingInput) {
        ratingInput.checked = true;
        this.updateStarDisplay(this.currentReview.diemDanhGia);
        this.updateRatingText(this.currentReview.diemDanhGia);
      }

      // Set comment
      const commentTextarea = this.modal.querySelector('#reviewComment');
      if (commentTextarea && this.currentReview.binhLuan) {
        commentTextarea.value = this.currentReview.binhLuan;
        const charCount = this.modal.querySelector('#charCount');
        if (charCount) {
          charCount.textContent = this.currentReview.binhLuan.length;
        }
      }
    }
  }

  async handleSubmit() {
    const form = this.modal.querySelector('#reviewForm');
    if (!form.checkValidity()) {
      form.reportValidity();
      return;
    }

    const formData = new FormData(form);
    const rating = parseInt(formData.get('rating'));
    const comment = formData.get('comment')?.trim() || null;

    if (!rating || rating < 1 || rating > 5) {
      alert('Vui lòng chọn điểm đánh giá');
      return;
    }

    const submitBtn = this.modal.querySelector('#submitReviewBtn');
    const originalText = submitBtn.textContent;
    
    try {
      submitBtn.disabled = true;
      submitBtn.textContent = 'Đang xử lý...';

      if (this.currentReview) {
        // Update existing review
        await updateReview(this.currentReview.id, rating, comment);
        alert('Cập nhật đánh giá thành công!');
      } else {
        // Create new review
        await createReview(this.courseId, rating, comment);
        alert('Đánh giá thành công! Cảm ơn bạn đã chia sẻ.');
      }

      this.close();
      if (this.onSuccess) {
        this.onSuccess();
      }
    } catch (error) {
      console.error('Error submitting review:', error);
      alert('Lỗi: ' + (error.message || 'Không thể gửi đánh giá. Vui lòng thử lại.'));
    } finally {
      submitBtn.disabled = false;
      submitBtn.textContent = originalText;
    }
  }

  close() {
    if (this.modal) {
      this.modal.remove();
      this.modal = null;
    }
  }
}

export function showReviewFormModal(courseId, onSuccess) {
  const modal = new ReviewFormModal(courseId, onSuccess);
  modal.show();
  return modal;
}

export default ReviewFormModal;




