/**
 * Test Authentication và Authorization
 * File này để test và debug các vấn đề về quyền truy cập
 */

import { getAccessToken, getUserFromToken } from './token.js';
import { getUserInfo } from './authHelper.js';

/**
 * Test toàn bộ authentication flow
 */
export async function testAuthFlow() {
  console.log('=== BẮT ĐẦU TEST AUTHENTICATION ===\n');
  
  // 1. Kiểm tra token
  console.log('1. Kiểm tra Token:');
  const token = getAccessToken();
  if (!token) {
    console.error('❌ KHÔNG CÓ TOKEN! Vui lòng đăng nhập lại.');
    return false;
  }
  console.log('✅ Token tồn tại');
  console.log('   Token (first 50 chars):', token.substring(0, 50) + '...');
  
  // 2. Decode token
  console.log('\n2. Decode Token:');
  const tokenUser = getUserFromToken();
  if (!tokenUser) {
    console.error('❌ KHÔNG THỂ DECODE TOKEN!');
    return false;
  }
  console.log('✅ Token hợp lệ');
  console.log('   User ID:', tokenUser.id);
  console.log('   Email:', tokenUser.email);
  console.log('   Name:', tokenUser.name);
  console.log('   Role từ token:', tokenUser.role);
  
  // 3. Kiểm tra user info từ API
  console.log('\n3. Lấy thông tin user từ API:');
  try {
    const userInfo = await getUserInfo();
    if (!userInfo) {
      console.error('❌ KHÔNG THỂ LẤY THÔNG TIN USER!');
      return false;
    }
    console.log('✅ Lấy được thông tin user');
    console.log('   User Info:', JSON.stringify(userInfo, null, 2));
    
    // Kiểm tra roles
    console.log('\n4. Kiểm tra Roles:');
    const roles = userInfo.vaiTros || userInfo.vaiTros || userInfo.roles || [];
    console.log('   vaiTros (camelCase):', userInfo.vaiTros);
    console.log('   VaiTros (PascalCase):', userInfo.VaiTros);
    console.log('   roles:', userInfo.roles);
    console.log('   vaiTro (string):', userInfo.vaiTro);
    console.log('   Tất cả roles tìm được:', roles);
    
    // Kiểm tra từng role
    if (Array.isArray(roles) && roles.length > 0) {
      console.log('\n5. Phân tích từng role:');
      roles.forEach((role, index) => {
        const roleUpper = String(role).toUpperCase().trim();
        console.log(`   Role ${index + 1}: "${role}" -> "${roleUpper}"`);
        
        const isQuanTriVien = roleUpper === 'QUANTRIVIEN' || roleUpper === 'QUẢN TRỊ VIÊN';
        const isKiemDuyetVien = roleUpper === 'KIEMDUYETVIEN' || roleUpper === 'KIỂM DUYỆT VIÊN';
        const isAdmin = roleUpper === 'ADMIN' || roleUpper === 'ADMINISTRATOR';
        
        console.log(`      - QUANTRIVIEN: ${isQuanTriVien ? '✅' : '❌'}`);
        console.log(`      - KIEMDUYETVIEN: ${isKiemDuyetVien ? '✅' : '❌'}`);
        console.log(`      - ADMIN: ${isAdmin ? '✅' : '❌'}`);
      });
    } else {
      console.warn('⚠️  KHÔNG TÌM THẤY ROLES TRONG USER INFO!');
    }
    
    return true;
  } catch (error) {
    console.error('❌ Lỗi khi lấy user info:', error);
    return false;
  }
}

/**
 * Test API call trực tiếp với fetch để xem response chi tiết
 */
export async function testDirectApiCall() {
  console.log('\n=== TEST API CALL TRỰC TIẾP ===\n');
  
  const token = getAccessToken();
  if (!token) {
    console.error('❌ Không có token!');
    return;
  }
  
  const url = 'http://localhost:5228/api/v1/courses';
  console.log('URL:', url);
  console.log('Token (first 50 chars):', token.substring(0, 50) + '...');
  
  try {
    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });
    
    console.log('\nResponse Status:', response.status);
    console.log('Response Status Text:', response.statusText);
    console.log('Response Headers:', Object.fromEntries(response.headers.entries()));
    
    const responseText = await response.text();
    console.log('Response Body:', responseText);
    
    if (response.ok) {
      try {
        const data = JSON.parse(responseText);
        console.log('Parsed Data:', JSON.stringify(data, null, 2));
      } catch (e) {
        console.log('Response không phải JSON');
      }
    } else {
      console.error('❌ Request thất bại!');
      if (response.status === 403) {
        console.error('⚠️  403 Forbidden - Không có quyền truy cập');
      } else if (response.status === 401) {
        console.error('⚠️  401 Unauthorized - Token không hợp lệ');
      }
    }
  } catch (error) {
    console.error('❌ Lỗi khi gọi API:', error);
  }
}

// Export để có thể gọi từ console
window.testAuthFlow = testAuthFlow;
window.testDirectApiCall = testDirectApiCall;

console.log('✅ Test functions đã được load. Sử dụng:');
console.log('   - testAuthFlow() - Test toàn bộ authentication flow');
console.log('   - testDirectApiCall() - Test API call trực tiếp');






