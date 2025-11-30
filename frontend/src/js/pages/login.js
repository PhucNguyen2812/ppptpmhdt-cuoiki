import { login } from "../api/authApi.js";
import { showAlert, setLoadingState } from "../utils/UILogin.js";
import AuthHelper, { refreshUserInfo } from "../utils/authHelper.js";

const form = document.getElementById("loginForm");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("matKhau");

form.addEventListener("submit", async (e) => {
  e.preventDefault();
  setLoadingState(true);

  try {
    const result = await login(emailInput.value, passwordInput.value);
    
    // Clear any cached user info to force refresh
    AuthHelper.clearUserCache();
    
    showAlert("Đăng nhập thành công!", "success");
    
    // Force refresh user info to get latest roles
    try {
      await refreshUserInfo();
    } catch (err) {
      console.warn('Could not refresh user info immediately:', err);
    }

    // Check redirect parameter
    const urlParams = new URLSearchParams(window.location.search);
    const redirect = urlParams.get('redirect');
    
    // Chuyển hướng dựa trên role hoặc redirect parameter
    setTimeout(() => {
      if (redirect === 'become-instructor') {
        window.location.href = "become-instructor.html";
      } else {
        // Redirect về trang chủ (user thường) hoặc admin (nếu là admin)
        const token = localStorage.getItem('accessToken');
        if (token) {
          try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
            const role = payload[roleClaim] || payload.role;
            if (role === 'ADMIN') {
              window.location.href = "admin/index.html";
            } else {
              // Force reload to ensure auth state is updated
              window.location.href = "index.html";
            }
          } catch {
            window.location.href = "index.html";
          }
        } else {
          window.location.href = "index.html";
        }
      }
    }, 1000);
  } catch (err) {
    showAlert(err.message || "Sai email hoặc mật khẩu", "error");
  } finally {
    setLoadingState(false);
  }
});