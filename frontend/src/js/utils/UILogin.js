export function showAlert(message, type) {
  const alertMessage = document.getElementById("alertMessage");
  alertMessage.textContent = message;
  alertMessage.className = `login__alert login__alert--show login__alert--${type}`;
  
  setTimeout(() => {
    alertMessage.className = "login__alert";
  }, 2000);
}

export function setLoadingState(isLoading, buttonId = "loginButton") {
  const button = document.getElementById(buttonId);
  const emailInput = document.getElementById("email");
  const passwordInput = document.getElementById("matKhau");
  const hoTenInput = document.getElementById("hoTen");
  const soDienThoaiInput = document.getElementById("soDienThoai");

  if (isLoading) {
    if (button) {
      button.classList.add("login__button--loading");
      button.disabled = true;
    }
    if (emailInput) emailInput.disabled = true;
    if (passwordInput) passwordInput.disabled = true;
    if (hoTenInput) hoTenInput.disabled = true;
    if (soDienThoaiInput) soDienThoaiInput.disabled = true;
  } else {
    if (button) {
      button.classList.remove("login__button--loading");
      button.disabled = false;
    }
    if (emailInput) emailInput.disabled = false;
    if (passwordInput) passwordInput.disabled = false;
    if (hoTenInput) hoTenInput.disabled = false;
    if (soDienThoaiInput) soDienThoaiInput.disabled = false;
  }
}