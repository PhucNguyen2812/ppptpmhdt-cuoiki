import { getCourseById, getCourseCurriculum, getCoursesByInstructor } from '../api/courseApi.js';
import { addToCart } from '../api/cartApi.js';
import { formatPrice, getStars, formatTotalDuration, formatRatingCount, formatStudentCount } from '../utils/courseHelper.js';
import { API_BASE_URL, DEFAULT_IMAGES } from '../config.js';
import { getAccessToken } from '../utils/token.js';

// Get course ID from URL
const urlParams = new URLSearchParams(window.location.search);
const courseId = urlParams.get('id');

// Mock reviews data (will be replaced with API call when available)
const mockReviews = [
    {
        id: 1,
        idHocVien: 1,
        tenHocVien: 'Laura M.',
        anhDaiDien: null,
        diemDanhGia: 5,
        binhLuan: 'This course was in-depth, informative, and entertaining. I like how it considered AI use for every area of life. I was almost done before the course was upgraded to include the 3 paths. That\'s a great idea. I appreciate the updates so that I can follow-up and update my knowledge in the future.',
        ngayDanhGia: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000)
    },
    {
        id: 2,
        idHocVien: 2,
        tenHocVien: 'Saniya',
        anhDaiDien: null,
        diemDanhGia: 5,
        binhLuan: 'This course is an excellent introduction to AI and ChatGPT! The lessons are clear, practical, and easy to follow. I learned a lot about generative AI and how to apply it in real life - highly recommended for anyone curious about modern AI tools.',
        ngayDanhGia: new Date(Date.now() - 21 * 24 * 60 * 60 * 1000)
    },
    {
        id: 3,
        idHocVien: 3,
        tenHocVien: 'Melissa M.',
        anhDaiDien: null,
        diemDanhGia: 5,
        binhLuan: 'The course is well-structured and presents information in a clear, accessible manner, making it easy to follow. However, for individuals with no prior background in AI, certain sections may feel overly complex or fast paced. While I\'ve gained valuable insights, I believe the learning experience could be enhanced by offering smaller, topic-specific modules that focus more on foundational concepts. A greater emphasis on beginner-level material would make the course more approachable for newcomers.',
        ngayDanhGia: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
    },
    {
        id: 4,
        idHocVien: 4,
        tenHocVien: 'Rishiraj S.',
        anhDaiDien: null,
        diemDanhGia: 5,
        binhLuan: 'It was a very good course for an introduction to various AI tools. Not too related to my work but was a good opportunity to figure out what you can do with various AI tools.',
        ngayDanhGia: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
    },
    {
        id: 5,
        idHocVien: 5,
        tenHocVien: 'John D.',
        anhDaiDien: null,
        diemDanhGia: 4,
        binhLuan: 'Great course overall! The instructor explains concepts clearly and provides practical examples. Some sections could use more depth, but it\'s a solid introduction to the topic.',
        ngayDanhGia: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000)
    },
    {
        id: 6,
        idHocVien: 6,
        tenHocVien: 'Sarah K.',
        anhDaiDien: null,
        diemDanhGia: 5,
        binhLuan: 'Excellent course! Very comprehensive and well-organized. The hands-on exercises really helped me understand the material better.',
        ngayDanhGia: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000)
    }
];

// Format date relative to now
function formatRelativeDate(date) {
    if (!date) return '';
    
    const now = new Date();
    const reviewDate = new Date(date);
    const diffMs = now - reviewDate;
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    
    if (diffDays === 0) return 'Hôm nay';
    if (diffDays === 1) return '1 ngày trước';
    if (diffDays < 7) return `${diffDays} ngày trước`;
    if (diffDays < 30) {
        const weeks = Math.floor(diffDays / 7);
        return `${weeks} ${weeks === 1 ? 'tuần' : 'tuần'} trước`;
    }
    if (diffDays < 365) {
        const months = Math.floor(diffDays / 30);
        return `${months} ${months === 1 ? 'tháng' : 'tháng'} trước`;
    }
    const years = Math.floor(diffDays / 365);
    return `${years} ${years === 1 ? 'năm' : 'năm'} trước`;
}

