import { createInstructorRequest, getMyInstructorRequest } from "../api/instructorRequestApi.js";
import { showAlert, setLoadingState } from "../utils/UILogin.js";
import { getAccessToken } from "../utils/token.js";
import { isInstructor } from "../utils/authHelper.js";

// Check authentication
if (!getAccessToken()) {
  window.location.href = "login.html?redirect=become-instructor";
}

const requestForm = document.getElementById("instructorRequestForm");
const submitBtn = document.getElementById("submitRequestBtn");
const chungChiFileInput = document.getElementById("chungChiFile");
const thongTinBoSungTextarea = document.getElementById("thongTinBoSung");
const requestStatusSection = document.getElementById("requestStatusSection");
const requestStatusContent = document.getElementById("requestStatusContent");
const filePreview = document.getElementById("filePreview");

// Check if user is already an instructor
async function checkInstructorStatus() {
  try {
    const isInstructorUser = await isInstructor();
    if (isInstructorUser) {
      showAlert("Bạn đã là giảng viên rồi!", "success");
      setTimeout(() => {
        window.location.href = "instructor-dashboard.html";
      }, 2000);
      return true;
    }
    return false;
  } catch (err) {
    console.error("Error checking instructor status:", err);
    return false;
  }
}

// Check existing request status
async function checkExistingRequest() {
  try {
    const request = await getMyInstructorRequest();
    
    if (request) {
      // Show request status
      displayRequestStatus(request);
      requestStatusSection.style.display = "block";
      requestForm.style.display = "none";
      return true;
    } else {
      // No existing request, show form
      requestStatusSection.style.display = "none";
      requestForm.style.display = "block";
      return false;
    }
  } catch (err) {
    console.error("Error checking existing request:", err);
    // If error, still show form
    requestStatusSection.style.display = "none";
    requestForm.style.display = "block";
    return false;
  }
}

// Display request status
function displayRequestStatus(request) {
  const status = request.trangThai || request.TrangThai;
  const ngayGui = request.ngayGui || request.NgayGui;
  const lyDoTuChoi = request.lyDoTuChoi || request.LyDoTuChoi;
  
  let statusBadge = '';
  let statusText = '';
  let statusClass = '';
  
  if (status === 'Chờ duyệt' || status === 'Cho duyet') {
    statusBadge = '<span class="request-status-badge status-pending">⏳ Chờ duyệt</span>';
    statusText = `
      <p style="margin: 10px 0;">Yêu cầu của bạn đã được gửi và đang chờ duyệt.</p>
      <p style="color: #666; font-size: 14px;">Ngày gửi: ${formatDate(ngayGui)}</p>
      <p style="color: #666; font-size: 14px; margin-top: 10px;">
        Vui lòng đợi quản trị viên xem xét yêu cầu của bạn. Bạn sẽ nhận được thông báo khi có kết quả.
      </p>
    `;
  } else if (status === 'Đã duyệt' || status === 'Da duyet') {
    statusBadge = '<span class="request-status-badge status-approved">✅ Đã duyệt</span>';
    statusText = `
      <p style="margin: 10px 0; color: #155724;">Chúc mừng! Yêu cầu của bạn đã được duyệt.</p>
      <p style="color: #666; font-size: 14px;">Ngày duyệt: ${formatDate(request.ngayDuyet || request.NgayDuyet)}</p>
      <p style="color: #666; font-size: 14px; margin-top: 10px;">
        Bạn có thể bắt đầu tạo khóa học ngay bây giờ.
      </p>
      <div style="margin-top: 15px;">
        <a href="instructor-dashboard.html" class="login__button" style="display: inline-block; text-decoration: none; padding: 10px 20px;">
          Đi đến trang giảng viên
        </a>
      </div>
    `;
  } else if (status === 'Đã từ chối' || status === 'Da tu choi') {
    statusBadge = '<span class="request-status-badge status-rejected">❌ Đã từ chối</span>';
    statusText = `
      <p style="margin: 10px 0; color: #721c24;">Yêu cầu của bạn đã bị từ chối.</p>
      ${lyDoTuChoi ? `<p style="color: #666; font-size: 14px; margin-top: 10px;"><strong>Lý do:</strong> ${lyDoTuChoi}</p>` : ''}
      <p style="color: #666; font-size: 14px; margin-top: 10px;">
        Bạn có thể gửi lại yêu cầu mới với thông tin đầy đủ hơn.
      </p>
      <div style="margin-top: 15px;">
        <button type="button" class="login__button" id="resubmitBtn" style="width: 100%;">
          Gửi lại yêu cầu
        </button>
      </div>
    `;
  }
  
  requestStatusContent.innerHTML = statusBadge + statusText;
  
  // Add resubmit button handler if exists
  const resubmitBtn = document.getElementById("resubmitBtn");
  if (resubmitBtn) {
    resubmitBtn.addEventListener("click", () => {
      requestStatusSection.style.display = "none";
      requestForm.style.display = "block";
    });
  }
}

