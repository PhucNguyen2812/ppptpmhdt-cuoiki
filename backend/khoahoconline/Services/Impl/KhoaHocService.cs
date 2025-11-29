using AutoMapper;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Middleware.Exceptions;

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
            if (course == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với id: {id}");
            }

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
                            XemThuMienPhi = b.XemThuMienPhi,
                            // VideoUrl chỉ trả về cho bài giảng xem thử miễn phí
                            // Còn lại phải check user đã mua khóa học chưa (implement sau)
                            VideoUrl = b.XemThuMienPhi == true ? b.VideoUrl : null
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

            // Verify instructor exists
            var instructor = await _unitOfWork.NguoiDungRepository.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                throw new NotFoundException($"Không tìm thấy giảng viên với id: {instructorId}");
            }

            var pagedResult = await _unitOfWork.KhoaHocRepository.GetByInstructorAsync(instructorId, pageNumber, pageSize);

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
                TenGiangVien = instructor.HoTen,
                AnhDaiDienGiangVien = instructor.AnhDaiDien
            }).ToList();

            return new PagedResult<KhoaHocDto>
            {
                Items = dtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}