using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;

namespace khoahoconline.Controllers;

[ApiController]
[Route("api/v1/vaitros")]
public class VaiTroController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VaiTroController> _logger;

    public VaiTroController(IUnitOfWork unitOfWork, ILogger<VaiTroController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// [ADMIN] Lấy danh sách tất cả vai trò
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            _logger.LogInformation("Getting all roles");
            var roles = await _unitOfWork.VaiTroRepository.GetAllAsync();
            _logger.LogInformation($"Found {roles.Count} roles");
            
            var roleDtos = roles.Select(r => new
            {
                id = r.Id,
                tenVaiTro = r.TenVaiTro,
                moTa = r.MoTa
            }).ToList();

            _logger.LogInformation($"Returning {roleDtos.Count} roles");
            return Ok(ApiResponse<object>.SuccessResponse(roleDtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return BadRequest(ApiResponse<string>.ErrorResponse("Lỗi khi lấy danh sách vai trò: " + ex.Message));
        }
    }
}

