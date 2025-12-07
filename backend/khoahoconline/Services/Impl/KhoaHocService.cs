using AutoMapper;
using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Repositories;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Middleware.Exceptions;
using khoahoconline.Helpers;

namespace khoahoconline.Services.Impl
{
    public class KhoaHocService : IKhoaHocService
    {
        private readonly ILogger<KhoaHocService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public KhoaHocService(
            ILogger<KhoaHocService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<KhoaHocDto>> GetPagedAsync(KhoaHocFilterDto filter)
        {
            _logger.LogInformation($"Getting paged courses with filter: {System.Text.Json.JsonSerializer.Serialize(filter)}");

            var pagedResult = await _unitOfWork.KhoaHocRepository.GetPagedAsync(filter);

            var dtos = pagedResult.Items.Select(k => new KhoaHocDto
            {
                Id = k.Id,
                TenKhoaHoc = k.TenKhoaHoc,
                MoTaNgan = k.MoTaNgan,
                GiaBan = k.GiaBan,
                HinhDaiDien = k.HinhDaiDien,
                MucDo = k.MucDo,
                TrangThai = k.TrangThai,
                SoLuongHocVien = k.SoLuongHocVien,
                DiemDanhGia = k.DiemDanhGia,
                SoLuongDanhGia = k.SoLuongDanhGia,
                IdDanhMuc = k.IdDanhMuc,
                TenDanhMuc = k.IdDanhMucNavigation?.TenDanhMuc,
                IdGiangVien = k.IdGiangVien,
                TenGiangVien = k.IdGiangVienNavigation?.HoTen,
                AnhDaiDienGiangVien = k.IdGiangVienNavigation?.AnhDaiDien
            }).ToList();

            return new PagedResult<KhoaHocDto>
            {
                Items = dtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<KhoaHocDetailDto> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting course detail by id: {id}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithDetailsAsync(id);
            if (course == null || course.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {id}");
            }

            // Kiểm tra trạng thái cho public access
            if (course.TrangThai != true)
            {
                throw new BadRequestException("Khóa học này hiện không khả dụng");
            }

            var dto = new KhoaHocDetailDto
            {
                Id = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                MoTaNgan = course.MoTaNgan,
                MoTaChiTiet = course.MoTaChiTiet,
                YeuCauTruoc = course.YeuCauTruoc,
                HocDuoc = course.HocDuoc,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                TrangThai = course.TrangThai,
                SoLuongHocVien = course.SoLuongHocVien,
                DiemDanhGia = course.DiemDanhGia,
                SoLuongDanhGia = course.SoLuongDanhGia,
                DanhMuc = course.IdDanhMucNavigation != null ? new DanhMucDto
                {
                    Id = course.IdDanhMucNavigation.Id,
                    TenDanhMuc = course.IdDanhMucNavigation.TenDanhMuc,
                    MoTa = course.IdDanhMucNavigation.MoTa,
                    TrangThai = course.IdDanhMucNavigation.TrangThai
                } : null,
                GiangVien = course.IdGiangVienNavigation != null ? new NguoiDungDto
                {
                    Id = course.IdGiangVienNavigation.Id,
                    HoTen = course.IdGiangVienNavigation.HoTen,
                    Email = course.IdGiangVienNavigation.Email,
                    SoDienThoai = course.IdGiangVienNavigation.SoDienThoai,
                    AnhDaiDien = course.IdGiangVienNavigation.AnhDaiDien,
                    TieuSu = course.IdGiangVienNavigation.TieuSu,
                    TrangThai = course.IdGiangVienNavigation.TrangThai
                } : null
            };

            return dto;
        }

        public async Task<CurriculumDto> GetCurriculumAsync(int id, int? userId = null)
        {
            _logger.LogInformation($"Getting curriculum for course: {id}, userId: {userId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(id);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {id}");
            }

            if (course.TrangThai != true)
            {
                throw new BadRequestException("Khóa học này hiện không khả dụng");
            }

            // Kiểm tra học viên đã đăng ký và chưa hết hạn (nếu có userId)
            bool isEnrolledAndNotExpired = false;
            if (userId.HasValue)
            {
                isEnrolledAndNotExpired = await CheckStudentEnrollmentAsync(userId.Value, id);
            }

            var chuongDtos = course.Chuongs
                .Where(c => c.TrangThai == true)
                .OrderBy(c => c.ThuTu)
                .Select(c => new ChuongDto
                {
                    Id = c.Id,
                    TenChuong = c.TenChuong,
                    MoTa = c.MoTa,
                    ThuTu = c.ThuTu,
                    BaiGiangs = c.BaiGiangs
                        .Where(b => b.TrangThai == true)
                        .OrderBy(b => b.ThuTu)
                        .Select(b => new BaiGiangDto
                        {
                            Id = b.Id,
                            TieuDe = b.TieuDe,
                            MoTa = b.MoTa,
                            ThoiLuong = b.ThoiLuong,
                            ThuTu = b.ThuTu,
                            XemThuMienPhi = b.MienPhiXem,
                            // VideoUrl chỉ trả về cho:
                            // 1. Bài giảng xem thử miễn phí (MienPhiXem = true)
                            // 2. Học viên đã đăng ký và chưa hết hạn
                            VideoUrl = (b.MienPhiXem == true || isEnrolledAndNotExpired) ? b.DuongDanVideo : null
                        }).ToList()
                }).ToList();

            var tongSoBaiGiang = chuongDtos.Sum(c => c.BaiGiangs.Count);
            var tongThoiLuong = chuongDtos.Sum(c => c.BaiGiangs.Sum(b => (b.ThoiLuong ?? 0))) / 60; // Convert to minutes

            return new CurriculumDto
            {
                IdKhoaHoc = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                TongSoChuong = chuongDtos.Count,
                TongSoBaiGiang = tongSoBaiGiang,
                TongThoiLuong = tongThoiLuong,
                Chuongs = chuongDtos
            };
        }

        /// <summary>
        /// Kiểm tra học viên đã đăng ký khóa học và chưa hết thời hạn truy cập
        /// </summary>
        private async Task<bool> CheckStudentEnrollmentAsync(int studentId, int courseId)
        {
            var context = _unitOfWork.GetDbContext();
            var now = DateTime.UtcNow;
            
            var enrollment = await context.DangKyKhoaHocs
                .FirstOrDefaultAsync(dk => dk.IdHocVien == studentId 
                    && dk.IdKhoaHoc == courseId 
                    && dk.TrangThai == true
                    && (dk.NgayHetHan == null || dk.NgayHetHan > now)); // Chưa hết thời hạn

            return enrollment != null;
        }

        public async Task<List<KhoaHocDto>> GetFeaturedCoursesAsync(int take = 8)
        {
            _logger.LogInformation($"Getting featured courses, take: {take}");

            var courses = await _unitOfWork.KhoaHocRepository.GetFeaturedCoursesAsync(take);

            return courses.Select(k => new KhoaHocDto
            {
                Id = k.Id,
                TenKhoaHoc = k.TenKhoaHoc,
                MoTaNgan = k.MoTaNgan,
                GiaBan = k.GiaBan,
                HinhDaiDien = k.HinhDaiDien,
                MucDo = k.MucDo,
                TrangThai = k.TrangThai,
                SoLuongHocVien = k.SoLuongHocVien,
                DiemDanhGia = k.DiemDanhGia,
                SoLuongDanhGia = k.SoLuongDanhGia,
                IdDanhMuc = k.IdDanhMuc,
                TenDanhMuc = k.IdDanhMucNavigation?.TenDanhMuc,
                IdGiangVien = k.IdGiangVien,
                TenGiangVien = k.IdGiangVienNavigation?.HoTen,
                AnhDaiDienGiangVien = k.IdGiangVienNavigation?.AnhDaiDien
            }).ToList();
        }

        public async Task<List<KhoaHocDto>> GetBestSellingCoursesAsync(int take = 8)
        {
            _logger.LogInformation($"Getting best-selling courses, take: {take}");

            var courses = await _unitOfWork.KhoaHocRepository.GetBestSellingCoursesAsync(take);

            return courses.Select(k => new KhoaHocDto
            {
                Id = k.Id,
                TenKhoaHoc = k.TenKhoaHoc,
                MoTaNgan = k.MoTaNgan,
                GiaBan = k.GiaBan,
                HinhDaiDien = k.HinhDaiDien,
                MucDo = k.MucDo,
                TrangThai = k.TrangThai,
                SoLuongHocVien = k.SoLuongHocVien,
                DiemDanhGia = k.DiemDanhGia,
                SoLuongDanhGia = k.SoLuongDanhGia,
                IdDanhMuc = k.IdDanhMuc,
                TenDanhMuc = k.IdDanhMucNavigation?.TenDanhMuc,
                IdGiangVien = k.IdGiangVien,
                TenGiangVien = k.IdGiangVienNavigation?.HoTen,
                AnhDaiDienGiangVien = k.IdGiangVienNavigation?.AnhDaiDien
            }).ToList();
        }

        public async Task<List<KhoaHocDto>> GetNewestCoursesAsync(int take = 8)
        {
            _logger.LogInformation($"Getting newest courses, take: {take}");

            var courses = await _unitOfWork.KhoaHocRepository.GetNewestCoursesAsync(take);

            return courses.Select(k => new KhoaHocDto
            {
                Id = k.Id,
                TenKhoaHoc = k.TenKhoaHoc,
                MoTaNgan = k.MoTaNgan,
                GiaBan = k.GiaBan,
                HinhDaiDien = k.HinhDaiDien,
                MucDo = k.MucDo,
                TrangThai = k.TrangThai,
                SoLuongHocVien = k.SoLuongHocVien,
                DiemDanhGia = k.DiemDanhGia,
                SoLuongDanhGia = k.SoLuongDanhGia,
                IdDanhMuc = k.IdDanhMuc,
                TenDanhMuc = k.IdDanhMucNavigation?.TenDanhMuc,
                IdGiangVien = k.IdGiangVien,
                TenGiangVien = k.IdGiangVienNavigation?.HoTen,
                AnhDaiDienGiangVien = k.IdGiangVienNavigation?.AnhDaiDien
            }).ToList();
        }

        public async Task<PagedResult<KhoaHocDto>> GetByInstructorAsync(int instructorId, int pageNumber = 1, int pageSize = 12)
        {
            _logger.LogInformation($"Getting courses by instructor: {instructorId}");

            try
            {
                // Verify instructor exists
                var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);
                if (instructor == null)
                {
                    throw new NotFoundException($"Không tìm thấy giảng viên với id: {instructorId}");
                }

                var pagedResult = await _unitOfWork.KhoaHocRepository.GetByInstructorAsync(instructorId, pageNumber, pageSize);

                // Map to DTOs with error handling for each course
                var dtos = new List<KhoaHocDto>();
                
                foreach (var k in pagedResult.Items)
                {
                    try
                    {
                        dtos.Add(new KhoaHocDto
                        {
                            Id = k.Id,
                            TenKhoaHoc = k.TenKhoaHoc,
                            MoTaNgan = k.MoTaNgan,
                            GiaBan = k.GiaBan,
                            HinhDaiDien = k.HinhDaiDien,
                            MucDo = k.MucDo,
                            TrangThai = k.TrangThai,
                            SoLuongHocVien = k.SoLuongHocVien,
                            DiemDanhGia = k.DiemDanhGia,
                            SoLuongDanhGia = k.SoLuongDanhGia,
                            IdDanhMuc = k.IdDanhMuc,
                            TenDanhMuc = k.IdDanhMucNavigation?.TenDanhMuc,
                            IdGiangVien = k.IdGiangVien,
                            TenGiangVien = instructor.HoTen,
                            AnhDaiDienGiangVien = instructor.AnhDaiDien
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error processing course {k.Id} for instructor {instructorId}. Skipping this course.");
                        // Continue with other courses instead of failing the entire request
                    }
                }

                return new PagedResult<KhoaHocDto>
                {
                    Items = dtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (NotFoundException)
            {
                throw; // Re-throw NotFoundException as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error getting courses by instructor: {instructorId}");
                throw new BadRequestException($"Lỗi khi lấy danh sách khóa học: {ex.Message}");
            }
        }

        public async Task<KhoaHocDto> CreateCourseAsync(CreateCourseDto dto, int instructorId)
        {
            _logger.LogInformation($"Creating course by instructor: {instructorId}");

            // Verify instructor exists
            var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                throw new NotFoundException($"Không tìm thấy giảng viên với id: {instructorId}");
            }

            // Verify category exists
            var category = await _unitOfWork.DanhMucRepository.GetByIdAsync(dto.IdDanhMuc);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {dto.IdDanhMuc}");
            }

            // Create course
            var course = new Data.Entities.KhoaHoc
            {
                TenKhoaHoc = dto.TenKhoaHoc,
                MoTaNgan = dto.MoTaNgan,
                MoTaChiTiet = dto.MoTaChiTiet,
                IdDanhMuc = dto.IdDanhMuc,
                IdGiangVien = instructorId,
                YeuCauTruoc = dto.YeuCauTruoc,
                HocDuoc = dto.HocDuoc,
                GiaBan = dto.GiaBan,
                HinhDaiDien = dto.HinhDaiDien,
                MucDo = dto.MucDo,
                TrangThai = true, // Khóa học mới tạo ở trạng thái hiển thị
                SoLuongHocVien = 0,
                DiemDanhGia = 0,
                SoLuongDanhGia = 0,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow,
                PhienBanHienTai = 1,
                IsDeleted = false
            };

            await _unitOfWork.KhoaHocRepository.CreateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Return DTO
            return new KhoaHocDto
            {
                Id = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                MoTaNgan = course.MoTaNgan,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                TrangThai = course.TrangThai,
                SoLuongHocVien = course.SoLuongHocVien,
                DiemDanhGia = course.DiemDanhGia,
                SoLuongDanhGia = course.SoLuongDanhGia,
                IdDanhMuc = course.IdDanhMuc,
                TenDanhMuc = category.TenDanhMuc,
                IdGiangVien = course.IdGiangVien,
                TenGiangVien = instructor.HoTen,
                AnhDaiDienGiangVien = instructor.AnhDaiDien
            };
        }

        public async Task<KhoaHocDto> CreateCourseWithCurriculumAsync(CreateCourseWithCurriculumDto dto, int instructorId)
        {
            _logger.LogInformation($"Creating course with curriculum by instructor: {instructorId}");

            // Verify instructor exists
            var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                throw new NotFoundException($"Không tìm thấy giảng viên với id: {instructorId}");
            }

            // Verify category exists
            var category = await _unitOfWork.DanhMucRepository.GetByIdAsync(dto.IdDanhMuc);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {dto.IdDanhMuc}");
            }

            // Validate khi publish: Phải có ít nhất 1 chương
            if (dto.Publish)
            {
                if (dto.Chuongs == null || dto.Chuongs.Count == 0)
                {
                    throw new BadRequestException("Khóa học phải có ít nhất 1 chương để publish");
                }

                // Validate: Mỗi chương phải có ít nhất 1 bài giảng
                foreach (var chuong in dto.Chuongs)
                {
                    if (chuong.BaiGiangs == null || chuong.BaiGiangs.Count == 0)
                    {
                        throw new BadRequestException($"Chương '{chuong.TenChuong}' phải có ít nhất một bài giảng");
                    }

                    // Validate: Mỗi bài giảng phải có video
                    foreach (var baiGiang in chuong.BaiGiangs)
                    {
                        if (string.IsNullOrWhiteSpace(baiGiang.DuongDanVideo))
                        {
                            throw new BadRequestException($"Bài giảng '{baiGiang.TieuDe}' phải có video");
                        }
                    }
                }
            }
            // Nếu lưu nháp: Vẫn validate cơ bản nhưng không bắt buộc đầy đủ
            else
            {
                // Validate cơ bản: Nếu có chương thì mỗi chương phải có ít nhất 1 bài giảng
                if (dto.Chuongs != null && dto.Chuongs.Count > 0)
                {
                    foreach (var chuong in dto.Chuongs)
                    {
                        if (chuong.BaiGiangs != null && chuong.BaiGiangs.Count > 0)
                        {
                            // Nếu có bài giảng thì phải có video
                            foreach (var baiGiang in chuong.BaiGiangs)
                            {
                                if (string.IsNullOrWhiteSpace(baiGiang.DuongDanVideo))
                                {
                                    throw new BadRequestException($"Bài giảng '{baiGiang.TieuDe}' phải có video");
                                }
                            }
                        }
                    }
                }
            }

            // Xác định trạng thái: Nếu publish thì tạo approval request, không auto-publish
            var now = DateTime.UtcNow;
            var trangThai = false; // Luôn tạo ở trạng thái draft, cần approval để publish
            DateTime? ngayPublish = null; // Chỉ set khi được approve

            // Create course
            var course = new Data.Entities.KhoaHoc
            {
                TenKhoaHoc = dto.TenKhoaHoc,
                MoTaNgan = dto.MoTaNgan,
                MoTaChiTiet = dto.MoTaChiTiet,
                IdDanhMuc = dto.IdDanhMuc,
                IdGiangVien = instructorId,
                YeuCauTruoc = dto.YeuCauTruoc,
                HocDuoc = dto.HocDuoc,
                GiaBan = dto.GiaBan,
                HinhDaiDien = dto.HinhDaiDien,
                MucDo = dto.MucDo,
                TrangThai = trangThai, // Luôn false, cần approval
                SoLuongHocVien = 0,
                DiemDanhGia = 0,
                SoLuongDanhGia = 0,
                NgayTao = now,
                NgayCapNhat = now,
                NgayPublish = ngayPublish,
                PhienBanHienTai = 1,
                IsDeleted = false
            };

            // Create chapters and lessons before saving course
            int chuongOrder = 1;
            foreach (var chuongDto in dto.Chuongs ?? new List<CreateChuongDto>())
            {
                var chuong = new Data.Entities.Chuong
                {
                    IdKhoaHoc = course.Id,
                    TenChuong = chuongDto.TenChuong,
                    MoTa = chuongDto.MoTa,
                    ThuTu = chuongOrder++,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };

                // Create lessons for this chapter
                int baiGiangOrder = 1;
                foreach (var baiGiangDto in chuongDto.BaiGiangs)
                {
                    var baiGiang = new Data.Entities.BaiGiang
                    {
                        IdChuong = 0, // Will be set after chapter is saved
                        TieuDe = baiGiangDto.TieuDe,
                        MoTa = baiGiangDto.MoTa,
                        DuongDanVideo = baiGiangDto.DuongDanVideo,
                        ThoiLuong = baiGiangDto.ThoiLuong,
                        ThuTu = baiGiangOrder++,
                        MienPhiXem = baiGiangDto.MienPhiXem ?? false,
                        TrangThai = true,
                        NgayTao = DateTime.UtcNow,
                        NgayCapNhat = DateTime.UtcNow
                    };

                    chuong.BaiGiangs.Add(baiGiang);
                }

                course.Chuongs.Add(chuong);
            }

            await _unitOfWork.KhoaHocRepository.CreateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Nếu publish = true, tạo approval request
            if (dto.Publish)
            {
                try
                {
                    var approvalRequest = new Data.Entities.KiemDuyetKhoaHoc
                    {
                        IdKhoaHoc = course.Id,
                        IdNguoiGui = instructorId,
                        TrangThai = "Chờ duyệt",
                        NgayGui = DateTime.UtcNow
                    };

                    await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(approvalRequest);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Created approval request {approvalRequest.Id} for course {course.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to create approval request for course {course.Id}, but course was created");
                    // Không throw exception, vì khóa học đã được tạo thành công
                }
            }

            // Return DTO
            return new KhoaHocDto
            {
                Id = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                MoTaNgan = course.MoTaNgan,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                TrangThai = course.TrangThai,
                SoLuongHocVien = course.SoLuongHocVien,
                DiemDanhGia = course.DiemDanhGia,
                SoLuongDanhGia = course.SoLuongDanhGia,
                IdDanhMuc = course.IdDanhMuc,
                TenDanhMuc = category.TenDanhMuc,
                IdGiangVien = course.IdGiangVien,
                TenGiangVien = instructor.HoTen,
                AnhDaiDienGiangVien = instructor.AnhDaiDien
            };
        }

        public async Task<KhoaHocDto> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int instructorId)
        {
            _logger.LogInformation($"Updating course {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền chỉnh sửa khóa học này");
            }

