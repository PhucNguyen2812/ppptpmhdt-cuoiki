using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    public class KhoaHocController : ControllerBase
    {
        private readonly ILogger<KhoaHocController> _logger;
        private readonly IKhoaHocService _khoaHocService;

        public KhoaHocController(
            ILogger<KhoaHocController> logger,
            IKhoaHocService khoaHocService)
        {
            _logger = logger;
            _khoaHocService = khoaHocService;
        }

        /// <summary>
        /// [PUBLIC] Lấy danh sách khóa học với filter, search, sort, pagination
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Danh sách khóa học</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] KhoaHocFilterDto filter)
        {
            _logger.LogInformation("Getting all courses with filters");
            var result = await _khoaHocService.GetPagedAsync(filter);
            return Ok(ApiResponse<PagedResult<KhoaHocDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// [PUBLIC] Lấy chi tiết khóa học
        /// </summary>
        /// <param name="id">ID khóa học</param>
        /// <returns>Chi tiết khóa học</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Getting course detail: {id}");
            var result = await _khoaHocService.GetByIdAsync(id);
            return Ok(ApiResponse<KhoaHocDetailDto>.SuccessResponse(result));
        }

        /// <summary>
        /// [PUBLIC] Lấy nội dung khóa học (curriculum)
        /// </summary>
        /// <param name="id">ID khóa học</param>
        /// <returns>Danh sách chương và bài giảng</returns>
        [HttpGet("{id}/curriculum")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurriculum(int id)
        {
            _logger.LogInformation($"Getting curriculum for course: {id}");
            var result = await _khoaHocService.GetCurriculumAsync(id);
            return Ok(ApiResponse<CurriculumDto>.SuccessResponse(result));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học nổi bật
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học nổi bật</returns>
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeatured([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting featured courses, take: {take}");
            var result = await _khoaHocService.GetFeaturedCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result, 
                $"Lấy {result.Count} khóa học nổi bật thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học bán chạy nhất
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học bán chạy</returns>
        [HttpGet("best-selling")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBestSelling([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting best-selling courses, take: {take}");
            var result = await _khoaHocService.GetBestSellingCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result,
                $"Lấy {result.Count} khóa học bán chạy thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học mới nhất
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học mới nhất</returns>
        [HttpGet("newest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewest([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting newest courses, take: {take}");
            var result = await _khoaHocService.GetNewestCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result,
                $"Lấy {result.Count} khóa học mới nhất thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học của giảng viên
        /// </summary>
        /// <param name="instructorId">ID giảng viên</param>
        /// <param name="pageNumber">Trang số (default: 1)</param>
        /// <param name="pageSize">Số items mỗi trang (default: 12)</param>
        /// <returns>Danh sách khóa học của giảng viên</returns>
        [HttpGet("instructor/{instructorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByInstructor(
            int instructorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            _logger.LogInformation($"Getting courses by instructor: {instructorId}");
            var result = await _khoaHocService.GetByInstructorAsync(instructorId, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<KhoaHocDto>>.SuccessResponse(result));
        }
    }
}