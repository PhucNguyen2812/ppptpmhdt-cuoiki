namespace khoahoconline.Data.Repositories
{
    public interface IUnitOfWork
    {
        INguoiDungRepository NguoiDungRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IVaiTroRepository VaiTroRepository { get; }
        IKhoaHocRepository KhoaHocRepository { get; }
        IDanhMucRepository DanhMucRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}