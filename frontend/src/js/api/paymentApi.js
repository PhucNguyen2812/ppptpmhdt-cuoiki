import { API_BASE_URL, API_ENDPOINTS } from '../config.js';
import { getAccessToken } from '../utils/token.js';

/**
 * Create order from cart items
 */
export async function createOrder(courseIds, voucherCode = null) {
  try {
    const token = getAccessToken();
    if (!token) {
      throw new Error('Chưa đăng nhập');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.CREATE_ORDER}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        courseIds: courseIds,
        voucherCode: voucherCode
      })
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Tạo đơn hàng thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error creating order:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi tạo đơn hàng'
    };
  }
}

/**
 * Create payment intent
 */
export async function createPaymentIntent(orderId) {
  try {
    const token = getAccessToken();
    if (!token) {
      throw new Error('Chưa đăng nhập');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.CREATE_PAYMENT_INTENT}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        orderId: orderId
      })
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Tạo payment intent thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error creating payment intent:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi tạo payment intent'
    };
  }
}

/**
 * Get order by ID
 */
export async function getOrder(orderId) {
  try {
    const token = getAccessToken();
    if (!token) {
      throw new Error('Chưa đăng nhập');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.GET_ORDER}/${orderId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Lấy thông tin đơn hàng thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error getting order:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi lấy thông tin đơn hàng'
    };
  }
}

/**
 * Confirm payment success (alternative to webhook for local development)
 */
export async function confirmPayment(paymentIntentId) {
  try {
    const token = getAccessToken();
    if (!token) {
      throw new Error('Chưa đăng nhập');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.CONFIRM_PAYMENT}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        paymentIntentId: paymentIntentId
      })
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Xác nhận thanh toán thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error confirming payment:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi xác nhận thanh toán'
    };
  }
}

/**
 * Get user orders
 */
export async function getUserOrders() {
  try {
    const token = getAccessToken();
    if (!token) {
      throw new Error('Chưa đăng nhập');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.GET_USER_ORDERS}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Lấy danh sách đơn hàng thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error getting user orders:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi lấy danh sách đơn hàng'
    };
  }
}


