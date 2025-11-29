/**
 * Format price to VND currency
 */
export function formatPrice(price) {
  if (!price || price === 0) {
    return 'Miễn phí';
  }
  
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(price);
}

/**
 * Get star rating display
 */
export function getStars(rating) {
  if (!rating) return '';
  
  const fullStars = Math.floor(rating);
  const halfStar = rating % 1 >= 0.5 ? 1 : 0;
  const emptyStars = 5 - fullStars - halfStar;
  
  return '★'.repeat(fullStars) + 
         (halfStar ? '☆' : '') + 
         '☆'.repeat(emptyStars);
}

/**
 * Format duration from seconds to readable format
 */
export function formatDuration(seconds) {
  if (!seconds) return '0 phút';
  
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  
  if (hours > 0) {
    return `${hours} giờ ${minutes} phút`;
  }
  return `${minutes} phút`;
}

/**
 * Format total duration from minutes
 */
export function formatTotalDuration(minutes) {
  if (!minutes) return '0 phút';
  
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  
  if (hours > 0) {
    return `${hours} giờ ${mins > 0 ? mins + ' phút' : ''}`;
  }
  return `${mins} phút`;
}

/**
 * Get level badge color
 */
export function getLevelColor(level) {
  const colors = {
    'Beginner': '#28a745',
    'Intermediate': '#ffc107',
    'Advanced': '#dc3545',
    'All Levels': '#17a2b8'
  };
  
  return colors[level] || '#6c757d';
}

/**
 * Truncate text to specified length
 */
export function truncateText(text, maxLength = 100) {
  if (!text) return '';
  
  if (text.length <= maxLength) {
    return text;
  }
  
  return text.substring(0, maxLength) + '...';
}

/**
 * Format student count
 */
export function formatStudentCount(count) {
  if (!count) return '0 học viên';
  
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K học viên`;
  }
  
  return `${count} học viên`;
}

/**
 * Format rating count
 */
export function formatRatingCount(count) {
  if (!count) return '(0)';
  
  if (count >= 1000) {
    return `(${(count / 1000).toFixed(1)}K)`;
  }
  
  return `(${count})`;
}

/**
 * Get course URL
 */
export function getCourseUrl(courseId) {
  return `/pages/course-detail.html?id=${courseId}`;
}

/**
 * Get courses by category URL
 */
export function getCategoryCoursesUrl(categoryId) {
  return `/pages/courses.html?categoryId=${categoryId}`;
}

/**
 * Get search results URL
 */
export function getSearchUrl(searchTerm) {
  return `/pages/courses.html?search=${encodeURIComponent(searchTerm)}`;
}

/**
 * Check if course is free
 */
export function isFree(price) {
  return !price || price === 0;
}

/**
 * Get discount percentage
 */
export function getDiscountPercentage(originalPrice, discountedPrice) {
  if (!originalPrice || !discountedPrice) return 0;
  
  return Math.round(((originalPrice - discountedPrice) / originalPrice) * 100);
}