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

            entity.NgayTao = DateTime.Now;
            entity.TrangThai = true;
            entity.MatKhau = PasswordHelper.HashPassword(dto.MatKhau!);

            await _unitOfWork.NguoiDungRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Process roles
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await ProcessUserRolesAsync(entity.Id, dto.Roles, isCreate: true);
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
            try
            {
                var pagedListNguoiDungDtos = await _unitOfWork.NguoiDungRepository.GetPagedListAsync(pageNumber, pageSize, active, searchTerm);

                var NguoiDungDtosItems = pagedListNguoiDungDtos.Items.Select(async nguoiDung =>
                {
                    try
                    {
                        var dto = _mapper.Map<NguoiDungDto>(nguoiDung);
                        // Load roles for each user
                        dto.VaiTros = await _unitOfWork.NguoiDungRepository.GetUserRolesAsync(nguoiDung.Id);
                        return dto;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error loading user {nguoiDung.Id} with roles");
                        // Return user without roles if there's an error loading roles
                        var dto = _mapper.Map<NguoiDungDto>(nguoiDung);
                        dto.VaiTros = new List<string>();
                        return dto;
                    }
                }).ToList();

                var items = await Task.WhenAll(NguoiDungDtosItems);

                return new PagedResult<NguoiDungDto>
                {
                    Items = items.ToList(),
                    TotalCount = pagedListNguoiDungDtos.TotalCount,
                    PageNumber = pagedListNguoiDungDtos.PageNumber,
                    PageSize = pagedListNguoiDungDtos.PageSize,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAllAsync: pageNumber={pageNumber}, pageSize={pageSize}, active={active}, searchTerm={searchTerm}");
                throw;
            }
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
            
            // Store current password before mapping
            var currentPassword = nguoiDung.MatKhau;
            
            _mapper.Map(dto, nguoiDung);
            
            // Only update password if a new one is provided (not empty)
            if (string.IsNullOrWhiteSpace(dto.MatKhau))
            {
                // Keep the current password if no new password is provided
                nguoiDung.MatKhau = currentPassword;
            }
            else
            {
                // Hash the new password
                nguoiDung.MatKhau = PasswordHelper.HashPassword(dto.MatKhau);
            }
            
            nguoiDung.NgayCapNhat = DateTime.Now;
            await _unitOfWork.NguoiDungRepository.UpdateAsync(nguoiDung);
            
            // Process roles if provided
            if (dto.Roles != null)
            {
                await ProcessUserRolesAsync(id, dto.Roles, isCreate: false);
            }
            
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Process user roles: add/remove roles based on provided list
        /// Rules:
        /// - Cannot add ADMIN role
        /// - If GIANGVIEN is added, automatically add HOCVIEN
        /// </summary>
        private async Task ProcessUserRolesAsync(int userId, List<string>? roles, bool isCreate = false)
        {
            if (roles == null || roles.Count == 0)
            {
                // If no roles provided and creating, default to HOCVIEN
                if (isCreate)
                {
                    var hocVienRole = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync("HOCVIEN");
                    if (hocVienRole != null)
                    {
                        await _unitOfWork.NguoiDungRepository.AddRoleToUserAsync(userId, hocVienRole.Id);
                    }
                }
                return;
            }

            // Remove ADMIN from list if present (not allowed)
            roles = roles.Where(r => !string.Equals(r, "ADMIN", StringComparison.OrdinalIgnoreCase)).ToList();

            // If GIANGVIEN is in the list, ensure HOCVIEN is also included
            bool hasGiangVien = roles.Any(r => string.Equals(r, "GIANGVIEN", StringComparison.OrdinalIgnoreCase));
            bool hasHocVien = roles.Any(r => string.Equals(r, "HOCVIEN", StringComparison.OrdinalIgnoreCase));
            
            if (hasGiangVien && !hasHocVien)
            {
                roles.Add("HOCVIEN");
            }

            // Get current user roles
            var currentRoles = await _unitOfWork.NguoiDungRepository.GetUserRolesAsync(userId);
            
            // Get all available roles from database
            var allRolesList = await _unitOfWork.VaiTroRepository.GetAllAsync();
            var roleDict = allRolesList.ToDictionary(r => r.TenVaiTro, r => r.Id, StringComparer.OrdinalIgnoreCase);

            // Remove all current roles first (for update) or keep existing (for create)
            if (!isCreate)
            {
                // Remove all roles that are not in the new list
                foreach (var currentRole in currentRoles)
                {
                    if (!roles.Any(r => string.Equals(r, currentRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (roleDict.TryGetValue(currentRole, out int roleId))
                        {
                            await _unitOfWork.NguoiDungRepository.RemoveRoleFromUserAsync(userId, roleId);
                        }
                    }
                }
            }

            // Add new roles
            foreach (var roleName in roles)
            {
                if (roleDict.TryGetValue(roleName, out int roleId))
                {
                    await _unitOfWork.NguoiDungRepository.AddRoleToUserAsync(userId, roleId);
                }
                else
                {
                    _logger.LogWarning($"Role '{roleName}' not found in system");
                }
            }
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

            // Kiểm tra xem đã có role GIANGVIEN chưa
            var hasInstructorRole = await _unitOfWork.NguoiDungRepository.HasRoleAsync(userId, "GIANGVIEN");
            if (hasInstructorRole)
            {
                throw new BadRequestException("Người dùng đã là giảng viên.");
            }

            // Lấy role GIANGVIEN
            var instructorRole = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync("GIANGVIEN");
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