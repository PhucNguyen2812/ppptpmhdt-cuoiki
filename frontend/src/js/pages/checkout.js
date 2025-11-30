import { getCart } from '../api/cartApi.js';
import { createOrder, createPaymentIntent, confirmPayment } from '../api/paymentApi.js';
import { validateVoucher } from '../api/voucherApi.js';
import { STRIPE_PUBLISHABLE_KEY } from '../config.js';
import { formatPrice } from '../utils/courseHelper.js';
import { getAccessToken } from '../utils/token.js';

// Initialize Stripe
const stripe = Stripe(STRIPE_PUBLISHABLE_KEY);
let elements;
let cardElement;
let currentOrder = null;
let currentVoucher = null;

// DOM Elements
const loadingState = document.getElementById('loadingState');
const errorState = document.getElementById('errorState');
const errorMessage = document.getElementById('errorMessage');
const checkoutContent = document.getElementById('checkoutContent');
const orderItems = document.getElementById('orderItems');
const totalPrice = document.getElementById('totalPrice');
const discountRow = document.getElementById('discountRow');
const discountAmount = document.getElementById('discountAmount');
const totalPayment = document.getElementById('totalPayment');
const voucherCode = document.getElementById('voucherCode');
const applyVoucherBtn = document.getElementById('applyVoucherBtn');
const voucherMessage = document.getElementById('voucherMessage');
const paymentForm = document.getElementById('paymentForm');
const submitPaymentBtn = document.getElementById('submitPaymentBtn');
const submitBtnText = document.getElementById('submitBtnText');
const submitBtnLoading = document.getElementById('submitBtnLoading');
const successModal = document.getElementById('successModal');

// Initialize
document.addEventListener('DOMContentLoaded', async () => {
  // Check authentication
  if (!getAccessToken()) {
    window.location.href = 'login.html';
    return;
  }

  await loadCheckout();
});

async function loadCheckout() {
  try {
    showLoading();
    
    // Get cart
    const cartResponse = await getCart();
    if (!cartResponse.success || !cartResponse.data) {
      showError('Không thể tải giỏ hàng');
      return;
    }

    const cart = cartResponse.data;
    if (!cart.items || cart.items.length === 0) {
      showError('Giỏ hàng trống');
      return;
    }

    // Extract course IDs
    const courseIds = cart.items.map(item => item.khoaHoc.id);

    // Create order
    const orderResponse = await createOrder(courseIds, null);
    if (!orderResponse.success) {
      showError(orderResponse.message || 'Tạo đơn hàng thất bại');
      return;
    }

    currentOrder = orderResponse.data;
    renderOrder(currentOrder);
    initializeStripe();
    hideLoading();
    showContent();
  } catch (error) {
    console.error('Error loading checkout:', error);
    showError('Có lỗi xảy ra khi tải trang thanh toán');
  }
}

function renderOrder(order) {
  // Render order items
  orderItems.innerHTML = order.items.map(item => `
    <div class="order-item">
      <img src="${item.courseImage || '../images/default-course.jpg'}" alt="${item.courseName}" class="order-item-image">
      <div class="order-item-info">
        <h3>${item.courseName}</h3>
        <p>Giảng viên: ${item.instructorName}</p>
      </div>
      <div class="order-item-price">
        ${formatPrice(item.price)}
      </div>
    </div>
  `).join('');

  // Update totals
  totalPrice.textContent = formatPrice(order.tongTienGoc);
  
  if (order.tienGiam && order.tienGiam > 0) {
    discountRow.style.display = 'flex';
    discountAmount.textContent = `-${formatPrice(order.tienGiam)}`;
  } else {
    discountRow.style.display = 'none';
  }
  
  totalPayment.textContent = formatPrice(order.tongTienThanhToan);
}

function initializeStripe() {
  // Create Stripe Elements
  elements = stripe.elements();
  
  const style = {
    base: {
      color: '#32325d',
      fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
      fontSmoothing: 'antialiased',
      fontSize: '16px',
      '::placeholder': {
        color: '#aab7c4'
      }
    },
    invalid: {
      color: '#fa755a',
      iconColor: '#fa755a'
    }
  };

  cardElement = elements.create('card', { style });
  cardElement.mount('#cardElement');

  // Handle real-time validation errors
  cardElement.on('change', ({error}) => {
    const displayError = document.getElementById('cardErrors');
    if (error) {
      displayError.textContent = error.message;
    } else {
      displayError.textContent = '';
    }
  });

  // Handle form submission
  paymentForm.addEventListener('submit', handlePayment);
}

