namespace khoahoconline.Dtos.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public string Loai { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int? IdKhoaHoc { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayDoc { get; set; }
    }
}




