namespace khoahoconline.Data.Repositories
{
    public interface IUnitOfWork
    {
        INguoiDungRepository NguoiDungRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IVaiTroRepository VaiTroRepository { get; }
        IKhoaHocRepository KhoaHocRepository { get; }
        IDanhMucRepository DanhMucRepository { get; }
        IGioHangRepository GioHangRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IYeuCauDangKyGiangVienRepository YeuCauDangKyGiangVienRepository { get; }
        IKiemDuyetKhoaHocRepository KiemDuyetKhoaHocRepository { get; }
        IDanhGiaRepository DanhGiaRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        /// <summary>
        /// Lấy DbContext để thực hiện các thao tác xóa cứng phức tạp
        /// </summary>
        Data.CourseOnlDbContext GetDbContext();

    }
}