// Get initials from name
function getInitials(name) {
    if (!name) return 'U';
    const parts = name.split(' ');
    if (parts.length >= 2) {
        return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return name[0].toUpperCase();
}

// Load course data
async function loadCourseDetail() {
    if (!courseId) {
        alert('Không tìm thấy khóa học');
        window.location.href = 'index.html';
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}v1/courses/${courseId}`);
        if (!response.ok) {
            throw new Error('Failed to fetch course');
        }
        const apiResponse = await response.json();
        const course = apiResponse.data;

        // Render course header
        renderCourseHeader(course);
        
        // Render course preview
        renderCoursePreview(course);
        
        // Render pricing box
        renderPricingBox(course);
        
        // Load curriculum
        await loadCurriculum();
        
        // Load instructor info
        renderInstructor(course);
        
        // Load reviews
        loadReviews(course);
        
        // Load other courses by instructor
        if (course.giangVien?.id) {
            await loadOtherCoursesByInstructor(course.giangVien.id, course.id);
        }
        
    } catch (error) {
        console.error('Error loading course:', error);
        document.querySelector('.course-title').textContent = 'Không thể tải thông tin khóa học';
    }
}

// Render course header
function renderCourseHeader(course) {
    document.getElementById('courseTitle').textContent = course.tenKhoaHoc || 'Khóa học';
    document.getElementById('courseTitleBreadcrumb').textContent = course.tenKhoaHoc || 'Khóa học';
    
    if (course.moTaNgan) {
        document.getElementById('courseSubtitle').textContent = course.moTaNgan;
    }
    
    // Breadcrumbs
    if (course.danhMuc?.tenDanhMuc) {
        document.getElementById('categoryBreadcrumb').textContent = course.danhMuc.tenDanhMuc;
    }
    
    // Badges
    const badgesContainer = document.getElementById('courseBadges');
    if (course.soLuongHocVien > 1000) {
        badgesContainer.innerHTML = '<span class="badge-item">Bán chạy nhất</span>';
    }
    
    // Rating and stats
    const ratingContainer = document.getElementById('courseRatingHeader');
    if (course.diemDanhGia) {
        ratingContainer.innerHTML = `
            <span class="rating-number">${course.diemDanhGia.toFixed(1)}</span>
            <span class="stars">${getStars(course.diemDanhGia)}</span>
            <span class="rating-count">${formatRatingCount(course.soLuongDanhGia)}</span>
        `;
    }
    
    const statsContainer = document.getElementById('courseStatsHeader');
    statsContainer.textContent = `${formatStudentCount(course.soLuongHocVien)}`;
    
    // Instructor
    const instructorContainer = document.getElementById('courseInstructorHeader');
    if (course.giangVien) {
        instructorContainer.innerHTML = `
            Được tạo bởi <a href="#">${course.giangVien.hoTen || 'Giảng viên'}</a>
        `;
    }
    
    // Update info
    document.getElementById('courseUpdateInfo').textContent = 'Lần cập nhật gần đây nhất 11/2025';
}

// Render course preview
function renderCoursePreview(course) {
    const previewContainer = document.getElementById('coursePreview');
    
    if (course.videoGioiThieu) {
        previewContainer.innerHTML = `
            <div class="preview-video">
                <video controls>
                    <source src="${course.videoGioiThieu}" type="video/mp4">
                </video>
            </div>
        `;
    } else {
        previewContainer.innerHTML = `
            <div class="preview-video">
                <div class="preview-placeholder">
                    <i class="fas fa-play-circle" style="font-size: 64px; margin-right: 16px;"></i>
                    <span>Xem trước khóa học này</span>
                </div>
            </div>
        `;
    }
}

// Render pricing box
function renderPricingBox(course) {
    // Preview thumbnail
    const thumbnailContainer = document.getElementById('coursePreviewThumbnail');
    if (course.hinhDaiDien) {
        thumbnailContainer.innerHTML = `
            <img src="${course.hinhDaiDien}" alt="${course.tenKhoaHoc}" onerror="this.src='${DEFAULT_IMAGES.COURSE}'">
        `;
    } else {
        thumbnailContainer.innerHTML = `
            <div style="width: 100%; height: 100%; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); display: flex; align-items: center; justify-content: center; color: white;">
                <i class="fas fa-play-circle" style="font-size: 48px;"></i>
            </div>
        `;
    }
    
    // Pricing
    const currentPrice = course.giaBan || 0;
    const originalPrice = currentPrice * 1.2; // Mock original price (20% higher)
    const discount = Math.round(((originalPrice - currentPrice) / originalPrice) * 100);
    
    document.getElementById('currentPrice').textContent = formatPrice(currentPrice);
    document.getElementById('originalPrice').textContent = formatPrice(originalPrice);
    document.getElementById('discountBadge').textContent = `Giảm ${discount}%`;
    
    // Urgency message
    document.getElementById('urgencyMessage').innerHTML = `
        <i class="fas fa-clock"></i>
        <span>1 ngày còn lại với mức giá này!</span>
    `;
    
    // Course includes
    const includesList = document.getElementById('includesList');
    includesList.innerHTML = `
        <li><i class="fas fa-video"></i> 40 giờ video theo yêu cầu</li>
        <li><i class="fas fa-file-alt"></i> 22 bài viết</li>
        <li><i class="fas fa-download"></i> 142 tài nguyên có thể tải xuống</li>
        <li><i class="fas fa-mobile-alt"></i> Truy cập trên thiết bị di động và TV</li>
        <li><i class="fas fa-infinity"></i> Quyền truy cập đầy đủ suốt đời</li>
        <li><i class="fas fa-closed-captioning"></i> Phụ đề chuẩn</li>
        <li><i class="fas fa-volume-up"></i> Nội dung mô tả về âm thanh hiện có</li>
        <li><i class="fas fa-certificate"></i> Giấy chứng nhận hoàn thành</li>
    `;
}

// Load curriculum
async function loadCurriculum() {
    try {
        const response = await getCourseCurriculum(courseId);
        const curriculum = response.data;
        
        if (!curriculum || !curriculum.chuongs || curriculum.chuongs.length === 0) {
            document.getElementById('courseCurriculum').innerHTML = '<p>Chưa có nội dung khóa học</p>';
            return;
        }
        
        // Calculate totals
        let totalLectures = 0;
        let totalDuration = 0;
        
        curriculum.chuongs.forEach(chuong => {
            if (chuong.baiGiangs) {
                totalLectures += chuong.baiGiangs.length;
                chuong.baiGiangs.forEach(bai => {
                    if (bai.thoiLuong) {
                        totalDuration += bai.thoiLuong;
                    }
                });
            }
        });
        
        document.getElementById('courseContentSummary').textContent = 
            `${curriculum.chuongs.length} phần - ${totalLectures} bài giảng - ${formatTotalDuration(totalDuration)} tổng thời lượng`;
        
        // Render curriculum
        const curriculumContainer = document.getElementById('courseCurriculum');
        curriculumContainer.innerHTML = curriculum.chuongs.map((chuong, index) => `
            <div class="curriculum-section ${index === 0 ? 'expanded' : ''}">
                <div class="curriculum-section-header" onclick="toggleCurriculumSection(${index})">
                    <div>
                        <div class="curriculum-section-title">${chuong.tenChuong || `Chương ${index + 1}`}</div>
                        <div class="curriculum-section-info">
                            ${chuong.baiGiangs?.length || 0} bài giảng - ${formatTotalDuration(
                                chuong.baiGiangs?.reduce((sum, bai) => sum + (bai.thoiLuong || 0), 0) || 0
                            )}
                        </div>
                    </div>
                    <i class="fas fa-chevron-down"></i>
                </div>
                <div class="curriculum-section-content">
                    ${chuong.baiGiangs?.map((bai, baiIndex) => `
                        <div class="curriculum-item">
                            <i class="fas fa-play-circle curriculum-item-icon"></i>
                            <span class="curriculum-item-title">${bai.tenBaiGiang || `Bài ${baiIndex + 1}`}</span>
                            <span class="curriculum-item-duration">${formatTotalDuration(bai.thoiLuong || 0)}</span>
                        </div>
                    `).join('') || ''}
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading curriculum:', error);
        document.getElementById('courseCurriculum').innerHTML = '<p>Không thể tải nội dung khóa học</p>';
    }
}