// Format date
function formatDate(dateString) {
  if (!dateString) return 'N/A';
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  } catch {
    return dateString;
  }
}

// File preview handler
chungChiFileInput.addEventListener("change", (e) => {
  const file = e.target.files[0];
  if (!file) {
    filePreview.innerHTML = "";
    return;
  }

  // Validate file size (10MB max)
  const maxSize = 10 * 1024 * 1024; // 10MB
  if (file.size > maxSize) {
    showAlert("File quá lớn. Vui lòng chọn file nhỏ hơn 10MB.", "error");
    e.target.value = "";
    filePreview.innerHTML = "";
    return;
  }

  // Validate file type
  const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'application/pdf', 
                        'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
  if (!allowedTypes.includes(file.type)) {
    showAlert("Định dạng file không được hỗ trợ. Vui lòng chọn file ảnh hoặc tài liệu (PDF, DOC, DOCX).", "error");
    e.target.value = "";
    filePreview.innerHTML = "";
    return;
  }

  // Show preview
  if (file.type.startsWith('image/')) {
    const reader = new FileReader();
    reader.onload = (e) => {
      filePreview.innerHTML = `
        <img src="${e.target.result}" alt="Preview" />
        <div class="file-info">
          <strong>${file.name}</strong><br>
          Kích thước: ${(file.size / 1024 / 1024).toFixed(2)} MB
        </div>
      `;
    };
    reader.readAsDataURL(file);
  } else {
    filePreview.innerHTML = `
      <div class="file-info">
        <strong>${file.name}</strong><br>
        Kích thước: ${(file.size / 1024 / 1024).toFixed(2)} MB<br>
        Loại: ${file.type}
      </div>
    `;
  }
});

// Form submit handler
requestForm.addEventListener("submit", async (e) => {
  e.preventDefault();
  
  const file = chungChiFileInput.files[0];
  if (!file) {
    showAlert("Vui lòng chọn file chứng chỉ", "error");
    return;
  }

  setLoadingState(true, "submitRequestBtn");

  try {
    const thongTinBoSung = thongTinBoSungTextarea.value.trim();
    const result = await createInstructorRequest(file, thongTinBoSung);
    
    showAlert(
      result.message || "Yêu cầu đã được gửi thành công! Vui lòng đợi duyệt.",
      "success"
    );

    // Reload page to show status
    setTimeout(() => {
      window.location.reload();
    }, 2000);
  } catch (err) {
    showAlert(err.message || "Gửi yêu cầu thất bại. Vui lòng thử lại.", "error");
  } finally {
    setLoadingState(false, "submitRequestBtn");
  }
});

// Initialize page
(async () => {
  const isInstructorUser = await checkInstructorStatus();
  if (!isInstructorUser) {
    await checkExistingRequest();
  }
})();
