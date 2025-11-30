using AutoMapper;
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

            // Kiểm tra trạng thái và duyệt cho public access
            if (course.TrangThai != true)
            {
                throw new BadRequestException("Khóa học này hiện không khả dụng");
            }

            // Kiểm tra xem khóa học đã được duyệt chưa
            var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(id);
            if (latestApproval == null || latestApproval.TrangThaiKiemDuyet != Helpers.KiemDuyetConstants.DaDuyet)
            {
                throw new BadRequestException("Khóa học này chưa được duyệt");
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
                VideoGioiThieu = course.VideoGioiThieu,
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

        public async Task<CurriculumDto> GetCurriculumAsync(int id)
        {
            _logger.LogInformation($"Getting curriculum for course: {id}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(id);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {id}");
            }

            if (course.TrangThai != true)
            {
                throw new BadRequestException("Khóa học này hiện không khả dụng");
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
                            // VideoUrl chỉ trả về cho bài giảng xem thử miễn phí
                            // Còn lại phải check user đã mua khóa học chưa (implement sau)
                            VideoUrl = b.MienPhiXem == true ? b.DuongDanVideo : null
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
                        // Lấy trạng thái kiểm duyệt mới nhất
                        var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(k.Id);
                        
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
                            AnhDaiDienGiangVien = instructor.AnhDaiDien,
                            TrangThaiKiemDuyet = latestApproval?.TrangThaiKiemDuyet
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
                VideoGioiThieu = dto.VideoGioiThieu,
                MucDo = dto.MucDo,
                TrangThai = false, // Khóa học mới tạo ở trạng thái ẩn (chờ duyệt)
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

            // Tạo bản kiểm duyệt
            var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
            {
                IdKhoaHoc = course.Id,
                PhienBan = 1,
                TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.ChoKiemDuyet,
                IdNguoiGui = instructorId,
                NgayGui = DateTime.UtcNow,
                NoiDungThayDoi = "Khóa học mới được tạo"
            };

            await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
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
                VideoGioiThieu = dto.VideoGioiThieu,
                MucDo = dto.MucDo,
                TrangThai = false, // Khóa học mới tạo ở trạng thái ẩn (chờ duyệt)
                SoLuongHocVien = 0,
                DiemDanhGia = 0,
                SoLuongDanhGia = 0,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow,
                PhienBanHienTai = 1,
                IsDeleted = false
            };

            // Create chapters and lessons before saving course
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

            await _unitOfWork.KhoaHocRepository.CreateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Tạo bản kiểm duyệt
            var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
            {
                IdKhoaHoc = course.Id,
                PhienBan = 1,
                TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.ChoKiemDuyet,
                IdNguoiGui = instructorId,
                NgayGui = DateTime.UtcNow,
                NoiDungThayDoi = "Khóa học mới được tạo"
            };

            await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
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
            course.VideoGioiThieu = dto.VideoGioiThieu;
            course.MucDo = dto.MucDo;
            course.TrangThai = false; // Ẩn khóa học khi chỉnh sửa (chờ duyệt lại)
            course.NgayCapNhat = DateTime.UtcNow;
            course.PhienBanHienTai += 1; // Tăng phiên bản

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Tạo bản kiểm duyệt mới
            var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
            {
                IdKhoaHoc = course.Id,
                PhienBan = course.PhienBanHienTai,
                TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.ChoKiemDuyet,
                IdNguoiGui = instructorId,
                NgayGui = DateTime.UtcNow,
                NoiDungThayDoi = "Khóa học được chỉnh sửa"
            };

            await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
            await _unitOfWork.SaveChangesAsync();

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
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Verify ownership
            if (course.IdGiangVien != instructorId)
            {
                throw new UnauthorizedException("Bạn không có quyền xóa khóa học này");
            }

            // Soft delete
            course.IsDeleted = true;
            course.TrangThai = false;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
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

            // Tăng phiên bản và tạo bản kiểm duyệt mới
            course.PhienBanHienTai += 1;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            // Tạo bản kiểm duyệt mới
            var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
            {
                IdKhoaHoc = course.Id,
                PhienBan = course.PhienBanHienTai,
                TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.ChoKiemDuyet,
                IdNguoiGui = instructorId,
                NgayGui = DateTime.UtcNow,
                NoiDungThayDoi = "Yêu cầu hiển thị lại khóa học"
            };

            await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<KiemDuyetKhoaHocDto>> GetPendingCoursesAsync()
        {
            _logger.LogInformation("Getting pending courses for approval");

            var pendingApprovals = await _unitOfWork.KiemDuyetKhoaHocRepository.GetPendingApprovalsAsync();

            return pendingApprovals.Select(kd => new KiemDuyetKhoaHocDto
            {
                Id = kd.Id,
                IdKhoaHoc = kd.IdKhoaHoc,
                TenKhoaHoc = kd.IdKhoaHocNavigation?.TenKhoaHoc ?? "N/A",
                PhienBan = kd.PhienBan,
                TrangThaiKiemDuyet = kd.TrangThaiKiemDuyet,
                IdNguoiGui = kd.IdNguoiGui,
                TenNguoiGui = kd.IdNguoiGuiNavigation?.HoTen ?? "N/A",
                IdNguoiKiemDuyet = kd.IdNguoiKiemDuyet,
                TenNguoiKiemDuyet = kd.IdNguoiKiemDuyetNavigation?.HoTen,
                LyDoTuChoi = kd.LyDoTuChoi,
                NgayGui = kd.NgayGui,
                NgayKiemDuyet = kd.NgayKiemDuyet,
                NoiDungThayDoi = kd.NoiDungThayDoi,
                GhiChu = kd.GhiChu
            }).ToList();
        }

        public async Task<List<KiemDuyetKhoaHocDto>> GetAllCourseApprovalsAsync(string? status = null)
        {
            _logger.LogInformation($"Getting all course approvals with status filter: {status ?? "all"}");

            var allApprovals = await _unitOfWork.KiemDuyetKhoaHocRepository.GetAllApprovalsAsync(status);

            return allApprovals.Select(kd => new KiemDuyetKhoaHocDto
            {
                Id = kd.Id,
                IdKhoaHoc = kd.IdKhoaHoc,
                TenKhoaHoc = kd.IdKhoaHocNavigation?.TenKhoaHoc ?? "N/A",
                PhienBan = kd.PhienBan,
                TrangThaiKiemDuyet = kd.TrangThaiKiemDuyet,
                IdNguoiGui = kd.IdNguoiGui,
                TenNguoiGui = kd.IdNguoiGuiNavigation?.HoTen ?? "N/A",
                IdNguoiKiemDuyet = kd.IdNguoiKiemDuyet,
                TenNguoiKiemDuyet = kd.IdNguoiKiemDuyetNavigation?.HoTen,
                LyDoTuChoi = kd.LyDoTuChoi,
                NgayGui = kd.NgayGui,
                NgayKiemDuyet = kd.NgayKiemDuyet,
                NoiDungThayDoi = kd.NoiDungThayDoi,
                GhiChu = kd.GhiChu
            }).ToList();
        }

        public async Task<bool> ApproveCourseAsync(int courseId, ApproveCourseDto dto, int reviewerId)
        {
            _logger.LogInformation($"Approving course {courseId} by reviewer: {reviewerId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Get latest pending approval
            var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(courseId);
            if (latestApproval == null || latestApproval.TrangThaiKiemDuyet != Helpers.KiemDuyetConstants.ChoKiemDuyet)
            {
                throw new BadRequestException("Khóa học này không có bản kiểm duyệt đang chờ duyệt");
            }

            // Update approval record
            latestApproval.TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.DaDuyet;
            latestApproval.IdNguoiKiemDuyet = reviewerId;
            latestApproval.NgayKiemDuyet = DateTime.UtcNow;
            latestApproval.GhiChu = dto.GhiChu;

            await _unitOfWork.KiemDuyetKhoaHocRepository.UpdateAsync(latestApproval);
            
            // Show course on public
            course.TrangThai = true;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectCourseAsync(int courseId, RejectCourseDto dto, int reviewerId)
        {
            _logger.LogInformation($"Rejecting course {courseId} by reviewer: {reviewerId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
            }

            // Get latest pending approval
            var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(courseId);
            if (latestApproval == null || latestApproval.TrangThaiKiemDuyet != Helpers.KiemDuyetConstants.ChoKiemDuyet)
            {
                throw new BadRequestException("Khóa học này không có bản kiểm duyệt đang chờ duyệt");
            }

            // Update approval record
            latestApproval.TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.TuChoi;
            latestApproval.IdNguoiKiemDuyet = reviewerId;
            latestApproval.NgayKiemDuyet = DateTime.UtcNow;
            latestApproval.LyDoTuChoi = dto.LyDoTuChoi;
            latestApproval.GhiChu = dto.GhiChu;

            await _unitOfWork.KiemDuyetKhoaHocRepository.UpdateAsync(latestApproval);
            
            // Keep course hidden
            course.TrangThai = false;
            course.NgayCapNhat = DateTime.UtcNow;

            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HideCourseByAdminAsync(int courseId, ApproveCourseDto dto, int reviewerId)
        {
            _logger.LogInformation($"Hiding course {courseId} by admin/reviewer: {reviewerId}");

            try
            {
                var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
                }

                if (!course.IdGiangVien.HasValue)
                {
                    throw new BadRequestException("Khóa học không có giảng viên");
                }

                // Get latest approval
                var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(courseId);
                
                // Tăng phiên bản nếu cần
                int newVersion = latestApproval != null ? latestApproval.PhienBan + 1 : 1;
                course.PhienBanHienTai = newVersion;
                course.NgayCapNhat = DateTime.UtcNow;

                await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Tạo bản kiểm duyệt mới với status BiAn
                var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
                {
                    IdKhoaHoc = course.Id,
                    PhienBan = newVersion,
                    TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.BiAn,
                    IdNguoiGui = course.IdGiangVien.Value,
                    IdNguoiKiemDuyet = reviewerId,
                    NgayGui = DateTime.UtcNow,
                    NgayKiemDuyet = DateTime.UtcNow,
                    NoiDungThayDoi = "Khóa học bị ẩn bởi quản trị viên",
                    GhiChu = dto.GhiChu
                };

                _logger.LogInformation($"Creating approval record with status: {kiemDuyet.TrangThaiKiemDuyet}, CourseId: {courseId}, Version: {newVersion}");

                await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
                
                // Ẩn khóa học khỏi public
                course.TrangThai = false;
                await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
                
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Successfully hid course {courseId}");
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error while hiding course {courseId}");
                
                // Kiểm tra nếu là lỗi constraint
                if (dbEx.InnerException != null)
                {
                    var innerEx = dbEx.InnerException;
                    _logger.LogError($"Inner exception: {innerEx.Message}");
                    
                    if (innerEx.Message.Contains("CHK_TrangThaiKiemDuyet") || 
                        innerEx.Message.Contains("CHECK constraint"))
                    {
                        throw new BadRequestException(
                            "Lỗi: Database constraint không cho phép giá trị 'BiAn'. " +
                            "Vui lòng chạy script SQL để cập nhật constraint CHK_TrangThaiKiemDuyet. " +
                            "Xem file update_constraint_biAn.sql trong thư mục gốc của dự án."
                        );
                    }
                }
                
                throw new BadRequestException($"Lỗi khi lưu dữ liệu: {dbEx.Message}. " +
                    $"Chi tiết: {dbEx.InnerException?.Message ?? "Không có thông tin chi tiết"}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while hiding course {courseId}");
                throw;
            }
        }

        public async Task<bool> UnhideCourseByAdminAsync(int courseId, ApproveCourseDto dto, int reviewerId)
        {
            _logger.LogInformation($"Unhiding course {courseId} by admin/reviewer: {reviewerId}");

            try
            {
                var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
                }

                if (!course.IdGiangVien.HasValue)
                {
                    throw new BadRequestException("Khóa học không có giảng viên");
                }

                // Get latest approval - phải là BiAn
                var latestApproval = await _unitOfWork.KiemDuyetKhoaHocRepository.GetLatestByCourseIdAsync(courseId);
                
                if (latestApproval == null || latestApproval.TrangThaiKiemDuyet != Helpers.KiemDuyetConstants.BiAn)
                {
                    throw new BadRequestException("Khóa học này không ở trạng thái bị ẩn");
                }
                
                // Tăng phiên bản
                int newVersion = latestApproval.PhienBan + 1;
                course.PhienBanHienTai = newVersion;
                course.NgayCapNhat = DateTime.UtcNow;

                await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Tạo bản kiểm duyệt mới với status DaDuyet (bỏ ẩn = duyệt lại)
                var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
                {
                    IdKhoaHoc = course.Id,
                    PhienBan = newVersion,
                    TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.DaDuyet,
                    IdNguoiGui = course.IdGiangVien.Value,
                    IdNguoiKiemDuyet = reviewerId,
                    NgayGui = DateTime.UtcNow,
                    NgayKiemDuyet = DateTime.UtcNow,
                    NoiDungThayDoi = "Khóa học được bỏ ẩn bởi quản trị viên",
                    GhiChu = dto.GhiChu
                };

                _logger.LogInformation($"Creating approval record with status: {kiemDuyet.TrangThaiKiemDuyet}, CourseId: {courseId}, Version: {newVersion}");

                await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
                
                // Hiển thị lại khóa học
                course.TrangThai = true;
                await _unitOfWork.KhoaHocRepository.UpdateAsync(course);
                
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Successfully unhid course {courseId}");
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error while unhiding course {courseId}");
                
                if (dbEx.InnerException != null)
                {
                    var innerEx = dbEx.InnerException;
                    _logger.LogError($"Inner exception: {innerEx.Message}");
                }
                
                throw new BadRequestException($"Lỗi khi lưu dữ liệu: {dbEx.Message}. " +
                    $"Chi tiết: {dbEx.InnerException?.Message ?? "Không có thông tin chi tiết"}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while unhiding course {courseId}");
                throw;
            }
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
                VideoGioiThieu = course.VideoGioiThieu,
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
        /// Lấy khóa học với curriculum cho admin/reviewer để xem (không cần ownership)
        /// </summary>
        public async Task<CreateCourseWithCurriculumDto> GetCourseForReviewAsync(int courseId)
        {
            _logger.LogInformation($"Getting course for review: {courseId}");

            var course = await _unitOfWork.KhoaHocRepository.GetByIdWithCurriculumAsync(courseId);
            if (course == null || course.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {courseId}");
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
                VideoGioiThieu = course.VideoGioiThieu,
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
            course.VideoGioiThieu = dto.VideoGioiThieu;
            course.MucDo = dto.MucDo;
            course.TrangThai = false; // Ẩn khóa học khi chỉnh sửa (chờ duyệt lại)
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

            // Tạo bản kiểm duyệt mới
            var kiemDuyet = new Data.Entities.KiemDuyetKhoaHoc
            {
                IdKhoaHoc = course.Id,
                PhienBan = course.PhienBanHienTai,
                TrangThaiKiemDuyet = Helpers.KiemDuyetConstants.ChoKiemDuyet,
                IdNguoiGui = instructorId,
                NgayGui = DateTime.UtcNow,
                NoiDungThayDoi = "Khóa học được chỉnh sửa"
            };

            await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(kiemDuyet);
            await _unitOfWork.SaveChangesAsync();

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