// Toggle curriculum section
window.toggleCurriculumSection = function(index) {
    const sections = document.querySelectorAll('.curriculum-section');
    const section = sections[index];
    section.classList.toggle('expanded');
};

// Render course description
function renderDescription(course) {
    const descContainer = document.getElementById('courseDescription');
    if (course.moTaChiTiet) {
        // Preserve line breaks and formatting
        descContainer.innerHTML = course.moTaChiTiet.replace(/\n/g, '<br>');
    } else {
        descContainer.textContent = 'Chưa có mô tả chi tiết cho khóa học này.';
    }
}

// Render what you'll learn
function renderLearnList(course) {
    const learnContainer = document.getElementById('learnList');
    if (course.hocDuoc) {
        const items = course.hocDuoc.split('\n').filter(item => item.trim());
        learnContainer.innerHTML = items.map(item => `<li>${item.trim()}</li>`).join('');
    } else {
        // Default learn items
        learnContainer.innerHTML = `
            <li>Nắm vững các khái niệm cơ bản</li>
            <li>Áp dụng kiến thức vào thực tế</li>
            <li>Xây dựng các dự án thực tế</li>
            <li>Phát triển kỹ năng chuyên sâu</li>
        `;
    }
}

// Render instructor
function renderInstructor(course) {
    if (!course.giangVien) {
        document.getElementById('instructorSection').style.display = 'none';
        return;
    }
    
    const instructor = course.giangVien;
    const instructorContainer = document.getElementById('instructorInfo');
    
    instructorContainer.innerHTML = `
        <div class="instructor-card">
            <img src="${instructor.anhDaiDien || DEFAULT_IMAGES.AVATAR}" 
                 alt="${instructor.hoTen}" 
                 class="instructor-avatar"
                 onerror="this.src='${DEFAULT_IMAGES.AVATAR}'">
            <div class="instructor-details">
                <div class="instructor-name">${instructor.hoTen || 'Giảng viên'}</div>
                <div class="instructor-title">AI Expert & Bestselling Instructor</div>
                <div class="instructor-stats">
                    <div class="instructor-stat">
                        <span class="instructor-stat-value">${course.diemDanhGia?.toFixed(1) || '4.5'}</span>
                        <span class="instructor-stat-label">Xếp hạng giảng viên</span>
                    </div>
                    <div class="instructor-stat">
                        <span class="instructor-stat-value">${formatRatingCount(course.soLuongDanhGia || 0)}</span>
                        <span class="instructor-stat-label">Đánh giá</span>
                    </div>
                    <div class="instructor-stat">
                        <span class="instructor-stat-value">${formatStudentCount(course.soLuongHocVien || 0)}</span>
                        <span class="instructor-stat-label">Học viên</span>
                    </div>
                    <div class="instructor-stat">
                        <span class="instructor-stat-value">7</span>
                        <span class="instructor-stat-label">Khóa học</span>
                    </div>
                </div>
                <div class="instructor-bio">
                    ${instructor.moTa || 'Giảng viên có nhiều kinh nghiệm trong lĩnh vực này. Đã giảng dạy cho hàng trăm nghìn học viên trên toàn thế giới.'}
                </div>
            </div>
        </div>
    `;
    
    document.getElementById('instructorNameForCourses').textContent = instructor.hoTen || 'Giảng viên';
}

