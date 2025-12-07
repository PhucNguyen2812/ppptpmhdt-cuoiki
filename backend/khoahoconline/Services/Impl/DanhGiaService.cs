using AutoMapper;
using khoahoconline.Data.Repositories;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Review;
using khoahoconline.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Services.Impl
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DanhGiaService> _logger;

        public DanhGiaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DanhGiaService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            _logger.LogInformation($"User {userId} creating review for course {dto.IdKhoaHoc}");

            // Validate: Học viên phải đã đăng ký khóa học
            var enrollment = await _unitOfWork.GetDbContext().DangKyKhoaHocs
                .FirstOrDefaultAsync(dk => dk.IdHocVien == userId 
                    && dk.IdKhoaHoc == dto.IdKhoaHoc 
                    && dk.TrangThai == true);

            if (enrollment == null)
            {
                throw new BadRequestException("Bạn phải đăng ký khóa học trước khi đánh giá");
            }

            // Check if review already exists
            var existingReview = await _unitOfWork.DanhGiaRepository.GetByCourseAndStudentAsync(dto.IdKhoaHoc, userId);
            if (existingReview != null)
            {
                throw new BadRequestException("Bạn đã đánh giá khóa học này. Vui lòng chỉnh sửa đánh giá hiện có.");
            }

            // Validate course exists
            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(dto.IdKhoaHoc);
            if (course == null)
            {
                throw new NotFoundException("Không tìm thấy khóa học");
            }

            // Create review
            var review = new DanhGiaKhoaHoc
            {
                IdKhoaHoc = dto.IdKhoaHoc,
                IdHocVien = userId,
                DiemDanhGia = dto.DiemDanhGia,
                BinhLuan = dto.BinhLuan,
                NgayDanhGia = DateTime.UtcNow,
                TrangThai = true
            };

            await _unitOfWork.DanhGiaRepository.CreateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var createdReview = await _unitOfWork.DanhGiaRepository.GetByIdAsync(review.Id);
            if (createdReview == null)
            {
                throw new Exception("Lỗi khi tạo đánh giá");
            }

            // Map to DTO
            var reviewDto = _mapper.Map<ReviewDto>(createdReview);
            reviewDto.HoTenHocVien = createdReview.IdHocVienNavigation?.HoTen ?? "Người dùng";
            reviewDto.AnhDaiDien = createdReview.IdHocVienNavigation?.AnhDaiDien;

            _logger.LogInformation($"Review {review.Id} created successfully");

            return reviewDto;
        }

        public async Task<ReviewDto> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDto dto)
        {
            _logger.LogInformation($"User {userId} updating review {reviewId}");

            var review = await _unitOfWork.DanhGiaRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new NotFoundException("Không tìm thấy đánh giá");
            }

            // Check ownership
            if (review.IdHocVien != userId)
            {
                throw new UnauthorizedException("Bạn không có quyền chỉnh sửa đánh giá này");
            }

            // Update review
            review.DiemDanhGia = dto.DiemDanhGia;
            review.BinhLuan = dto.BinhLuan;
            review.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.DanhGiaRepository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var updatedReview = await _unitOfWork.DanhGiaRepository.GetByIdAsync(reviewId);
            if (updatedReview == null)
            {
                throw new Exception("Lỗi khi cập nhật đánh giá");
            }

            // Map to DTO
            var reviewDto = _mapper.Map<ReviewDto>(updatedReview);
            reviewDto.HoTenHocVien = updatedReview.IdHocVienNavigation?.HoTen ?? "Người dùng";
            reviewDto.AnhDaiDien = updatedReview.IdHocVienNavigation?.AnhDaiDien;

            _logger.LogInformation($"Review {reviewId} updated successfully");

            return reviewDto;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            _logger.LogInformation($"User {userId} deleting review {reviewId}");

            var review = await _unitOfWork.DanhGiaRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new NotFoundException("Không tìm thấy đánh giá");
            }

            // Check ownership
            if (review.IdHocVien != userId)
            {
                throw new UnauthorizedException("Bạn không có quyền xóa đánh giá này");
            }

            // Soft delete
            review.TrangThai = false;
            review.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.DanhGiaRepository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Review {reviewId} deleted successfully");

            return true;
        }

        public async Task<ReviewDto?> GetMyReviewAsync(int courseId, int userId)
        {
            _logger.LogInformation($"Getting review for user {userId} and course {courseId}");

            var review = await _unitOfWork.DanhGiaRepository.GetByCourseAndStudentAsync(courseId, userId);
            if (review == null)
            {
                return null;
            }

            var reviewDto = _mapper.Map<ReviewDto>(review);
            reviewDto.HoTenHocVien = review.IdHocVienNavigation?.HoTen ?? "Người dùng";
            reviewDto.AnhDaiDien = review.IdHocVienNavigation?.AnhDaiDien;

            return reviewDto;
        }

        public async Task<PagedResult<ReviewDto>> GetReviewsByCourseIdAsync(int courseId, ReviewFilterDto filter)
        {
            _logger.LogInformation($"Getting reviews for course {courseId} with filter");

            // Validate course exists
            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException("Không tìm thấy khóa học");
            }

            // Get reviews
            var reviews = await _unitOfWork.DanhGiaRepository.GetByCourseIdAsync(
                courseId, 
                filter.DiemDanhGia, 
                filter.PageNumber, 
                filter.PageSize);

            var totalCount = await _unitOfWork.DanhGiaRepository.GetTotalCountByCourseIdAsync(
                courseId, 
                filter.DiemDanhGia);

            // Map to DTOs
            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                IdKhoaHoc = r.IdKhoaHoc,
                IdHocVien = r.IdHocVien,
                HoTenHocVien = r.IdHocVienNavigation?.HoTen ?? "Người dùng",
                AnhDaiDien = r.IdHocVienNavigation?.AnhDaiDien,
                DiemDanhGia = r.DiemDanhGia,
                BinhLuan = r.BinhLuan,
                NgayDanhGia = r.NgayDanhGia,
                NgayCapNhat = r.NgayCapNhat
            }).ToList();

            return new PagedResult<ReviewDto>
            {
                Items = reviewDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CourseReviewSummaryDto> GetCourseReviewSummaryAsync(int courseId)
        {
            _logger.LogInformation($"Getting review summary for course {courseId}");

            // Validate course exists
            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException("Không tìm thấy khóa học");
            }

            var averageRating = await _unitOfWork.DanhGiaRepository.GetAverageRatingByCourseIdAsync(courseId);
            var totalReviews = await _unitOfWork.DanhGiaRepository.GetTotalCountByCourseIdAsync(courseId);
            var distribution = await _unitOfWork.DanhGiaRepository.GetRatingDistributionByCourseIdAsync(courseId);

            return new CourseReviewSummaryDto
            {
                IdKhoaHoc = courseId,
                DiemTrungBinh = Math.Round(averageRating, 2),
                TongSoDanhGia = totalReviews,
                SoDanhGia1Sao = distribution.GetValueOrDefault(1, 0),
                SoDanhGia2Sao = distribution.GetValueOrDefault(2, 0),
                SoDanhGia3Sao = distribution.GetValueOrDefault(3, 0),
                SoDanhGia4Sao = distribution.GetValueOrDefault(4, 0),
                SoDanhGia5Sao = distribution.GetValueOrDefault(5, 0)
            };
        }
    }
}




