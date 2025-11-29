using System;
using System.Collections.Generic;

namespace khoahoconline.Data.Entities;

public partial class Voucher
{
    public int Id { get; set; }

    public string? MaCode { get; set; }

    public decimal? TiLeGiam { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayHetHan { get; set; }

    public bool? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