// Load reviews
function loadReviews(course) {
    // Sort reviews by date (newest first)
    const sortedReviews = [...mockReviews].sort((a, b) => new Date(b.ngayDanhGia) - new Date(a.ngayDanhGia));
    
    // Show first 4 reviews
    const displayedReviews = sortedReviews.slice(0, 4);
    
    // Render reviews summary
    const avgRating = course.diemDanhGia || 4.5;
    const totalReviews = course.soLuongDanhGia || mockReviews.length;
    
    document.getElementById('reviewsSummary').innerHTML = `
        <div class="reviews-summary-rating">${avgRating.toFixed(1)}</div>
        <div>
            <div class="reviews-summary-stars">${getStars(avgRating)}</div>
            <div class="reviews-summary-count">${formatRatingCount(totalReviews)}</div>
        </div>
    `;
    
    // Render reviews list
    const reviewsContainer = document.getElementById('reviewsList');
    reviewsContainer.innerHTML = displayedReviews.map(review => `
        <div class="review-card">
            <div class="review-header">
                <div class="review-avatar">${getInitials(review.tenHocVien)}</div>
                <div class="review-info">
                    <div class="review-name">${review.tenHocVien}</div>
                    <div class="review-rating">${getStars(review.diemDanhGia)}</div>
                    <div class="review-date">${formatRelativeDate(review.ngayDanhGia)}</div>
                </div>
            </div>
            <div class="review-text">${review.binhLuan}</div>
            <div class="review-helpful">
                <span class="review-helpful-text">Bạn thấy hữu ích?</span>
                <button class="review-helpful-btn">
                    <i class="fas fa-thumbs-up"></i>
                </button>
                <button class="review-helpful-btn">
                    <i class="fas fa-thumbs-down"></i>
                </button>
            </div>
        </div>
    `).join('');
    
    // Store all reviews for modal
    window.allReviews = sortedReviews;
    window.courseRating = avgRating;
    window.totalReviewsCount = totalReviews;
}

