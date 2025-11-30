/**
 * Authentication Helper
 * Provides utility functions for authentication state management
 */

import { getAccessToken, removeAccessToken, getUserFromToken, isTokenExpired } from './token.js';
import { getProfile } from '../api/nguoiDungApi.js';
import { logout as apiLogout } from '../api/authApi.js';

// Cache user info to avoid repeated API calls
let cachedUserInfo = null;
let userInfoPromise = null;

/**
 * Check if user is logged in
 * @returns {boolean} True if user is authenticated
 */
export function isLoggedIn() {
  const token = getAccessToken();
  
  if (!token) {
    return false;
  }
  
  // Check if token is expired
  if (isTokenExpired()) {
    removeAccessToken();
    cachedUserInfo = null;
    return false;
  }
  
  return true;
}

/**
 * Get user information
 * @returns {Promise<Object|null>} User info or null
 */
export async function getUserInfo() {
  // Check if logged in first
  if (!isLoggedIn()) {
    return null;
  }
  
  // If already fetching, wait for it to complete
  if (userInfoPromise) {
    return await userInfoPromise;
  }
  
  // Return cached info if available AND it has VaiTros/vaiTros (complete info)
  if (cachedUserInfo && (cachedUserInfo.VaiTros || cachedUserInfo.vaiTros)) {
    return cachedUserInfo;
  }
  
  // Try to get user info from token first
  const tokenUser = getUserFromToken();
  
  // If we have basic info from token, use it and fetch full profile
  if (tokenUser) {
    cachedUserInfo = {
      id: tokenUser.id,
      email: tokenUser.email,
      hoTen: tokenUser.name || tokenUser.email?.split('@')[0] || 'User',
      vaiTro: tokenUser.role
    };
    
    // Fetch full profile - WAIT for it to complete
    userInfoPromise = getProfile()
      .then(response => {
        // Handle different response structures
        let profileData = null;
        if (response && response.data) {
          profileData = response.data;
        } else if (response && !response.data && response.id) {
          // Response is the user object directly
          profileData = response;
        }
        
        // Merge with cached info to preserve token-based info
        if (profileData) {
          cachedUserInfo = {
            ...cachedUserInfo,
            ...profileData,
            // Preserve VaiTros/vaiTros if present - prioritize from API
            VaiTros: profileData.VaiTros || profileData.vaiTros || cachedUserInfo?.VaiTros || cachedUserInfo?.vaiTros,
            vaiTros: profileData.vaiTros || profileData.VaiTros || cachedUserInfo?.vaiTros || cachedUserInfo?.VaiTros
          };
        }
        
        userInfoPromise = null;
        return cachedUserInfo;
      })
      .catch(error => {
        console.error('Error fetching user profile:', error);
        userInfoPromise = null;
        return cachedUserInfo; // Return cached basic info
      });
    
    // WAIT for profile to be fetched before returning
    return await userInfoPromise;
  }
  
  // If no token info, try to fetch from API
  try {
    userInfoPromise = getProfile()
      .then(response => {
        // Handle different response structures
        let profileData = null;
        if (response && response.data) {
          profileData = response.data;
        } else if (response && !response.data && response.id) {
          // Response is the user object directly
          profileData = response;
        }
        
        if (profileData) {
          cachedUserInfo = profileData;
        }
        
        userInfoPromise = null;
        return cachedUserInfo;
      });
    
    return await userInfoPromise;
  } catch (error) {
    console.error('Error fetching user info:', error);
    userInfoPromise = null;
    return null;
  }
}

/**
 * Check if user is an instructor
 * @returns {Promise<boolean>} True if user is instructor
 */
