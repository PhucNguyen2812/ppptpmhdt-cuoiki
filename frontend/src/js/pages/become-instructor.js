import { registerAsInstructor } from "../api/nguoiDungApi.js";
import { showAlert, setLoadingState } from "../utils/UILogin.js";
import { getAccessToken } from "../utils/token.js";
import { logout } from "../api/authApi.js";

// Check authentication
if (!getAccessToken()) {
  window.location.href = "login.html?redirect=become-instructor";
}

const registerBtn = document.getElementById("registerInstructorBtn");

registerBtn.addEventListener("click", async () => {
  setLoadingState(true, "registerInstructorBtn");

  try {
    const result = await registerAsInstructor();
    
    showAlert(
      result.message || "Đăng ký làm giảng viên thành công! Bạn có thể bắt đầu tạo khóa học.",
      "success"
    );

    // Thông báo cần đăng nhập lại để cập nhật role
    setTimeout(() => {
      if (confirm("Bạn cần đăng nhập lại để cập nhật quyền giảng viên. Bạn có muốn đăng xuất và đăng nhập lại không?")) {
        logout();
      } else {
        window.location.href = "index.html";
      }
    }, 2000);
  } catch (err) {
    showAlert(err.message || "Đăng ký làm giảng viên thất bại. Vui lòng thử lại.", "error");
  } finally {
    setLoadingState(false, "registerInstructorBtn");
  }
});

