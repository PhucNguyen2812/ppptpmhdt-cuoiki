using System;
using System.Collections.Generic;

namespace khoahoconline.Data.Entities;

public partial class ChiaSeLuanNhuan
{
    public int Id { get; set; }

    public int? IdDonHang { get; set; }

    public int? IdKhoaHoc { get; set; }

    public int? IdGiangVien { get; set; }

    public decimal? DoanhThu { get; set; }

    public decimal? TyLeChiaGiangVien { get; set; }

    public decimal? TienGiangVien { get; set; }

    public decimal? TienHeThong { get; set; }

    public bool? TrangThaiThanhToan { get; set; }

    public virtual DonHang? IdDonHangNavigation { get; set; }

    public virtual NguoiDung? IdGiangVienNavigation { get; set; }

    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}
