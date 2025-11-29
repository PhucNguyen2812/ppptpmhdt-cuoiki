using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("TienDoHocTapChiTiet")]
[Index("IdTienDoHocTap", "IdBaiGiang", Name = "UQ_TienDoHocTapChiTiet", IsUnique = true)]
public partial class TienDoHocTapChiTiet
{
    [Key]
    public int Id { get; set; }

    public int IdTienDoHocTap { get; set; }

    public int IdBaiGiang { get; set; }

    public bool? DaHoanThanh { get; set; }

    [ForeignKey("IdBaiGiang")]
    [InverseProperty("TienDoHocTapChiTiets")]
    public virtual BaiGiang IdBaiGiangNavigation { get; set; } = null!;

    [ForeignKey("IdTienDoHocTap")]
    [InverseProperty("TienDoHocTapChiTiets")]
    public virtual TienDoHocTap IdTienDoHocTapNavigation { get; set; } = null!;
}
