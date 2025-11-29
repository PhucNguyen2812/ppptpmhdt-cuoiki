namespace khoahoconline.Dtos.DanhMuc
{
    public class DanhMucDetailDto
    {
        public int Id { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? MoTa { get; set; }
        public bool? TrangThai { get; set; }
        public int SoKhoaHoc { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}