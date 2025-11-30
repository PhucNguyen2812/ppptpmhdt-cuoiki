import { register } from "../api/authApi.js";
import { showAlert, setLoadingState } from "../utils/UILogin.js";

const form = document.getElementById("registerForm");
const hoTenInput = document.getElementById("hoTen");
const emailInput = document.getElementById("email");
const matKhauInput = document.getElementById("matKhau");
const soDienThoaiInput = document.getElementById("soDienThoai");

form.addEventListener("submit", async (e) => {
  e.preventDefault();
  setLoadingState(true, "registerButton");

  // Validate
  if (hoTenInput.value.trim().length < 2) {
    showAlert("Họ tên phải có ít nhất 2 ký tự", "error");
    setLoadingState(false, "registerButton");
    return;
  }

  if (matKhauInput.value.length < 6) {
    showAlert("Mật khẩu phải có ít nhất 6 ký tự", "error");
    setLoadingState(false, "registerButton");
    return;
  }

  // Email validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(emailInput.value)) {
    showAlert("Email không hợp lệ", "error");
    setLoadingState(false, "registerButton");
    return;
  }

  try {
    const result = await register(
      hoTenInput.value.trim(),
      emailInput.value.trim(),
      matKhauInput.value,
      soDienThoaiInput.value.trim() || null
    );
    
    showAlert("Đăng ký thành công! Đang chuyển hướng...", "success");

    // Chuyển hướng về trang chủ sau 1.5 giây
    setTimeout(() => {
      window.location.href = "index.html";
    }, 1500);
  } catch (err) {
    showAlert(err.message || "Đăng ký thất bại. Vui lòng thử lại.", "error");
  } finally {
    setLoadingState(false, "registerButton");
  }
});

