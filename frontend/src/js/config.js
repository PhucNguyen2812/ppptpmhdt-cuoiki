// API Configuration
export const API_BASE_URL = "https://localhost:7043/api/";

// API Endpoints
export const API_ENDPOINTS = {
    // Auth
    LOGIN: "v1/auth/login",
    REGISTER: "v1/auth/register",
    REFRESH_TOKEN: "v1/auth/refresh-token",
    
    // Categories
    CATEGORIES: "v1/categories",
    CATEGORIES_ALL: "v1/categories/all",
    
    // Courses
    COURSES: "v1/courses",
    COURSES_FEATURED: "v1/courses/featured",
    COURSES_BEST_SELLING: "v1/courses/best-selling",
    COURSES_NEWEST: "v1/courses/newest",
    COURSE_DETAIL: "v1/courses",
    COURSE_CURRICULUM: "v1/courses",
    COURSES_BY_INSTRUCTOR: "v1/courses/instructor",
    
    // User
    NGUOIDUNGS: "v1/nguoidungs",
    PROFILE: "v1/nguoidungs/profile",
    
    // Cart (placeholder for future)
    CART: "v1/cart",
    CART_COUNT: "v1/cart/count"
};

// Default Images
export const DEFAULT_IMAGES = {
    AVATAR: 'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22100%22 height=%22100%22%3E%3Crect fill=%22%235624d0%22 width=%22100%22 height=%22100%22/%3E%3Ctext x=%2250%25%22 y=%2250%25%22 font-family=%22Arial%22 font-size=%2240%22 fill=%22%23fff%22 text-anchor=%22middle%22 dy=%22.3em%22%3EU%3C/text%3E%3C/svg%3E',
    COURSE: 'data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%22280%22 height=%22160%22%3E%3Crect fill=%22%23f0f0f0%22 width=%22280%22 height=%22160%22/%3E%3Ctext x=%2250%25%22 y=%2250%25%22 font-family=%22Arial%22 font-size=%2216%22 fill=%22%23999%22 text-anchor=%22middle%22 dy=%22.3em%22%3ECourse Image%3C/text%3E%3C/svg%3E'
};