// Show all reviews modal
function showAllReviewsModal() {
    const modal = document.getElementById('reviewsModal');
    modal.classList.add('active');
    
    // Render rating breakdown
    const breakdown = calculateRatingBreakdown(window.allReviews);
    const breakdownContainer = document.getElementById('ratingBreakdown');
    breakdownContainer.innerHTML = `
        <h3>Phân tích đánh giá</h3>
        ${[5, 4, 3, 2, 1].map(rating => {
            const count = breakdown[rating] || 0;
            const percent = window.allReviews.length > 0 ? (count / window.allReviews.length) * 100 : 0;
            return `
                <div class="rating-item">
                    <div class="rating-item-stars">${getStars(rating)}</div>
                    <div class="rating-item-bar">
                        <div class="rating-item-bar-fill" style="width: ${percent}%"></div>
                    </div>
                    <div class="rating-item-percent">${Math.round(percent)}%</div>
                </div>
            `;
        }).join('')}
    `;
    
    // Render all reviews
    const allReviewsContainer = document.getElementById('allReviewsList');
    allReviewsContainer.innerHTML = window.allReviews.map(review => `
        <div class="review-card">
            <div class="review-header">
                <div class="review-avatar">${getInitials(review.tenHocVien)}</div>
                <div class="review-info">
                    <div class="review-name">${review.tenHocVien}</div>
                    <div class="review-rating">${getStars(review.diemDanhGia)}</div>
                    <div class="review-date">${formatRelativeDate(review.ngayDanhGia)}</div>
                </div>
            </div>
            <div class="review-text">${review.binhLuan}</div>
            <div class="review-helpful">
                <span class="review-helpful-text">Bạn thấy hữu ích?</span>
                <button class="review-helpful-btn">
                    <i class="fas fa-thumbs-up"></i>
                </button>
                <button class="review-helpful-btn">
                    <i class="fas fa-thumbs-down"></i>
                </button>
            </div>
        </div>
    `).join('');
    
    // Update modal header
    document.getElementById('modalRatingText').textContent = `${window.courseRating.toFixed(1)} xếp hạng khóa học`;
    document.getElementById('modalRatingCount').textContent = ` • ${formatRatingCount(window.totalReviewsCount)}`;
}

// Calculate rating breakdown
function calculateRatingBreakdown(reviews) {
    const breakdown = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };
    reviews.forEach(review => {
        breakdown[review.diemDanhGia] = (breakdown[review.diemDanhGia] || 0) + 1;
    });
    return breakdown;
}