            // Verify category exists
            var category = await _unitOfWork.DanhMucRepository.GetByIdAsync(dto.IdDanhMuc);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {dto.IdDanhMuc}");
            }

            // Update course
            course.TenKhoaHoc = dto.TenKhoaHoc;
            course.MoTaNgan = dto.MoTaNgan;
            course.MoTaChiTiet = dto.MoTaChiTiet;
            course.IdDanhMuc = dto.IdDanhMuc;
            course.YeuCauTruoc = dto.YeuCauTruoc;
            course.HocDuoc = dto.HocDuoc;
            course.GiaBan = dto.GiaBan;
            course.HinhDaiDien = dto.HinhDaiDien;
            course.MucDo = dto.MucDo;
                // Giữ nguyên trạng thái khi chỉnh sửa
            course.NgayCapNhat = DateTime.UtcNow;
            course.PhienBanHienTai += 1; // Tăng phiên bản

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Kiểm tra và gửi thông báo cho học viên đã đăng ký khóa học
            try
            {
                var courseName = course.TenKhoaHoc ?? "Khóa học";
                var tieuDe = "Khóa học cập nhật";
                var noiDung = $"Khóa học '{courseName}' đã được cập nhật";
                var loai = "Khóa học cập nhật";

                await _unitOfWork.NotificationRepository.CreateNotificationsForCourseStudentsAsync(
                    courseId, 
                    tieuDe, 
                    noiDung, 
                    loai
                );

                _logger.LogInformation($"Đã gửi thông báo cập nhật khóa học {courseId} cho học viên đã đăng ký");
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw để không ảnh hưởng đến việc cập nhật khóa học
                _logger.LogError(ex, $"Lỗi khi gửi thông báo cập nhật khóa học {courseId}");
            }

