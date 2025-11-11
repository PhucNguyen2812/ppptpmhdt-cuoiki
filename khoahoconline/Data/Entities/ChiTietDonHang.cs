using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("ChiTietDonHang")]
public partial class ChiTietDonHang
{
    [Key]
    public int Id { get; set; }

    public int IdDonHang { get; set; }

    public int IdKhoaHoc { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual DonHang IdDonHangNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
