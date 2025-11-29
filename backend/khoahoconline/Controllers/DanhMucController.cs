using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class DanhMucController : ControllerBase
    {
        private readonly ILogger<DanhMucController> _logger;
        private readonly IDanhMucService _danhMucService;

        public DanhMucController(
            ILogger<DanhMucController> logger,
            IDanhMucService danhMucService)
        {
            _logger = logger;
            _danhMucService = danhMucService;
        }

        /// <summary>
        /// [PUBLIC] Lấy tất cả danh mục đang hoạt động
        /// </summary>
        /// <returns>Danh sách danh mục kèm số lượng khóa học</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllActive()
        {
            _logger.LogInformation("Getting all active categories");
            var result = ApiResponse<List<DanhMucDetailDto>>.SuccessResponse(
                await _danhMucService.GetAllActiveAsync());
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Lấy tất cả danh mục (bao gồm cả inactive)
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm (optional)</param>
        /// <returns>Danh sách tất cả danh mục</returns>
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            _logger.LogInformation("Getting all categories (including inactive)");
            
            List<DanhMucDetailDto> categories;
            if (!string.IsNullOrWhiteSpace(search))
            {
                categories = await _danhMucService.SearchAsync(search);
            }
            else
            {
                categories = await _danhMucService.GetAllAsync();
            }

            var result = ApiResponse<List<DanhMucDetailDto>>.SuccessResponse(categories);
            return Ok(result);
        }

        /// <summary>
        /// [PUBLIC] Lấy chi tiết danh mục theo ID
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <returns>Chi tiết danh mục kèm số lượng khóa học</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Getting category by id: {id}");
            var result = ApiResponse<DanhMucDetailDto>.SuccessResponse(
                await _danhMucService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// [PUBLIC] Đếm số lượng khóa học trong danh mục
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <returns>Số lượng khóa học</returns>
        [HttpGet("{id}/courses/count")]
        [AllowAnonymous]
        public async Task<IActionResult> CountCourses(int id)
        {
            _logger.LogInformation($"Counting courses in category: {id}");
            var count = await _danhMucService.CountCoursesAsync(id);
            var result = ApiResponse<int>.SuccessResponse(count, $"Danh mục có {count} khóa học");
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Tạo danh mục mới
        /// </summary>
        /// <param name="dto">Thông tin danh mục mới</param>
        /// <returns>Danh mục đã tạo</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreateDanhMucDto dto)
        {
            _logger.LogInformation($"Creating new category: {dto.TenDanhMuc}");
            var created = await _danhMucService.CreateAsync(dto);
            var result = ApiResponse<DanhMucDto>.SuccessResponse(created, "Tạo danh mục thành công");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        /// <summary>
        /// [ADMIN] Cập nhật danh mục
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDanhMucDto dto)
        {
            _logger.LogInformation($"Updating category: {id}");
            await _danhMucService.UpdateAsync(id, dto);
            var result = ApiResponse<string>.SuccessResponse("Cập nhật danh mục thành công");
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Xóa mềm danh mục (chỉ được xóa nếu không còn khóa học)
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            _logger.LogInformation($"Soft deleting category: {id}");
            await _danhMucService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Xóa danh mục thành công");
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Khôi phục danh mục đã xóa
        /// </summary>
        /// <param name="id">ID danh mục</param>
        /// <returns>Success message</returns>
        [HttpPut("{id}/restore")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Restore(int id)
        {
            _logger.LogInformation($"Restoring category: {id}");
            await _danhMucService.RestoreAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Khôi phục danh mục thành công");
            return Ok(result);
        }
    }
}