import { API_BASE_URL, API_ENDPOINTS } from '../config.js';

/**
 * Validate voucher code
 */
export async function validateVoucher(code, courseIds) {
  try {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.VALIDATE_VOUCHER}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        code: code,
        courseIds: courseIds
      })
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Validate voucher thất bại');
    }

    return {
      success: true,
      data: data.data
    };
  } catch (error) {
    console.error('Error validating voucher:', error);
    return {
      success: false,
      message: error.message || 'Có lỗi xảy ra khi validate voucher'
    };
  }
}





