namespace khoahoconline.Helpers;

/// <summary>
/// Constants cho các giá trị TrangThaiKiemDuyet hợp lệ
/// Phải khớp với CHECK constraint trong database: CHK_TrangThaiKiemDuyet
/// </summary>
public static class KiemDuyetConstants
{
    /// <summary>
    /// Trạng thái: Chờ kiểm duyệt
    /// </summary>
    public const string ChoKiemDuyet = "ChoKiemDuyet";

    /// <summary>
    /// Trạng thái: Đã duyệt
    /// </summary>
    public const string DaDuyet = "DaDuyet";

    /// <summary>
    /// Trạng thái: Từ chối
    /// </summary>
    public const string TuChoi = "TuChoi";

    /// <summary>
    /// Trạng thái: Bị ẩn
    /// </summary>
    public const string BiAn = "BiAn";

    /// <summary>
    /// Kiểm tra xem giá trị có hợp lệ không
    /// </summary>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value == ChoKiemDuyet || 
               value == DaDuyet || 
               value == TuChoi ||
               value == BiAn;
    }
}





