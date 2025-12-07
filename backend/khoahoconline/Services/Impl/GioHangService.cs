using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos.GioHang;
using khoahoconline.Middleware.Exceptions;

namespace khoahoconline.Services.Impl
{
    public class GioHangService : IGioHangService
    {
        private readonly ILogger<GioHangService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GioHangService(
            ILogger<GioHangService> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> GetCartByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Getting cart for user: {userId}");

            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);

            // Nếu chưa có giỏ hàng, tạo mới
            if (cart == null)
            {
                cart = await _unitOfWork.GioHangRepository.CreateCartForUserAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToCartDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            _logger.LogInformation($"Adding course {dto.IdKhoaHoc} to cart for user: {userId}");

            // Kiểm tra khóa học có tồn tại không
            var khoaHoc = await _unitOfWork.KhoaHocRepository.GetByIdAsync(dto.IdKhoaHoc);
            if (khoaHoc == null)
            {
                throw new NotFoundException($"Không tìm thấy khóa học với ID: {dto.IdKhoaHoc}");
            }

            // Kiểm tra khóa học có đang active không
            if (khoaHoc.TrangThai != true)
            {
                throw new BadRequestException("Khóa học này hiện không khả dụng");
            }

            // Lấy hoặc tạo giỏ hàng
            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _unitOfWork.GioHangRepository.CreateCartForUserAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            // Kiểm tra khóa học đã có trong giỏ hàng chưa
            var existingItem = await _unitOfWork.GioHangRepository
                .GetCartItemByCartAndCourseAsync(cart.Id, dto.IdKhoaHoc);

            if (existingItem != null)
            {
                throw new BadRequestException("Khóa học này đã có trong giỏ hàng");
            }

            // Thêm vào giỏ hàng
            var cartItem = new ChiTietGioHang
            {
                IdGioHang = cart.Id,
                IdKhoaHoc = dto.IdKhoaHoc
            };

            await _unitOfWork.GioHangRepository.AddItemAsync(cartItem);
            await _unitOfWork.GioHangRepository.UpdateCartTotalAsync(cart.Id);
            await _unitOfWork.SaveChangesAsync();

            // Lấy lại giỏ hàng với thông tin đầy đủ
            cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            return MapToCartDto(cart!);
        }

        public async Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId)
        {
            _logger.LogInformation($"Removing cart item {cartItemId} from cart for user: {userId}");

            // Lấy giỏ hàng của user
            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("Không tìm thấy giỏ hàng");
            }

            // Kiểm tra item có thuộc giỏ hàng của user không
            var cartItem = await _unitOfWork.GioHangRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.IdGioHang != cart.Id)
            {
                throw new NotFoundException("Không tìm thấy item trong giỏ hàng");
            }

            // Xóa item
            await _unitOfWork.GioHangRepository.RemoveItemAsync(cartItem);
            await _unitOfWork.GioHangRepository.UpdateCartTotalAsync(cart.Id);
            await _unitOfWork.SaveChangesAsync();

            // Lấy lại giỏ hàng
            cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            return MapToCartDto(cart!);
        }

        public async Task ClearCartAsync(int userId)
        {
            _logger.LogInformation($"Clearing cart for user: {userId}");

            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                return; // Không có giỏ hàng, không cần làm gì
            }

            await _unitOfWork.GioHangRepository.ClearCartAsync(cart.Id);
            await _unitOfWork.GioHangRepository.UpdateCartTotalAsync(cart.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(userId);
            return cart?.SoLuongKhoaHoc ?? 0;
        }

        private CartDto MapToCartDto(GioHang cart)
        {
            var items = cart.ChiTietGioHangs.Select(c => new CartItemDto
            {
                Id = c.Id,
                IdKhoaHoc = c.IdKhoaHoc,
                TenKhoaHoc = c.IdKhoaHocNavigation?.TenKhoaHoc,
                MoTaNgan = c.IdKhoaHocNavigation?.MoTaNgan,
                HinhDaiDien = c.IdKhoaHocNavigation?.HinhDaiDien,
                GiaBan = c.IdKhoaHocNavigation?.GiaBan,
                TenGiangVien = c.IdKhoaHocNavigation?.IdGiangVienNavigation?.HoTen,
                TenDanhMuc = c.IdKhoaHocNavigation?.IdDanhMucNavigation?.TenDanhMuc,
                DiemDanhGia = c.IdKhoaHocNavigation?.DiemDanhGia,
                SoLuongDanhGia = c.IdKhoaHocNavigation?.SoLuongDanhGia
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                IdNguoiDung = cart.IdNguoiDung,
                Items = items,
                TongTienGoc = cart.TongTienGoc,
                PhanTramGiam = cart.PhanTramGiam,
                TienGiamVoucher = cart.TienGiamVoucher,
                TongTienThanhToan = cart.TongTienThanhToan,
                SoLuongKhoaHoc = cart.SoLuongKhoaHoc,
                NgayTao = cart.NgayTao,
                NgayCapNhat = cart.NgayCapNhat
            };
        }
    }
}
