async function handlePayment(event) {
  event.preventDefault();

  if (!currentOrder) {
    alert('Không tìm thấy đơn hàng');
    return;
  }

  try {
    // Disable submit button
    submitPaymentBtn.disabled = true;
    submitBtnText.style.display = 'none';
    submitBtnLoading.style.display = 'inline';

    // Create payment intent
    const intentResponse = await createPaymentIntent(currentOrder.id);
    if (!intentResponse.success) {
      throw new Error(intentResponse.message || 'Tạo payment intent thất bại');
    }

    const { clientSecret } = intentResponse.data;

    // Confirm payment
    const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
      payment_method: {
        card: cardElement,
        billing_details: {
          // You can add billing details here if needed
        }
      }
    });

    if (error) {
      // Show error to user
      const displayError = document.getElementById('cardErrors');
      displayError.textContent = error.message;
      
      // Re-enable submit button
      submitPaymentBtn.disabled = false;
      submitBtnText.style.display = 'inline';
      submitBtnLoading.style.display = 'none';
    } else if (paymentIntent && paymentIntent.status === 'succeeded') {
      // Payment succeeded - Process payment on backend (alternative to webhook)
      const confirmResponse = await confirmPayment(paymentIntent.id);
      if (confirmResponse.success) {
        showSuccessModal();
      } else {
        // Payment succeeded on Stripe but failed to process on backend
        alert('Thanh toán thành công nhưng có lỗi khi xử lý. Vui lòng liên hệ hỗ trợ.');
        console.error('Payment processing error:', confirmResponse.message);
      }
    }
  } catch (error) {
    console.error('Payment error:', error);
    alert('Có lỗi xảy ra khi thanh toán: ' + error.message);
    
    // Re-enable submit button
    submitPaymentBtn.disabled = false;
    submitBtnText.style.display = 'inline';
    submitBtnLoading.style.display = 'none';
  }
}

// Voucher handling
applyVoucherBtn.addEventListener('click', async () => {
  const code = voucherCode.value.trim();
  if (!code) {
    voucherMessage.innerHTML = '<span class="error">Vui lòng nhập mã voucher</span>';
    return;
  }

  if (!currentOrder) {
    voucherMessage.innerHTML = '<span class="error">Không tìm thấy đơn hàng</span>';
    return;
  }

  try {
    applyVoucherBtn.disabled = true;
    applyVoucherBtn.textContent = 'Đang kiểm tra...';

    const courseIds = currentOrder.items.map(item => item.courseId);
    const voucherResponse = await validateVoucher(code, courseIds);

    if (!voucherResponse.success || !voucherResponse.data.isValid) {
      voucherMessage.innerHTML = `<span class="error">${voucherResponse.data.message || 'Mã voucher không hợp lệ'}</span>`;
      applyVoucherBtn.disabled = false;
      applyVoucherBtn.textContent = 'Áp dụng';
      return;
    }

    currentVoucher = voucherResponse.data;
    
    // Recreate order with voucher
    const orderResponse = await createOrder(courseIds, code);
    if (orderResponse.success) {
      currentOrder = orderResponse.data;
      renderOrder(currentOrder);
      voucherMessage.innerHTML = `<span class="success">${currentVoucher.message}</span>`;
    } else {
      voucherMessage.innerHTML = `<span class="error">${orderResponse.message}</span>`;
    }

    applyVoucherBtn.disabled = false;
    applyVoucherBtn.textContent = 'Áp dụng';
  } catch (error) {
    console.error('Voucher error:', error);
    voucherMessage.innerHTML = '<span class="error">Có lỗi xảy ra khi áp dụng voucher</span>';
    applyVoucherBtn.disabled = false;
    applyVoucherBtn.textContent = 'Áp dụng';
  }
});

function showLoading() {
  loadingState.style.display = 'block';
  errorState.style.display = 'none';
  checkoutContent.style.display = 'none';
}

function hideLoading() {
  loadingState.style.display = 'none';
}

function showError(message) {
  loadingState.style.display = 'none';
  errorState.style.display = 'block';
  checkoutContent.style.display = 'none';
  errorMessage.textContent = message;
}

function showContent() {
  loadingState.style.display = 'none';
  errorState.style.display = 'none';
  checkoutContent.style.display = 'block';
}

function showSuccessModal() {
  successModal.style.display = 'flex';
}

function viewOrders() {
  // TODO: Navigate to orders page
  window.location.href = 'index.html';
}