// Close reviews modal
function closeReviewsModal() {
    document.getElementById('reviewsModal').classList.remove('active');
}

// Load other courses by instructor
async function loadOtherCoursesByInstructor(instructorId, currentCourseId) {
    try {
        const response = await getCoursesByInstructor(instructorId, 1, 12);
        const courses = response.data.items || [];
        
        // Filter out current course and sort by rating
        const otherCourses = courses
            .filter(c => c.id !== parseInt(currentCourseId))
            .sort((a, b) => (b.diemDanhGia || 0) - (a.diemDanhGia || 0))
            .slice(0, 3);
        
        if (otherCourses.length === 0) {
            document.getElementById('otherCoursesSection').style.display = 'none';
            return;
        }
        
        const coursesContainer = document.getElementById('otherCoursesGrid');
        coursesContainer.innerHTML = otherCourses.map(course => `
            <div class="course-card" onclick="window.location.href='course-detail.html?id=${course.id}'">
                <img src="${course.hinhDaiDien || DEFAULT_IMAGES.COURSE}" 
                     alt="${course.tenKhoaHoc}" 
                     class="course-image"
                     onerror="this.src='${DEFAULT_IMAGES.COURSE}'">
                <div class="course-body">
                    <h3 class="course-title">${course.tenKhoaHoc}</h3>
                    <p class="course-instructor">${course.tenGiangVien || 'Giảng viên'}</p>
                    ${course.diemDanhGia ? `
                    <div class="course-rating">
                        <span class="rating-number">${course.diemDanhGia.toFixed(1)}</span>
                        <span class="stars">${getStars(course.diemDanhGia)}</span>
                        <span class="rating-count">${formatRatingCount(course.soLuongDanhGia)}</span>
                    </div>
                    ` : ''}
                    <p class="course-stats">
                        ${formatStudentCount(course.soLuongHocVien)}
                    </p>
                    <div class="course-price">
                        ${formatPrice(course.giaBan)}
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading other courses:', error);
        document.getElementById('otherCoursesSection').style.display = 'none';
    }
}

// Add to cart handler
async function handleAddToCart() {
    const token = getAccessToken();
    if (!token) {
        alert('Vui lòng đăng nhập để thêm khóa học vào giỏ hàng');
        window.location.href = 'login.html';
        return;
    }
    
    try {
        await addToCart(courseId);
        alert('Đã thêm khóa học vào giỏ hàng!');
        // Reload cart count
        if (window.loadCartCount) {
            window.loadCartCount();
        }
    } catch (error) {
        console.error('Error adding to cart:', error);
        alert('Không thể thêm khóa học vào giỏ hàng. Vui lòng thử lại.');
    }
}

// Buy now handler
function handleBuyNow() {
    const token = getAccessToken();
    if (!token) {
        alert('Vui lòng đăng nhập để mua khóa học');
        window.location.href = 'login.html';
        return;
    }
    
    // Redirect to checkout or cart
    window.location.href = 'my-cart.html';
}

// Initialize
document.addEventListener('DOMContentLoaded', async () => {
    await loadCourseDetail();
    
    // Event listeners
    document.getElementById('showAllReviewsBtn').addEventListener('click', showAllReviewsModal);
    document.getElementById('closeReviewsModal').addEventListener('click', closeReviewsModal);
    document.getElementById('addToCartBtn').addEventListener('click', handleAddToCart);
    document.getElementById('buyNowBtn').addEventListener('click', handleBuyNow);
    
    // Close modal when clicking outside
    document.getElementById('reviewsModal').addEventListener('click', (e) => {
        if (e.target.id === 'reviewsModal') {
            closeReviewsModal();
        }
    });
    
    // Load description and learn list after course loads
    setTimeout(async () => {
        try {
            const response = await fetch(`${API_BASE_URL}v1/courses/${courseId}`);
            if (response.ok) {
                const data = await response.json();
                const course = data.data;
                renderDescription(course);
                renderLearnList(course);
            }
        } catch (err) {
            console.error('Error loading course details:', err);
        }
    }, 100);
});

