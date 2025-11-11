using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("Voucher")]
[Index("MaCode", Name = "UQ__Voucher__152C7C5CB9B298A6", IsUnique = true)]
public partial class Voucher
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string MaCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TiLeGiam { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayHetHan { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdVoucherNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
