const ACCESS_TOKEN_KEY = 'accessToken';

export function saveAccessToken(token) {
  localStorage.setItem(ACCESS_TOKEN_KEY, token);
}

export function getAccessToken() {
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function removeAccessToken() {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
}

/**
 * Decode JWT token and extract user information
 * @returns {Object|null} User object or null if invalid
 */
export function getUserFromToken() {
  const token = getAccessToken();
  
  if (!token) {
    return null;
  }
  
  try {
    // JWT format: header.payload.signature
    const parts = token.split('.');
    
    if (parts.length !== 3) {
      console.error('Invalid token format');
      return null;
    }
    
    // Decode base64 payload
    const payload = JSON.parse(atob(parts[1]));
    
    // Extract claims - chuẩn JWT claims
    const emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
    const nameIdentifierClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
    const nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
    const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    
    return {
      id: payload[nameIdentifierClaim] || payload.sub || payload.userId || payload.id,
      email: payload[emailClaim] || payload.email,
      name: payload[nameClaim] || payload.name || payload.email?.split('@')[0] || 'User',
      username: payload[nameClaim] || payload.userName || payload.email,
      role: payload[roleClaim] || payload.role || 'User',
      exp: payload.exp,
      iat: payload.iat,
      iss: payload.iss,
      aud: payload.aud
    };
  } catch (error) {
    console.error('Error decoding token:', error);
    return null;
  }
}

/**
 * Check if token is expired
 * @returns {boolean} True if expired
 */
export function isTokenExpired() {
  const user = getUserFromToken();
  
  if (!user || !user.exp) {
    return true;
  }
  
  const exp = user.exp * 1000; // Convert to milliseconds
  const now = Date.now();
  
  return now >= exp;
}

/**
 * Get token expiry time
 * @returns {Date|null} Expiry date or null
 */
export function getTokenExpiry() {
  const user = getUserFromToken();
  
  if (!user || !user.exp) {
    return null;
  }
  
  return new Date(user.exp * 1000);
}

/**
 * Get time until token expires
 * @returns {number|null} Milliseconds until expiry or null
 */
export function getTimeUntilExpiry() {
  const expiry = getTokenExpiry();
  
  if (!expiry) {
    return null;
  }
  
  return expiry.getTime() - Date.now();
}