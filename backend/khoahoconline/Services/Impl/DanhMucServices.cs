using AutoMapper;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Middleware.Exceptions;

namespace khoahoconline.Services.Impl
{
    public class DanhMucService : IDanhMucService
    {
        private readonly ILogger<DanhMucService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DanhMucService(
            ILogger<DanhMucService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DanhMucDetailDto>> GetAllActiveAsync()
        {
            _logger.LogInformation("Getting all active categories");
            
            var categories = await _unitOfWork.DanhMucRepository.GetAllActiveAsync();
            var result = new List<DanhMucDetailDto>();

            foreach (var category in categories)
            {
                var dto = _mapper.Map<DanhMucDetailDto>(category);
                dto.SoKhoaHoc = await _unitOfWork.DanhMucRepository.CountCoursesAsync(category.Id);
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<DanhMucDetailDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all categories (including inactive)");
            
            var categories = await _unitOfWork.DanhMucRepository.GetAllAsync();
            var result = new List<DanhMucDetailDto>();

            foreach (var category in categories)
            {
                var dto = _mapper.Map<DanhMucDetailDto>(category);
                dto.SoKhoaHoc = await _unitOfWork.DanhMucRepository.CountCoursesAsync(category.Id);
                result.Add(dto);
            }

            return result;
        }

        public async Task<DanhMucDetailDto> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting category by id: {id}");
            
            var category = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            var dto = _mapper.Map<DanhMucDetailDto>(category);
            dto.SoKhoaHoc = await _unitOfWork.DanhMucRepository.CountCoursesAsync(category.Id);

            return dto;
        }

        public async Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto)
        {
            _logger.LogInformation($"Creating new category: {dto.TenDanhMuc}");

            // Kiểm tra tên danh mục đã tồn tại chưa
            var exists = await _unitOfWork.DanhMucRepository.IsTenDanhMucExistsAsync(dto.TenDanhMuc);
            if (exists)
            {
                throw new BadRequestException($"Tên danh mục '{dto.TenDanhMuc}' đã tồn tại");
            }

            var entity = _mapper.Map<DanhMucKhoaHoc>(dto);
            entity.TrangThai = true;
            entity.NgayTao = DateTime.Now;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.DanhMucRepository.CreateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Category created successfully with id: {entity.Id}");
                return _mapper.Map<DanhMucDto>(entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating category");
                throw;
            }
        }

        public async Task UpdateAsync(int id, UpdateDanhMucDto dto)
        {
            _logger.LogInformation($"Updating category with id: {id}");

            var entity = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            // Kiểm tra tên danh mục mới có trùng với danh mục khác không
            var exists = await _unitOfWork.DanhMucRepository.IsTenDanhMucExistsAsync(dto.TenDanhMuc, id);
            if (exists)
            {
                throw new BadRequestException($"Tên danh mục '{dto.TenDanhMuc}' đã tồn tại");
            }

            _mapper.Map(dto, entity);
            entity.NgayCapNhat = DateTime.Now;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.DanhMucRepository.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Category updated successfully: {id}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error updating category: {id}");
                throw;
            }
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation($"Soft deleting category with id: {id}");

            var entity = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            // Kiểm tra xem danh mục còn khóa học không
            var courseCount = await _unitOfWork.DanhMucRepository.CountCoursesAsync(id);
            if (courseCount > 0)
            {
                throw new BadRequestException(
                    $"Không thể xóa danh mục này vì còn {courseCount} khóa học. " +
                    $"Vui lòng xóa hoặc chuyển các khóa học sang danh mục khác trước.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.DanhMucRepository.SoftDeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Category soft deleted successfully: {id}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error soft deleting category: {id}");
                throw;
            }
        }

        public async Task RestoreAsync(int id)
        {
            _logger.LogInformation($"Restoring category with id: {id}");

            var entity = await _unitOfWork.DanhMucRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {id}");
            }

            if (entity.TrangThai == true)
            {
                throw new BadRequestException("Danh mục này chưa bị vô hiệu hóa");
            }

            entity.TrangThai = true;
            entity.NgayCapNhat = DateTime.Now;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.DanhMucRepository.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Category restored successfully: {id}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error restoring category: {id}");
                throw;
            }
        }

        public async Task<List<DanhMucDetailDto>> SearchAsync(string searchTerm)
        {
            _logger.LogInformation($"Searching categories with term: {searchTerm}");

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllActiveAsync();
            }

            var categories = await _unitOfWork.DanhMucRepository.SearchAsync(searchTerm);
            var result = new List<DanhMucDetailDto>();

            foreach (var category in categories)
            {
                var dto = _mapper.Map<DanhMucDetailDto>(category);
                dto.SoKhoaHoc = await _unitOfWork.DanhMucRepository.CountCoursesAsync(category.Id);
                result.Add(dto);
            }

            return result;
        }

        public async Task<int> CountCoursesAsync(int danhMucId)
        {
            _logger.LogInformation($"Counting courses in category: {danhMucId}");
            
            var exists = await _unitOfWork.DanhMucRepository.GetByIdAsync(danhMucId);
            if (exists == null)
            {
                throw new NotFoundException($"Không tìm thấy danh mục với id: {danhMucId}");
            }

            return await _unitOfWork.DanhMucRepository.CountCoursesAsync(danhMucId);
        }
    }
}