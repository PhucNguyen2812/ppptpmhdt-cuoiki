/**
 * API Index
 * Centralized export of all API functions
 */

import * as categoryApi from './categoryApi.js';
import * as courseApi from './courseApi.js';
import * as authApi from './authApi.js';
import * as cartApi from './cartApi.js';
import * as nguoiDungApi from './nguoiDungApi.js';

// Create API object with all methods
export const API = {
  // Category APIs
  getCategories: categoryApi.getCategories,
  getAllCategories: categoryApi.getAllCategories,
  getCategoryById: categoryApi.getCategoryById,
  
  // Course APIs
  getCourses: courseApi.getCourses,
  getFeaturedCourses: courseApi.getFeaturedCourses,
  getBestSellingCourses: courseApi.getBestSellingCourses,
  getNewestCourses: courseApi.getNewestCourses,
  getCourseById: courseApi.getCourseById,
  getCourseCurriculum: courseApi.getCourseCurriculum,
  getCoursesByInstructor: courseApi.getCoursesByInstructor,
  searchCourses: courseApi.searchCourses,
  
  // Auth APIs
  login: authApi.login,
  register: authApi.register,
  logout: authApi.logout,
  requireAuth: authApi.requireAuth,
  isAuthenticated: authApi.isAuthenticated,
  
  // Cart APIs
  getCart: cartApi.getCart,
  addToCart: cartApi.addToCart,
  removeFromCart: cartApi.removeFromCart,
  getCartCount: cartApi.getCartCount,
  clearCart: cartApi.clearCart,
  
  // User APIs
  getAllUsers: nguoiDungApi.getAllUsers,
  getUserById: nguoiDungApi.getUserById,
  createUser: nguoiDungApi.createUser,
  updateUser: nguoiDungApi.updateUser,
  deleteUser: nguoiDungApi.deleteUser,
  registerAsInstructor: nguoiDungApi.registerAsInstructor,
  getProfile: nguoiDungApi.getProfile
};

// Also export individual APIs for direct imports
export { categoryApi, courseApi, authApi, cartApi, nguoiDungApi };