            // Get instructor for DTO
            var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);

            return new KhoaHocDto
            {
                Id = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                MoTaNgan = course.MoTaNgan,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                TrangThai = course.TrangThai,
                SoLuongHocVien = course.SoLuongHocVien,
                DiemDanhGia = course.DiemDanhGia,
                SoLuongDanhGia = course.SoLuongDanhGia,
                IdDanhMuc = course.IdDanhMuc,
                TenDanhMuc = category.TenDanhMuc,
                IdGiangVien = course.IdGiangVien,
                TenGiangVien = instructor?.HoTen,
                AnhDaiDienGiangVien = instructor?.AnhDaiDien
            };
        }

        public async Task<bool> DeleteCourseAsync(int courseId, int instructorId)
        {
            _logger.LogInformation($"Deleting course {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền xóa khóa học này");
            }

            // Kiểm tra xem khóa học có học viên đăng ký không
            var deleteContext = _unitOfWork.GetDbContext();
            var enrollmentCount = await deleteContext.DangKyKhoaHocs
                .Where(dk => dk.IdKhoaHoc == courseId && dk.TrangThai == true)
                .CountAsync();

            if (enrollmentCount > 0)
            {
                throw new BadRequestException($"Không thể xóa khóa học. Khóa học đã có {enrollmentCount} học viên đăng ký.");
            }

            // Lấy khóa học với tất cả navigation properties để xóa
            var courseToDelete = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(courseId);
            if (courseToDelete == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Xóa các bản ghi liên quan (hard delete)
                // 1. Xóa Notification liên quan
                var notifications = await deleteContext.Notifications
                    .Where(n => n.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (notifications.Any())
                {
                    deleteContext.Notifications.RemoveRange(notifications);
                    _logger.LogInformation($"Đã xóa {notifications.Count} thông báo liên quan đến khóa học {courseId}");
                }

                // 2. Xóa ChiTietGioHang (khóa học trong giỏ hàng)
                var cartItems = await deleteContext.ChiTietGioHangs
                    .Where(c => c.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (cartItems.Any())
                {
                    deleteContext.ChiTietGioHangs.RemoveRange(cartItems);
                    _logger.LogInformation($"Đã xóa {cartItems.Count} item khỏi giỏ hàng liên quan đến khóa học {courseId}");
                }

                // 3. Xóa DanhGiaKhoaHoc
                var reviews = await deleteContext.DanhGiaKhoaHocs
                    .Where(d => d.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (reviews.Any())
                {
                    deleteContext.DanhGiaKhoaHocs.RemoveRange(reviews);
                    _logger.LogInformation($"Đã xóa {reviews.Count} đánh giá liên quan đến khóa học {courseId}");
                }

                // 4. Xóa TienDoHocTap và TienDoHocTapChiTiet
                var progressList = await deleteContext.TienDoHocTaps
                    .Where(t => t.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (progressList.Any())
                {
                    // Xóa TienDoHocTapChiTiet trước
                    foreach (var progress in progressList)
                    {
                        var progressDetails = await deleteContext.TienDoHocTapChiTiets
                            .Where(td => td.IdTienDoHocTap == progress.Id)
                            .ToListAsync();
                        if (progressDetails.Any())
                        {
                            deleteContext.TienDoHocTapChiTiets.RemoveRange(progressDetails);
                        }
                    }
                    deleteContext.TienDoHocTaps.RemoveRange(progressList);
                    _logger.LogInformation($"Đã xóa {progressList.Count} tiến độ học tập liên quan đến khóa học {courseId}");
                }

                // 5. Xóa ChiTietChiaSeDoanhThu
                var revenueShares = await deleteContext.ChiTietChiaSeDoanhThus
                    .Where(c => c.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (revenueShares.Any())
                {
                    deleteContext.ChiTietChiaSeDoanhThus.RemoveRange(revenueShares);
                    _logger.LogInformation($"Đã xóa {revenueShares.Count} bản ghi chia sẻ doanh thu liên quan đến khóa học {courseId}");
                }

                // 6. Xóa KhoaHocKhuyenMai
                var coursePromotions = await deleteContext.KhoaHocKhuyenMais
                    .Where(k => k.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (coursePromotions.Any())
                {
                    deleteContext.KhoaHocKhuyenMais.RemoveRange(coursePromotions);
                    _logger.LogInformation($"Đã xóa {coursePromotions.Count} khuyến mãi liên quan đến khóa học {courseId}");
                }

                // 7. Xóa KhoaHocPhienBan
                var courseVersions = await deleteContext.KhoaHocPhienBans
                    .Where(k => k.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (courseVersions.Any())
                {
                    deleteContext.KhoaHocPhienBans.RemoveRange(courseVersions);
                    _logger.LogInformation($"Đã xóa {courseVersions.Count} phiên bản liên quan đến khóa học {courseId}");
                }

                // 8. Xóa ChiTietDonHang (nếu muốn xóa cứng)
                var orderDetails = await deleteContext.ChiTietDonHangs
                    .Where(c => c.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (orderDetails.Any())
                {
                    deleteContext.ChiTietDonHangs.RemoveRange(orderDetails);
                    _logger.LogInformation($"Đã xóa {orderDetails.Count} chi tiết đơn hàng liên quan đến khóa học {courseId}");
                }

                // 9. Xóa TaiLieuKhoaHoc (tài liệu khóa học)
                var courseMaterials = await deleteContext.TaiLieuKhoaHocs
                    .Where(t => t.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (courseMaterials.Any())
                {
                    deleteContext.TaiLieuKhoaHocs.RemoveRange(courseMaterials);
                    _logger.LogInformation($"Đã xóa {courseMaterials.Count} tài liệu khóa học liên quan đến khóa học {courseId}");
                }

                // 10. Xóa Chuong và BaiGiang (cascade)
                // Load tất cả chương với bài giảng
                var chapters = await deleteContext.Chuongs
                    .Where(c => c.IdKhoaHoc == courseId)
                    .Include(c => c.BaiGiangs)
                    .ToListAsync();

                foreach (var chapter in chapters)
                {
                    // Xóa TaiLieuBaiGiang cho mỗi bài giảng
                    foreach (var baiGiang in chapter.BaiGiangs)
                    {
                        var taiLieu = await deleteContext.TaiLieuBaiGiangs
                            .Where(t => t.IdBaiGiang == baiGiang.Id)
                            .ToListAsync();
                        if (taiLieu.Any())
                        {
                            deleteContext.TaiLieuBaiGiangs.RemoveRange(taiLieu);
                        }
                    }
                    // Xóa bài giảng
                    deleteContext.BaiGiangs.RemoveRange(chapter.BaiGiangs);
                }
                // Xóa chương
                if (chapters.Any())
                {
                    deleteContext.Chuongs.RemoveRange(chapters);
                    _logger.LogInformation($"Đã xóa {chapters.Count} chương và các bài giảng liên quan đến khóa học {courseId}");
                }

                // 11. Xóa DangKyKhoaHoc (chỉ xóa nếu không có học viên đang học - TrangThai = true)
                // Đã kiểm tra ở trên, nhưng để đảm bảo, kiểm tra lại một lần nữa
                var activeEnrollments = await deleteContext.DangKyKhoaHocs
                    .Where(d => d.IdKhoaHoc == courseId && d.TrangThai == true)
                    .CountAsync();
                
                if (activeEnrollments > 0)
                {
                    throw new BadRequestException($"Không thể xóa khóa học. Khóa học đã có {activeEnrollments} học viên đang học.");
                }

                // Xóa tất cả DangKyKhoaHoc (kể cả TrangThai = false) để đảm bảo xóa sạch
                var allEnrollments = await deleteContext.DangKyKhoaHocs
                    .Where(d => d.IdKhoaHoc == courseId)
                    .ToListAsync();
                if (allEnrollments.Any())
                {
                    deleteContext.DangKyKhoaHocs.RemoveRange(allEnrollments);
                    _logger.LogInformation($"Đã xóa {allEnrollments.Count} bản ghi đăng ký khóa học (bao gồm cả inactive)");
                }

                // 12. Cuối cùng xóa khóa học
                deleteContext.KhoaHocs.Remove(courseToDelete);
                await deleteContext.SaveChangesAsync();

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Đã xóa cứng khóa học {courseId} và tất cả dữ liệu liên quan");

                // TODO: Xóa file video và tài liệu từ storage (nếu có)
                // - Xóa hình đại diện: courseToDelete.HinhDaiDien
                // - Xóa video bài giảng: từ BaiGiang.DuongDanVideo
                // - Xóa tài liệu: từ TaiLieuBaiGiang.DuongDan và TaiLieuKhoaHoc.DuongDan

                return true;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Lỗi khi xóa khóa học {courseId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> HideCourseAsync(int courseId, int instructorId)
        {
            _logger.LogInformation($"Hiding course {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền ẩn khóa học này");
            }

            // Hide course (ẩn khỏi public nhưng học viên đã đăng ký vẫn thấy)
            course.TrangThai = false;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnhideCourseAsync(int courseId, int instructorId)
        {
            _logger.LogInformation($"Unhiding course {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền hiển thị lại khóa học này");
            }

            // Hiển thị lại khóa học
            course.TrangThai = true;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Lấy khóa học với curriculum cho instructor để chỉnh sửa
        /// </summary>
        public async Task<CreateCourseWithCurriculumDto> GetCourseForEditAsync(int courseId, int instructorId)
        {
            _logger.LogInformation($"Getting course for edit: {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền chỉnh sửa khóa học này");
            }

            if (!course.IdDanhMuc.HasValue)
            {
                throw new BadRequestException("Khóa học không có danh mục");
            }

            var dto = new CreateCourseWithCurriculumDto
            {
                TenKhoaHoc = course.TenKhoaHoc ?? string.Empty,
                MoTaNgan = course.MoTaNgan,
                MoTaChiTiet = course.MoTaChiTiet,
                IdDanhMuc = course.IdDanhMuc.Value,
                YeuCauTruoc = course.YeuCauTruoc,
                HocDuoc = course.HocDuoc,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                Chuongs = course.Chuongs
                    .Where(c => c.TrangThai == true)
                    .OrderBy(c => c.ThuTu)
                    .Select(c => new CreateChuongDto
                    {
                        TenChuong = c.TenChuong ?? string.Empty,
                        MoTa = c.MoTa,
                        BaiGiangs = c.BaiGiangs
                            .Where(b => b.TrangThai == true)
                            .OrderBy(b => b.ThuTu)
                            .Select(b => new CreateBaiGiangDto
                            {
                                TieuDe = b.TieuDe ?? string.Empty,
                                MoTa = b.MoTa,
                                DuongDanVideo = b.DuongDanVideo ?? string.Empty,
                                ThoiLuong = b.ThoiLuong,
                                MienPhiXem = b.MienPhiXem
                            }).ToList()
                    }).ToList()
            };

            return dto;
        }


        /// <summary>
        /// Cập nhật khóa học kèm chương và bài giảng
        /// </summary>
        public async Task<KhoaHocDto> UpdateCourseWithCurriculumAsync(int courseId, UpdateCourseWithCurriculumDto dto, int instructorId)
        {
            _logger.LogInformation($"Updating course with curriculum {courseId} by instructor: {instructorId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền chỉnh sửa khóa học này");
            }

            // Validate: Mỗi chương phải có ít nhất 1 bài giảng
            foreach (var chuong in dto.Chuongs)
            {
                if (chuong.BaiGiangs == null || chuong.BaiGiangs.Count == 0)
                {
                    throw new BadRequestException($"Chương '{chuong.TenChuong}' phải có ít nhất một bài giảng");
                }

                // Validate: Mỗi bài giảng phải có video
                foreach (var baiGiang in chuong.BaiGiangs)
                {
                    if (string.IsNullOrWhiteSpace(baiGiang.DuongDanVideo))
                    {
                        throw new BadRequestException($"Bài giảng '{baiGiang.TieuDe}' phải có video");
                    }
                }
            }

            // Update course basic info (KHÔNG được thay đổi IdDanhMuc khi edit)
            course.TenKhoaHoc = dto.TenKhoaHoc;
            course.MoTaNgan = dto.MoTaNgan;
            course.MoTaChiTiet = dto.MoTaChiTiet;
            // course.IdDanhMuc = dto.IdDanhMuc; // KHÔNG cho phép thay đổi danh mục
            course.YeuCauTruoc = dto.YeuCauTruoc;
            course.HocDuoc = dto.HocDuoc;
            course.GiaBan = dto.GiaBan;
            course.HinhDaiDien = dto.HinhDaiDien;
            course.MucDo = dto.MucDo;
                // Giữ nguyên trạng thái khi chỉnh sửa
            course.NgayCapNhat = DateTime.UtcNow;
            course.PhienBanHienTai += 1; // Tăng phiên bản

            // Soft delete old chapters and lessons (set TrangThai = false)
            foreach (var oldChuong in course.Chuongs)
            {
                oldChuong.TrangThai = false;
                oldChuong.NgayCapNhat = DateTime.UtcNow;
                foreach (var oldBaiGiang in oldChuong.BaiGiangs)
                {
                    oldBaiGiang.TrangThai = false;
                    oldBaiGiang.NgayCapNhat = DateTime.UtcNow;
                }
            }

            // Create new chapters and lessons
            int chuongOrder = 1;
            foreach (var chuongDto in dto.Chuongs)
            {
                var chuong = new Data.Entities.Chuong
                {
                    IdKhoaHoc = course.Id,
                    TenChuong = chuongDto.TenChuong,
                    MoTa = chuongDto.MoTa,
                    ThuTu = chuongOrder++,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };

                // Create lessons for this chapter
                int baiGiangOrder = 1;
                foreach (var baiGiangDto in chuongDto.BaiGiangs)
                {
                    var baiGiang = new Data.Entities.BaiGiang
                    {
                        IdChuong = 0, // Will be set after chapter is saved
                        TieuDe = baiGiangDto.TieuDe,
                        MoTa = baiGiangDto.MoTa,
                        DuongDanVideo = baiGiangDto.DuongDanVideo,
                        ThoiLuong = baiGiangDto.ThoiLuong,
                        ThuTu = baiGiangOrder++,
                        MienPhiXem = baiGiangDto.MienPhiXem ?? false,
                        TrangThai = true,
                        NgayTao = DateTime.UtcNow,
                        NgayCapNhat = DateTime.UtcNow
                    };

                    chuong.BaiGiangs.Add(baiGiang);
                }

                course.Chuongs.Add(chuong);
            }

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Kiểm tra và gửi thông báo cho học viên đã đăng ký khóa học
            try
            {
                var courseName = course.TenKhoaHoc ?? "Khóa học";
                var tieuDe = "Khóa học cập nhật";
                var noiDung = $"Khóa học '{courseName}' đã được cập nhật";
                var loai = "Khóa học cập nhật";

                await _unitOfWork.NotificationRepository.CreateNotificationsForCourseStudentsAsync(
                    courseId, 
                    tieuDe, 
                    noiDung, 
                    loai
                );

                _logger.LogInformation($"Đã gửi thông báo cập nhật khóa học {courseId} cho học viên đã đăng ký");
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw để không ảnh hưởng đến việc cập nhật khóa học
                _logger.LogError(ex, $"Lỗi khi gửi thông báo cập nhật khóa học {courseId}");
            }

            // Get category and instructor for DTO
            if (!course.IdDanhMuc.HasValue)
            {
                throw new BadRequestException("Khóa học không có danh mục");
            }
            var category = await _unitOfWork.DanhMucRepository.GetByIdAsync(course.IdDanhMuc.Value);
            var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);

            return new KhoaHocDto
            {
                Id = course.Id,
                TenKhoaHoc = course.TenKhoaHoc,
                MoTaNgan = course.MoTaNgan,
                GiaBan = course.GiaBan,
                HinhDaiDien = course.HinhDaiDien,
                MucDo = course.MucDo,
                TrangThai = course.TrangThai,
                SoLuongHocVien = course.SoLuongHocVien,
                DiemDanhGia = course.DiemDanhGia,
                SoLuongDanhGia = course.SoLuongDanhGia,
                IdDanhMuc = course.IdDanhMuc,
                TenDanhMuc = category?.TenDanhMuc,
                IdGiangVien = course.IdGiangVien,
                TenGiangVien = instructor?.HoTen,
                AnhDaiDienGiangVien = instructor?.AnhDaiDien
            };
        }
    }
}