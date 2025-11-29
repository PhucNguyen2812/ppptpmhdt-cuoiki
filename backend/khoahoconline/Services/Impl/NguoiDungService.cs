using AutoMapper;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Helpers;
using khoahoconline.Middleware.Exceptions;

namespace khoahoconline.Services.Impl
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly ILogger<NguoiDungService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public NguoiDungService(IUnitOfWork unitOfWork, ILogger<NguoiDungService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<NguoiDungDto> createAsync(CreateNguoiDungDto dto)
        {
            _logger.LogInformation("Create new user");
            var entity = _mapper.Map<NguoiDung>(dto);
            var role = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync("USER");

            entity.NgayTao = DateTime.Now;
            entity.TrangThai = true;
            entity.MatKhau = PasswordHelper.HashPassword(dto.MatKhau!);

            await _unitOfWork.NguoiDungRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Create relationship in NguoiDungVaiTro table
            var nguoiDungVaiTro = new NguoiDungVaiTro
            {
                IdNguoiDung = entity.Id,
                IdVaiTro = role!.Id
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            var result = _mapper.Map<NguoiDungDto>(entity);
            return result;
        }

        public async Task<PagedResult<NguoiDungDto>> GetAllAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null)
        {
            var pagedListNguoiDungDtos = await _unitOfWork.NguoiDungRepository.GetPagedListAsync(pageNumber, pageSize, active, searchTerm);

            var NguoiDungDtosItems = pagedListNguoiDungDtos.Items.
                Select(nguoiDung => _mapper.Map<NguoiDungDto>(nguoiDung)).ToList();

            return new PagedResult<NguoiDungDto>
            {
                Items = NguoiDungDtosItems,
                TotalCount = pagedListNguoiDungDtos.TotalCount,
                PageNumber = pagedListNguoiDungDtos.PageNumber,
                PageSize = pagedListNguoiDungDtos.PageSize,
            };
        }

        public async Task<NguoiDungDto?> getByIdAsync(int id)
        {
            _logger.LogInformation($"Get user by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }
            var result = _mapper.Map<NguoiDungDto>(entity);
            return result;
        }


        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft delete by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }
            entity.TrangThai = false;
            await _unitOfWork.NguoiDungRepository.SoftDelete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task updateAsync(int id, UpdateNguoiDungDto dto)
        {
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }
            _mapper.Map(dto, nguoiDung);
            nguoiDung.NgayCapNhat = DateTime.Now;
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<NguoiDungDetailDto?> GetDetailByIdAsync(int id)
        {
            _logger.LogInformation($"Get user detail by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdWithRolesAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }

            var result = _mapper.Map<NguoiDungDetailDto>(entity);
            
            // Lấy danh sách vai trò
            result.VaiTros = await _unitOfWork.NguoiDungRepository.GetUserRolesAsync(id);
            
            return result;
        }

        public async Task RestoreAsync(int id)
        {
            _logger.LogInformation($"Restore user by id: {id}");
            var entity = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }
            entity.TrangThai = true;
            entity.NgayCapNhat = DateTime.Now;
            await _unitOfWork.NguoiDungRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> RegisterAsInstructorAsync(int userId)
        {
            _logger.LogInformation($"User {userId} registering as instructor");
            
            var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            if (user.TrangThai == false)
            {
                throw new BadRequestException("Tài khoản đã bị vô hiệu hóa.");
            }

            // Kiểm tra xem đã có role INSTRUCTOR chưa
            var hasInstructorRole = await _unitOfWork.NguoiDungRepository.HasRoleAsync(userId, "INSTRUCTOR");
            if (hasInstructorRole)
            {
                throw new BadRequestException("Người dùng đã là giảng viên.");
            }

            // Lấy role INSTRUCTOR
            var instructorRole = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync("INSTRUCTOR");
            if (instructorRole == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò giảng viên trong hệ thống.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Thêm role INSTRUCTOR cho user
                await _unitOfWork.NguoiDungRepository.AddRoleToUserAsync(userId, instructorRole.Id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                _logger.LogInformation($"User {userId} successfully registered as instructor");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error registering user {userId} as instructor");
                throw;
            }
        }

        public async Task UpdateProfileAsync(int id, UpdateProfileDto dto)
        {
            _logger.LogInformation($"Updating profile for user {id}");
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }

            _mapper.Map(dto, nguoiDung);
            nguoiDung.NgayCapNhat = DateTime.Now;
            
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            _logger.LogInformation($"Changing password for user {id}");
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }

            // Verify current password
            if (!PasswordHelper.VerifyPassword(dto.MatKhauHienTai, nguoiDung.MatKhau))
            {
                throw new BadRequestException("Mật khẩu hiện tại không đúng.");
            }

            // Update to new password
            nguoiDung.MatKhau = PasswordHelper.HashPassword(dto.MatKhauMoi);
            nguoiDung.NgayCapNhat = DateTime.Now;
            
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> UploadAvatarAsync(int id, IFormFile file)
        {
            _logger.LogInformation($"Uploading avatar for user {id}");
            var nguoiDung = await _unitOfWork.NguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với id: {id}");
            }

            // Validate file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                throw new BadRequestException("Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif)");
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                throw new BadRequestException("Kích thước file không được vượt quá 5MB");
            }

            // Generate unique filename
            var fileName = $"avatar_{id}_{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine("wwwroot", "uploads", "avatars");
            
            // Create directory if not exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user avatar path
            var avatarUrl = $"/uploads/avatars/{fileName}";
            nguoiDung.AnhDaiDien = avatarUrl;
            nguoiDung.NgayCapNhat = DateTime.Now;
            
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            await _unitOfWork.SaveChangesAsync();

            return avatarUrl;
        }

    }
}