export async function isInstructor() {
  const userInfo = await getUserInfo();
  
  if (!userInfo) {
    return false;
  }
  
  // Check if user has instructor role
  // API returns VaiTros as List<string> (array of role names)
  // JSON serialization may convert to camelCase (vaiTros) or PascalCase (VaiTros)
  let roleArray = [];
  
  // Check VaiTros/vaiTros (array) - this is the main source from API
  // Try both PascalCase and camelCase - check camelCase FIRST as API likely returns camelCase
  if (userInfo.vaiTros && Array.isArray(userInfo.vaiTros) && userInfo.vaiTros.length > 0) {
    roleArray = [...userInfo.vaiTros];
  } else if (userInfo.VaiTros && Array.isArray(userInfo.VaiTros) && userInfo.VaiTros.length > 0) {
    roleArray = [...userInfo.VaiTros];
  } else if (userInfo.roles && Array.isArray(userInfo.roles) && userInfo.roles.length > 0) {
    roleArray = [...userInfo.roles];
  }
  
  // Check vaiTro (string) for backward compatibility
  if (userInfo.vaiTro) {
    if (Array.isArray(userInfo.vaiTro)) {
      roleArray = [...roleArray, ...userInfo.vaiTro];
    } else {
      roleArray.push(userInfo.vaiTro);
    }
  }
  
  // Also check role from token if available
  const tokenUser = getUserFromToken();
  if (tokenUser && tokenUser.role) {
    roleArray.push(tokenUser.role);
  }
  
  // Remove duplicates and filter empty values
  roleArray = [...new Set(roleArray.filter(r => r && String(r).trim()))];
  
  // Check for various instructor role formats
  const isInstructorRole = roleArray.some(role => {
    if (!role) return false;
    const roleStr = String(role).toUpperCase().trim();
    return roleStr === 'GIANGVIEN' || 
           roleStr === 'INSTRUCTOR' ||
           roleStr === 'GIẢNG VIÊN';
  });
  
  return isInstructorRole;
}

/**
 * Check if user is an admin
 * @returns {Promise<boolean>} True if user is admin
 */
export async function isAdmin() {
  const userInfo = await getUserInfo();
  
  if (!userInfo) {
    return false;
  }
  
  // Check if user has admin role
  let roleArray = [];
  
  // Check VaiTros/vaiTros (array) - this is the main source from API
  if (userInfo.vaiTros && Array.isArray(userInfo.vaiTros) && userInfo.vaiTros.length > 0) {
    roleArray = [...userInfo.vaiTros];
  } else if (userInfo.VaiTros && Array.isArray(userInfo.VaiTros) && userInfo.VaiTros.length > 0) {
    roleArray = [...userInfo.VaiTros];
  } else if (userInfo.roles && Array.isArray(userInfo.roles) && userInfo.roles.length > 0) {
    roleArray = [...userInfo.roles];
  }
  
  // Check vaiTro (string) for backward compatibility
  if (userInfo.vaiTro) {
    if (Array.isArray(userInfo.vaiTro)) {
      roleArray = [...roleArray, ...userInfo.vaiTro];
    } else {
      roleArray.push(userInfo.vaiTro);
    }
  }
  
  // Also check role from token if available
  const tokenUser = getUserFromToken();
  if (tokenUser && tokenUser.role) {
    roleArray.push(tokenUser.role);
  }
  
  // Remove duplicates and filter empty values
  roleArray = [...new Set(roleArray.filter(r => r && String(r).trim()))];
  
  // Check for various admin role formats
  // Backend uses: QUANTRIVIEN, KIEMDUYETVIEN
  const isAdminRole = roleArray.some(role => {
    if (!role) return false;
    const roleStr = String(role).toUpperCase().trim();
    return roleStr === 'ADMIN' || 
           roleStr === 'ADMINISTRATOR' ||
           roleStr === 'QUANTRIVIEN' ||  // Backend role name
           roleStr === 'QUẢN TRỊ VIÊN' ||
           roleStr === 'KIEMDUYETVIEN' || // Reviewer role
           roleStr === 'KIỂM DUYỆT VIÊN';
  });
  
  return isAdminRole;
}

/**
 * Logout user
 */
export async function logout() {
  try {
    await apiLogout();
  } catch (error) {
    console.error('Error during logout:', error);
  } finally {
    // Clear cache
    cachedUserInfo = null;
    userInfoPromise = null;
    
    // Remove token
    removeAccessToken();
    
    // Redirect to login
    window.location.href = 'login.html';
  }
}

/**
 * Clear cached user info (useful after profile updates)
 */
export function clearUserCache() {
  cachedUserInfo = null;
  userInfoPromise = null;
}

/**
 * Force refresh user info from API (clears cache and fetches fresh data)
 */
export async function refreshUserInfo() {
  clearUserCache();
  return await getUserInfo();
}

// Export as default object for convenience
const AuthHelper = {
  isLoggedIn,
  getUserInfo,
  isInstructor,
  isAdmin,
  logout,
  clearUserCache
};

export default AuthHelper;

