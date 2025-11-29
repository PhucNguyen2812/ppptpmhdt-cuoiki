using System;
using System.Collections.Generic;

namespace khoahoconline.Data.Entities;

public partial class TaiLieuKhoaHoc
{
    public int Id { get; set; }

    public int? IdKhoaHoc { get; set; }

    public string? TenTaiLieu { get; set; }

    public string? MoTa { get; set; }

    public string? DuongDan { get; set; }

    public string? LoaiTaiLieu { get; set; }

    public long? KichThuoc { get; set; }

    public int? ThuTu { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}
