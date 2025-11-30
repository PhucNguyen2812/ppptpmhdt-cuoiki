import { getCart, removeFromCart, clearCart } from "../api/cartApi.js";
import { getAccessToken } from "../utils/token.js";
import { DEFAULT_IMAGES } from "../config.js";

// Check authentication
if (!getAccessToken()) {
    window.location.href = "login.html?redirect=my-cart";
}

// DOM Elements
const elements = {
    loadingState: document.getElementById("loadingState"),
    emptyCartState: document.getElementById("emptyCartState"),
    cartContent: document.getElementById("cartContent"),
    cartItemsList: document.getElementById("cartItemsList"),
    totalPrice: document.getElementById("totalPrice"),
    discount: document.getElementById("discount"),
    totalPayment: document.getElementById("totalPayment"),
    clearCartBtn: document.getElementById("clearCartBtn"),
    checkoutBtn: document.getElementById("checkoutBtn")
};

// Initialize
document.addEventListener("DOMContentLoaded", () => {
    loadCart();
    setupEventListeners();
});

// Event Listeners
function setupEventListeners() {
    elements.clearCartBtn.addEventListener("click", handleClearCart);
    elements.checkoutBtn.addEventListener("click", handleCheckout);
}

// Load Cart
async function loadCart() {
    try {
        showLoading();
        const response = await getCart();
        
        if (response.success && response.data) {
            const cart = response.data;
            
            if (!cart.items || cart.items.length === 0) {
                showEmptyCart();
            } else {
                showCartContent(cart);
            }
        } else {
            showEmptyCart();
        }
    } catch (error) {
        console.error("Error loading cart:", error);
        alert("Không thể tải giỏ hàng. Vui lòng thử lại.");
        showEmptyCart();
    }
}

// Show Loading
function showLoading() {
    elements.loadingState.style.display = "block";
    elements.emptyCartState.style.display = "none";
    elements.cartContent.style.display = "none";
}

// Show Empty Cart
function showEmptyCart() {
    elements.loadingState.style.display = "none";
    elements.emptyCartState.style.display = "block";
    elements.cartContent.style.display = "none";
}

// Show Cart Content
function showCartContent(cart) {
    elements.loadingState.style.display = "none";
    elements.emptyCartState.style.display = "none";
    elements.cartContent.style.display = "block";
    
    renderCartItems(cart.items);
    updateSummary(cart);
}

// Render Cart Items
function renderCartItems(items) {
    if (!items || items.length === 0) {
        elements.cartItemsList.innerHTML = "<p>Không có khóa học nào trong giỏ hàng</p>";
        return;
    }
    
    const html = items.map(item => `
        <div class="cart-item" data-item-id="${item.id}">
            <img 
                src="${item.hinhDaiDien || DEFAULT_IMAGES.COURSE}" 
                alt="${item.tenKhoaHoc || 'Khóa học'}"
                class="cart-item-image"
                onerror="this.src='${DEFAULT_IMAGES.COURSE}'"
            >
            <div class="cart-item-content">
                <h3 class="cart-item-title">
                    <a href="course-detail.html?id=${item.idKhoaHoc}">
                        ${item.tenKhoaHoc || 'Khóa học'}
                    </a>
                </h3>
                <div class="cart-item-meta">
                    ${item.tenGiangVien ? `<p>Giảng viên: ${item.tenGiangVien}</p>` : ''}
                    ${item.tenDanhMuc ? `<p>Danh mục: ${item.tenDanhMuc}</p>` : ''}
                </div>
                ${item.diemDanhGia ? `
                <div class="cart-item-rating">
                    <span class="rating-number">${item.diemDanhGia.toFixed(1)}</span>
                    <span class="stars">${getStars(item.diemDanhGia)}</span>
                    <span class="rating-count">(${item.soLuongDanhGia || 0})</span>
                </div>
                ` : ''}
                <div class="cart-item-price">
                    ${formatPrice(item.giaBan)}
                </div>
            </div>
            <div class="cart-item-actions">
                <button class="btn-remove" onclick="handleRemoveItem(${item.id})" title="Xóa khỏi giỏ hàng">
                    <i class="fas fa-trash"></i> Xóa
                </button>
            </div>
        </div>
    `).join("");
    
    elements.cartItemsList.innerHTML = html;
}

// Update Summary
function updateSummary(cart) {
    const tongTienGoc = cart.tongTienGoc || 0;
    const tienGiam = cart.tienGiamVoucher || 0;
    const tongTienThanhToan = cart.tongTienThanhToan || tongTienGoc;
    
    elements.totalPrice.textContent = formatPrice(tongTienGoc);
    elements.discount.textContent = formatPrice(tienGiam);
    elements.totalPayment.textContent = formatPrice(tongTienThanhToan);
}

// Handle Remove Item
window.handleRemoveItem = async function(cartItemId) {
    if (!confirm("Bạn có chắc muốn xóa khóa học này khỏi giỏ hàng?")) {
        return;
    }
    
    try {
        const response = await removeFromCart(cartItemId);
        
        if (response.success) {
            // Reload cart
            await loadCart();
            
            // Update cart count in header if exists
            if (window.updateCartCount) {
                window.updateCartCount();
            }
        } else {
            alert("Không thể xóa khóa học. Vui lòng thử lại.");
        }
    } catch (error) {
        console.error("Error removing item:", error);
        alert("Không thể xóa khóa học. Vui lòng thử lại.");
    }
};

// Handle Clear Cart
async function handleClearCart() {
    if (!confirm("Bạn có chắc muốn xóa tất cả khóa học khỏi giỏ hàng?")) {
        return;
    }
    
    try {
        const response = await clearCart();
        
        if (response.success) {
            showEmptyCart();
            
            // Update cart count in header if exists
            if (window.updateCartCount) {
                window.updateCartCount();
            }
        } else {
            alert("Không thể xóa giỏ hàng. Vui lòng thử lại.");
        }
    } catch (error) {
        console.error("Error clearing cart:", error);
        alert("Không thể xóa giỏ hàng. Vui lòng thử lại.");
    }
}

// Handle Checkout
function handleCheckout() {
    // Redirect to checkout page
    window.location.href = 'checkout.html';
}

// Helper Functions
function formatPrice(price) {
    if (!price || price === 0) return "Miễn phí";
    return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND"
    }).format(price);
}

function getStars(rating) {
    const fullStars = Math.floor(rating);
    const halfStar = rating % 1 >= 0.5 ? 1 : 0;
    const emptyStars = 5 - fullStars - halfStar;
    
    return "★".repeat(fullStars) + 
           (halfStar ? "☆" : "") + 
           "☆".repeat(emptyStars);
}



