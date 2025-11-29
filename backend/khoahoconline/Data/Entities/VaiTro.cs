using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("VaiTro")]
[Index("TenVaiTro", Name = "UQ__VaiTro__1DA5581416BF30EE", IsUnique = true)]
public partial class VaiTro
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string TenVaiTro { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdVaiTroNavigation")]
    public virtual ICollection<NguoiDungVaiTro> NguoiDungVaiTros { get; set; } = new List<NguoiDungVaiTro>();
}
