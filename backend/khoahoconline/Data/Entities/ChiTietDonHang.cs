using System;
using System.Collections.Generic;

namespace khoahoconline.Data.Entities;

public partial class ChiTietDonHang
{
    public int Id { get; set; }

    public int? IdDonHang { get; set; }

    public int? IdKhoaHoc { get; set; }

    public virtual DonHang? IdDonHangNavigation { get; set; }

    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}
