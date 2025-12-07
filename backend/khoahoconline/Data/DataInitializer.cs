using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Helpers;

namespace khoahoconline.Data
{
    public class DataInitializer
    {
        public static async Task SeedData(CourseOnlDbContext context)
        {
            try
            {
                // Đảm bảo DB được tạo
                await context.Database.MigrateAsync();

                // ===== TẠO VAI TRÒ =====
                var adminRole = await EnsureRoleAsync(context, "ADMIN", "Quản trị viên hệ thống");
                var giangVienRole = await EnsureRoleAsync(context, "GIANGVIEN", "Giảng viên");
                var hocVienRole = await EnsureRoleAsync(context, "HOCVIEN", "Học viên");

                await context.SaveChangesAsync();

                // ===== TẠO DANH MỤC KHÓA HỌC =====
                var danhMuc1 = await EnsureDanhMucAsync(context, "Lập trình Web", "Các khóa học về phát triển web");
                var danhMuc2 = await EnsureDanhMucAsync(context, "Lập trình Mobile", "Các khóa học về phát triển ứng dụng di động");
                var danhMuc3 = await EnsureDanhMucAsync(context, "Thiết kế đồ họa", "Các khóa học về thiết kế và đồ họa");
                var danhMuc4 = await EnsureDanhMucAsync(context, "Marketing", "Các khóa học về marketing và quảng cáo");
                var danhMuc5 = await EnsureDanhMucAsync(context, "Kinh doanh", "Các khóa học về kinh doanh và khởi nghiệp");

                await context.SaveChangesAsync();

                // ===== TẠO ADMIN =====
                var admin = await EnsureUserAsync(context, "admin@gmail.com", "admin123", "Nguyễn Văn Admin", adminRole.Id);
                await EnsureUserRoleAsync(context, admin.Id, adminRole.Id);


                // ===== TẠO 3 TÀI KHOẢN HỌC VIÊN THUẦN TÚY =====
                var hocVien1 = await EnsureUserAsync(context, "hocvien1@gmail.com", "hocvien123", "Lê Văn Học Viên 1", hocVienRole.Id);
                await EnsureUserRoleAsync(context, hocVien1.Id, hocVienRole.Id);

                var hocVien2 = await EnsureUserAsync(context, "hocvien2@gmail.com", "hocvien123", "Phạm Thị Học Viên 2", hocVienRole.Id);
                await EnsureUserRoleAsync(context, hocVien2.Id, hocVienRole.Id);

                var hocVien3 = await EnsureUserAsync(context, "hocvien3@gmail.com", "hocvien123", "Hoàng Văn Học Viên 3", hocVienRole.Id);
                await EnsureUserRoleAsync(context, hocVien3.Id, hocVienRole.Id);

                // ===== TẠO 2 TÀI KHOẢN HỌC VIÊN + GIẢNG VIÊN =====
                var giangVien1 = await EnsureUserAsync(context, "giangvien1@gmail.com", "giangvien123", "Nguyễn Thị Giảng Viên 1", giangVienRole.Id);
                giangVien1.ChuyenMon = "Lập trình Web, JavaScript, React";
                giangVien1.MoTaNgan = "Giảng viên với 5 năm kinh nghiệm trong lĩnh vực phát triển web";
                giangVien1.TieuSu = "Tốt nghiệp Đại học Bách Khoa, có nhiều năm kinh nghiệm làm việc tại các công ty công nghệ hàng đầu";
                await EnsureUserRoleAsync(context, giangVien1.Id, hocVienRole.Id);
                await EnsureUserRoleAsync(context, giangVien1.Id, giangVienRole.Id);

                var giangVien2 = await EnsureUserAsync(context, "giangvien2@gmail.com", "giangvien123", "Trần Văn Giảng Viên 2", giangVienRole.Id);
                giangVien2.ChuyenMon = "Mobile Development, Flutter, React Native";
                giangVien2.MoTaNgan = "Chuyên gia phát triển ứng dụng di động với hơn 7 năm kinh nghiệm";
                giangVien2.TieuSu = "Từng làm việc tại các startup công nghệ, có nhiều ứng dụng được đánh giá cao trên App Store và Google Play";
                await EnsureUserRoleAsync(context, giangVien2.Id, hocVienRole.Id);
                await EnsureUserRoleAsync(context, giangVien2.Id, giangVienRole.Id);

                await context.SaveChangesAsync();

                // ===== TẠO KHÓA HỌC CHO GIẢNG VIÊN 1 =====
                var khoaHoc1 = await CreateKhoaHocAsync(context, 
                    "Lập trình Web với React từ Zero đến Hero",
                    "Khóa học toàn diện về React, từ cơ bản đến nâng cao",
                    "Bạn sẽ học được cách xây dựng ứng dụng web hiện đại với React, Hooks, Redux, và nhiều công nghệ liên quan. Khóa học bao gồm các dự án thực tế giúp bạn có portfolio ấn tượng.",
                    danhMuc1.Id, giangVien1.Id, "Trung bình", 499000);

                var khoaHoc2 = await CreateKhoaHocAsync(context,
                    "JavaScript ES6+ và Modern Web Development",
                    "Nắm vững JavaScript hiện đại và các tính năng mới nhất",
                    "Khóa học giúp bạn thành thạo JavaScript ES6+, async/await, modules, và các pattern hiện đại. Phù hợp cho người mới bắt đầu và muốn nâng cao kỹ năng.",
                    danhMuc1.Id, giangVien1.Id, "Cơ bản", 399000);

                // ===== TẠO KHÓA HỌC CHO GIẢNG VIÊN 2 =====
                var khoaHoc3 = await CreateKhoaHocAsync(context,
                    "Flutter - Xây dựng ứng dụng di động đa nền tảng",
                    "Học Flutter để tạo ứng dụng iOS và Android với một codebase duy nhất",
                    "Khóa học hướng dẫn chi tiết cách sử dụng Flutter để phát triển ứng dụng di động. Bạn sẽ học về widgets, state management, API integration và nhiều hơn nữa.",
                    danhMuc2.Id, giangVien2.Id, "Trung bình", 599000);

                var khoaHoc4 = await CreateKhoaHocAsync(context,
                    "React Native - Phát triển ứng dụng mobile với JavaScript",
                    "Tạo ứng dụng native cho iOS và Android bằng React Native",
                    "Khóa học toàn diện về React Native, từ setup môi trường đến publish ứng dụng lên App Store và Google Play. Bao gồm Redux, Navigation, và các thư viện quan trọng.",
                    danhMuc2.Id, giangVien2.Id, "Nâng cao", 699000);

                await context.SaveChangesAsync();

                // ===== TẠO CHƯƠNG VÀ BÀI GIẢNG CHO KHÓA HỌC 1 =====
                await CreateChuongVaBaiGiangAsync(context, khoaHoc1.Id, "Giới thiệu về React", new[]
                {
                    ("React là gì? Tại sao nên học React?", "Tìm hiểu về React và lý do tại sao nó trở thành framework phổ biến nhất hiện nay", 15),
                    ("Cài đặt môi trường phát triển", "Hướng dẫn cài đặt Node.js, npm và tạo dự án React đầu tiên", 20),
                    ("JSX và Components cơ bản", "Học cách viết JSX và tạo components trong React", 25)
                });

                await CreateChuongVaBaiGiangAsync(context, khoaHoc1.Id, "State và Props", new[]
                {
                    ("Hiểu về Props trong React", "Cách truyền dữ liệu giữa các components thông qua props", 18),
                    ("State và setState", "Quản lý state trong component và cách cập nhật state", 22),
                    ("Lifting State Up", "Kỹ thuật chia sẻ state giữa các components", 20)
                });

                await CreateChuongVaBaiGiangAsync(context, khoaHoc1.Id, "React Hooks", new[]
                {
                    ("useState Hook", "Sử dụng useState để quản lý state trong functional components", 25),
                    ("useEffect Hook", "Xử lý side effects với useEffect", 30),
                    ("Custom Hooks", "Tạo và sử dụng custom hooks để tái sử dụng logic", 28)
                });

                // ===== TẠO CHƯƠNG VÀ BÀI GIẢNG CHO KHÓA HỌC 2 =====
                await CreateChuongVaBaiGiangAsync(context, khoaHoc2.Id, "JavaScript Cơ bản", new[]
                {
                    ("Biến và Kiểu dữ liệu", "Tìm hiểu về let, const, var và các kiểu dữ liệu trong JavaScript", 20),
                    ("Functions và Arrow Functions", "Cách khai báo và sử dụng functions trong JavaScript", 25),
                    ("Arrays và Objects", "Làm việc với arrays và objects trong JavaScript", 30)
                });

                await CreateChuongVaBaiGiangAsync(context, khoaHoc2.Id, "ES6+ Features", new[]
                {
                    ("Destructuring", "Cách sử dụng destructuring cho arrays và objects", 22),
                    ("Spread và Rest Operators", "Sử dụng spread và rest operators", 20),
                    ("Template Literals", "Template strings và tagged templates", 15)
                });

                // ===== TẠO CHƯƠNG VÀ BÀI GIẢNG CHO KHÓA HỌC 3 =====
                await CreateChuongVaBaiGiangAsync(context, khoaHoc3.Id, "Giới thiệu Flutter", new[]
                {
                    ("Flutter là gì?", "Tìm hiểu về Flutter và Dart programming language", 18),
                    ("Cài đặt Flutter SDK", "Hướng dẫn cài đặt Flutter trên Windows, macOS và Linux", 25),
                    ("Tạo dự án Flutter đầu tiên", "Tạo và chạy ứng dụng Flutter đầu tiên", 20)
                });

                await CreateChuongVaBaiGiangAsync(context, khoaHoc3.Id, "Widgets và Layout", new[]
                {
                    ("Stateless và Stateful Widgets", "Hiểu về widgets và cách tạo widgets trong Flutter", 30),
                    ("Layout Widgets", "Sử dụng Row, Column, Container và các layout widgets", 35),
                    ("Material Design Components", "Sử dụng Material Design components trong Flutter", 28)
                });

                // ===== TẠO CHƯƠNG VÀ BÀI GIẢNG CHO KHÓA HỌC 4 =====
                await CreateChuongVaBaiGiangAsync(context, khoaHoc4.Id, "React Native Fundamentals", new[]
                {
                    ("Giới thiệu React Native", "Tìm hiểu về React Native và sự khác biệt với React", 20),
                    ("Cài đặt môi trường", "Setup React Native CLI và Expo", 30),
                    ("Components cơ bản", "Text, View, Image và các components cơ bản", 25)
                });

                await CreateChuongVaBaiGiangAsync(context, khoaHoc4.Id, "Navigation và State Management", new[]
                {
                    ("React Navigation", "Thiết lập navigation trong React Native app", 35),
                    ("Redux trong React Native", "Quản lý state với Redux", 40),
                    ("Context API", "Sử dụng Context API cho state management", 30)
                });

                await context.SaveChangesAsync();

                // ===== ĐĂNG KÝ HỌC VIÊN VÀO KHÓA HỌC =====
                // Mỗi khóa học có 3 học viên đăng ký
                await CreateDangKyKhoaHocAsync(context, hocVien1.Id, khoaHoc1.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien2.Id, khoaHoc1.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien3.Id, khoaHoc1.Id);

                await CreateDangKyKhoaHocAsync(context, hocVien1.Id, khoaHoc2.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien2.Id, khoaHoc2.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien3.Id, khoaHoc2.Id);

                await CreateDangKyKhoaHocAsync(context, hocVien1.Id, khoaHoc3.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien2.Id, khoaHoc3.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien3.Id, khoaHoc3.Id);

                await CreateDangKyKhoaHocAsync(context, hocVien1.Id, khoaHoc4.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien2.Id, khoaHoc4.Id);
                await CreateDangKyKhoaHocAsync(context, hocVien3.Id, khoaHoc4.Id);

                // Cập nhật số lượng học viên cho các khóa học
                khoaHoc1.SoLuongHocVien = 3;
                khoaHoc2.SoLuongHocVien = 3;
                khoaHoc3.SoLuongHocVien = 3;
                khoaHoc4.SoLuongHocVien = 3;

                // Thêm đánh giá mẫu
                await CreateDanhGiaAsync(context, hocVien1.Id, khoaHoc1.Id, 5, "Khóa học rất hay và chi tiết!");
                await CreateDanhGiaAsync(context, hocVien2.Id, khoaHoc1.Id, 4, "Nội dung tốt, giảng viên giải thích rõ ràng");
                await CreateDanhGiaAsync(context, hocVien3.Id, khoaHoc1.Id, 5, "Rất hữu ích cho người mới bắt đầu");

                await CreateDanhGiaAsync(context, hocVien1.Id, khoaHoc3.Id, 5, "Flutter rất mạnh mẽ, khóa học này giúp tôi hiểu rõ");
                await CreateDanhGiaAsync(context, hocVien2.Id, khoaHoc3.Id, 4, "Tốt nhưng cần thêm ví dụ thực tế");

                // Cập nhật điểm đánh giá trung bình
                await UpdateCourseRatingsAsync(context, khoaHoc1.Id);
                await UpdateCourseRatingsAsync(context, khoaHoc3.Id);

                await context.SaveChangesAsync();

                // ===== TẠO VOUCHER =====
                var voucher1 = await CreateVoucherAsync(context, "WELCOME10", 10, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30));
                var voucher2 = await CreateVoucherAsync(context, "SUMMER2024", 20, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(20));
                var voucher3 = await CreateVoucherAsync(context, "STUDENT50", 50, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5));
                var voucher4 = await CreateVoucherAsync(context, "EXPIRED", 15, DateTime.UtcNow.AddDays(-60), DateTime.UtcNow.AddDays(-30)); // Đã hết hạn

                await context.SaveChangesAsync();

                // ===== TẠO GIỎ HÀNG VÀ CHI TIẾT GIỎ HÀNG =====
                // Giỏ hàng cho học viên 1
                var gioHang1 = await CreateGioHangAsync(context, hocVien1.Id);
                await CreateChiTietGioHangAsync(context, gioHang1.Id, khoaHoc1.Id);
                await CreateChiTietGioHangAsync(context, gioHang1.Id, khoaHoc2.Id);
                await UpdateGioHangTotalAsync(context, gioHang1.Id);

                // Giỏ hàng cho học viên 2
                var gioHang2 = await CreateGioHangAsync(context, hocVien2.Id);
                await CreateChiTietGioHangAsync(context, gioHang2.Id, khoaHoc3.Id);
                await UpdateGioHangTotalAsync(context, gioHang2.Id);

                await context.SaveChangesAsync();

                // ===== TẠO ĐƠN HÀNG VÀ CHI TIẾT ĐƠN HÀNG =====
                // Đơn hàng 1: Học viên 1 mua khóa học 1 và 2 (đã thanh toán)
                var donHang1 = await CreateDonHangAsync(context, hocVien1.Id, null, 
                    new[] { khoaHoc1.Id, khoaHoc2.Id }, 
                    "Đã thanh toán", "Thành công", "Chuyển khoản", DateTime.UtcNow.AddDays(-5));
                
                // Đơn hàng 2: Học viên 2 mua khóa học 3 (đã thanh toán)
                var donHang2 = await CreateDonHangAsync(context, hocVien2.Id, voucher1.Id,
                    new[] { khoaHoc3.Id },
                    "Đã thanh toán", "Thành công", "VNPay", DateTime.UtcNow.AddDays(-3));

                // Đơn hàng 3: Học viên 3 mua khóa học 4 (chưa thanh toán)
                var donHang3 = await CreateDonHangAsync(context, hocVien3.Id, null,
                    new[] { khoaHoc4.Id },
                    "Chờ thanh toán", "Chưa thanh toán", null, null);

                await context.SaveChangesAsync();

                // Cập nhật đăng ký khóa học từ đơn hàng đã thanh toán
                await UpdateDangKyKhoaHocFromOrderAsync(context, donHang1.Id, hocVien1.Id, new[] { khoaHoc1.Id, khoaHoc2.Id });
                await UpdateDangKyKhoaHocFromOrderAsync(context, donHang2.Id, hocVien2.Id, new[] { khoaHoc3.Id });

                await context.SaveChangesAsync();

                await context.SaveChangesAsync();

                // ===== TẠO TIẾN ĐỘ HỌC TẬP =====
                // Tiến độ học tập cho học viên 1 - khóa học 1
                var tienDo1 = await CreateTienDoHocTapAsync(context, hocVien1.Id, khoaHoc1.Id);
                if (tienDo1 != null)
                {
                    var baiGiangs1 = await context.BaiGiangs
                        .Where(b => b.IdChuongNavigation!.IdKhoaHoc == khoaHoc1.Id)
                        .OrderBy(b => b.IdChuongNavigation!.ThuTu)
                        .ThenBy(b => b.ThuTu)
                        .ToListAsync();
                    
                    // Học viên 1 đã hoàn thành 5/9 bài giảng
                    for (int i = 0; i < Math.Min(5, baiGiangs1.Count); i++)
                    {
                        await CreateTienDoHocTapChiTietAsync(context, tienDo1.Id, baiGiangs1[i].Id, true);
                    }
                    await UpdateTienDoHocTapAsync(context, tienDo1.Id);
                }

                // Tiến độ học tập cho học viên 2 - khóa học 1
                var tienDo2 = await CreateTienDoHocTapAsync(context, hocVien2.Id, khoaHoc1.Id);
                if (tienDo2 != null)
                {
                    var baiGiangs1 = await context.BaiGiangs
                        .Where(b => b.IdChuongNavigation!.IdKhoaHoc == khoaHoc1.Id)
                        .OrderBy(b => b.IdChuongNavigation!.ThuTu)
                        .ThenBy(b => b.ThuTu)
                        .ToListAsync();
                    
                    // Học viên 2 đã hoàn thành 3/9 bài giảng
                    for (int i = 0; i < Math.Min(3, baiGiangs1.Count); i++)
                    {
                        await CreateTienDoHocTapChiTietAsync(context, tienDo2.Id, baiGiangs1[i].Id, true);
                    }
                    await UpdateTienDoHocTapAsync(context, tienDo2.Id);
                }

                // Tiến độ học tập cho học viên 2 - khóa học 3
                var tienDo3 = await CreateTienDoHocTapAsync(context, hocVien2.Id, khoaHoc3.Id);
                if (tienDo3 != null)
                {
                    var baiGiangs3 = await context.BaiGiangs
                        .Where(b => b.IdChuongNavigation!.IdKhoaHoc == khoaHoc3.Id)
                        .OrderBy(b => b.IdChuongNavigation!.ThuTu)
                        .ThenBy(b => b.ThuTu)
                        .ToListAsync();
                    
                    // Học viên 2 đã hoàn thành 2/6 bài giảng
                    for (int i = 0; i < Math.Min(2, baiGiangs3.Count); i++)
                    {
                        await CreateTienDoHocTapChiTietAsync(context, tienDo3.Id, baiGiangs3[i].Id, true);
                    }
                    await UpdateTienDoHocTapAsync(context, tienDo3.Id);
                }

                await context.SaveChangesAsync();

                Console.WriteLine("✓ Sample data created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                throw;
            }
        }

        // Helper methods
        private static async Task<VaiTro> EnsureRoleAsync(CourseOnlDbContext context, string tenVaiTro, string moTa)
        {
            var role = await context.VaiTros.FirstOrDefaultAsync(vt => vt.TenVaiTro == tenVaiTro);
            if (role == null)
            {
                role = new VaiTro
                {
                    TenVaiTro = tenVaiTro,
                    MoTa = moTa,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.VaiTros.AddAsync(role);
            }
            return role;
        }

        private static async Task<DanhMucKhoaHoc> EnsureDanhMucAsync(CourseOnlDbContext context, string tenDanhMuc, string moTa)
        {
            var danhMuc = await context.DanhMucKhoaHocs.FirstOrDefaultAsync(dm => dm.TenDanhMuc == tenDanhMuc);
            if (danhMuc == null)
            {
                danhMuc = new DanhMucKhoaHoc
                {
                    TenDanhMuc = tenDanhMuc,
                    MoTa = moTa,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.DanhMucKhoaHocs.AddAsync(danhMuc);
            }
            return danhMuc;
        }

        private static async Task<NguoiDung> EnsureUserAsync(CourseOnlDbContext context, string email, string password, string hoTen, int? defaultRoleId = null)
        {
            var user = await context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new NguoiDung
                {
                    Email = email,
                    MatKhau = PasswordHelper.HashPassword(password),
                    HoTen = hoTen,
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.NguoiDungs.AddAsync(user);
                await context.SaveChangesAsync();
            }
            return user;
        }

        private static async Task EnsureUserRoleAsync(CourseOnlDbContext context, int userId, int roleId)
        {
            var exists = await context.NguoiDungVaiTros
                .AnyAsync(ndvt => ndvt.IdNguoiDung == userId && ndvt.IdVaiTro == roleId);
            
            if (!exists)
            {
                await context.NguoiDungVaiTros.AddAsync(new NguoiDungVaiTro
                {
                    IdNguoiDung = userId,
                    IdVaiTro = roleId
                });
            }
        }

        private static async Task<KhoaHoc> CreateKhoaHocAsync(CourseOnlDbContext context, string tenKhoaHoc, string moTaNgan, string moTaChiTiet, int idDanhMuc, int idGiangVien, string mucDo, decimal giaBan)
        {
            var khoaHoc = new KhoaHoc
            {
                TenKhoaHoc = tenKhoaHoc,
                MoTaNgan = moTaNgan,
                MoTaChiTiet = moTaChiTiet,
                IdDanhMuc = idDanhMuc,
                IdGiangVien = idGiangVien,
                GiaBan = giaBan,
                MucDo = mucDo,
                TrangThai = true,
                SoLuongHocVien = 0,
                DiemDanhGia = null,
                SoLuongDanhGia = 0,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow,
                PhienBanHienTai = 1,
                IsDeleted = false,
                YeuCauTruoc = "Không có yêu cầu đặc biệt",
                HocDuoc = "Sau khóa học, bạn sẽ có thể xây dựng ứng dụng thực tế, hiểu rõ các khái niệm cốt lõi, và có nền tảng vững chắc để tiếp tục phát triển."
            };
            await context.KhoaHocs.AddAsync(khoaHoc);
            return khoaHoc;
        }

        private static async Task CreateChuongVaBaiGiangAsync(CourseOnlDbContext context, int idKhoaHoc, string tenChuong, (string tieuDe, string moTa, int thoiLuong)[] baiGiangs)
        {
            var chuong = new Chuong
            {
                IdKhoaHoc = idKhoaHoc,
                TenChuong = tenChuong,
                MoTa = $"Chương về {tenChuong.ToLower()}",
                ThuTu = await context.Chuongs.CountAsync(c => c.IdKhoaHoc == idKhoaHoc) + 1,
                TrangThai = true,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            };
            await context.Chuongs.AddAsync(chuong);
            await context.SaveChangesAsync();

            for (int i = 0; i < baiGiangs.Length; i++)
            {
                var (tieuDe, moTa, thoiLuong) = baiGiangs[i];
                var baiGiang = new BaiGiang
                {
                    IdChuong = chuong.Id,
                    TieuDe = tieuDe,
                    MoTa = moTa,
                    DuongDanVideo = null, // Để trống như yêu cầu
                    VideoPublicId = null,
                    ThoiLuong = thoiLuong, // Phút
                    ThuTu = i + 1,
                    MienPhiXem = i == 0, // Bài đầu tiên miễn phí
                    TrangThai = true,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.BaiGiangs.AddAsync(baiGiang);
            }
        }

        private static async Task CreateDangKyKhoaHocAsync(CourseOnlDbContext context, int idHocVien, int idKhoaHoc)
        {
            var exists = await context.DangKyKhoaHocs
                .AnyAsync(dk => dk.IdHocVien == idHocVien && dk.IdKhoaHoc == idKhoaHoc);
            
            if (!exists)
            {
                await context.DangKyKhoaHocs.AddAsync(new DangKyKhoaHoc
                {
                    IdHocVien = idHocVien,
                    IdKhoaHoc = idKhoaHoc,
                    NgayDangKy = DateTime.UtcNow,
                    TrangThai = true
                });
            }
        }

        private static async Task CreateDanhGiaAsync(CourseOnlDbContext context, int idHocVien, int idKhoaHoc, int diem, string binhLuan)
        {
            var exists = await context.DanhGiaKhoaHocs
                .AnyAsync(dg => dg.IdHocVien == idHocVien && dg.IdKhoaHoc == idKhoaHoc);
            
            if (!exists)
            {
                await context.DanhGiaKhoaHocs.AddAsync(new DanhGiaKhoaHoc
                {
                    IdHocVien = idHocVien,
                    IdKhoaHoc = idKhoaHoc,
                    DiemDanhGia = diem,
                    BinhLuan = binhLuan,
                    NgayDanhGia = DateTime.UtcNow,
                    TrangThai = true
                });
            }
        }

        private static async Task UpdateCourseRatingsAsync(CourseOnlDbContext context, int idKhoaHoc)
        {
            var danhGias = await context.DanhGiaKhoaHocs
                .Where(dg => dg.IdKhoaHoc == idKhoaHoc)
                .ToListAsync();
            
            if (danhGias.Any())
            {
                var khoaHoc = await context.KhoaHocs.FindAsync(idKhoaHoc);
                if (khoaHoc != null)
                {
                    khoaHoc.DiemDanhGia = (decimal)danhGias.Average(d => d.DiemDanhGia);
                    khoaHoc.SoLuongDanhGia = danhGias.Count;
                }
            }
        }

        // ===== HELPER METHODS CHO CÁC CHỨC NĂNG CÒN THIẾU =====

        private static async Task<Voucher> CreateVoucherAsync(CourseOnlDbContext context, string maCode, decimal tiLeGiam, DateTime ngayBatDau, DateTime ngayHetHan)
        {
            var voucher = await context.Vouchers.FirstOrDefaultAsync(v => v.MaCode == maCode);
            if (voucher == null)
            {
                voucher = new Voucher
                {
                    MaCode = maCode,
                    TiLeGiam = tiLeGiam,
                    NgayBatDau = ngayBatDau,
                    NgayHetHan = ngayHetHan,
                    TrangThai = DateTime.UtcNow >= ngayBatDau && DateTime.UtcNow <= ngayHetHan,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.Vouchers.AddAsync(voucher);
            }
            return voucher;
        }

        private static async Task<GioHang> CreateGioHangAsync(CourseOnlDbContext context, int idNguoiDung)
        {
            var gioHang = await context.GioHangs.FirstOrDefaultAsync(g => g.IdNguoiDung == idNguoiDung);
            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    IdNguoiDung = idNguoiDung,
                    TongTienGoc = 0,
                    TongTienThanhToan = 0,
                    SoLuongKhoaHoc = 0,
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow
                };
                await context.GioHangs.AddAsync(gioHang);
                await context.SaveChangesAsync();
            }
            return gioHang;
        }

        private static async Task CreateChiTietGioHangAsync(CourseOnlDbContext context, int idGioHang, int idKhoaHoc)
        {
            var exists = await context.ChiTietGioHangs
                .AnyAsync(c => c.IdGioHang == idGioHang && c.IdKhoaHoc == idKhoaHoc);
            
            if (!exists)
            {
                await context.ChiTietGioHangs.AddAsync(new ChiTietGioHang
                {
                    IdGioHang = idGioHang,
                    IdKhoaHoc = idKhoaHoc
                });
            }
        }

        private static async Task UpdateGioHangTotalAsync(CourseOnlDbContext context, int idGioHang)
        {
            var gioHang = await context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                    .ThenInclude(c => c.IdKhoaHocNavigation)
                .FirstOrDefaultAsync(g => g.Id == idGioHang);
            
            if (gioHang != null)
            {
                gioHang.TongTienGoc = gioHang.ChiTietGioHangs
                    .Where(c => c.IdKhoaHocNavigation != null && c.IdKhoaHocNavigation.GiaBan.HasValue)
                    .Sum(c => c.IdKhoaHocNavigation!.GiaBan!.Value);
                
                gioHang.SoLuongKhoaHoc = gioHang.ChiTietGioHangs.Count;
                gioHang.TongTienThanhToan = gioHang.TongTienGoc - (gioHang.TienGiamVoucher ?? 0);
                gioHang.NgayCapNhat = DateTime.UtcNow;
            }
        }

        private static async Task<DonHang> CreateDonHangAsync(CourseOnlDbContext context, int idNguoiDung, int? idVoucher, int[] idKhoaHocs, string trangThaiDonHang, string trangThaiThanhToan, string? phuongThucThanhToan, DateTime? ngayThanhToan)
        {
            // Tính tổng tiền
            var khoaHocs = await context.KhoaHocs
                .Where(k => idKhoaHocs.Contains(k.Id))
                .ToListAsync();
            
            var tongTienGoc = khoaHocs.Sum(k => k.GiaBan ?? 0);
            
            // Tính giảm giá từ voucher
            decimal? tienGiam = null;
            if (idVoucher.HasValue)
            {
                var voucher = await context.Vouchers.FindAsync(idVoucher.Value);
                if (voucher != null && voucher.TiLeGiam.HasValue)
                {
                    tienGiam = tongTienGoc * (voucher.TiLeGiam.Value / 100);
                }
            }
            
            var tongTienThanhToan = tongTienGoc - (tienGiam ?? 0);

            var donHang = new DonHang
            {
                IdNguoiDung = idNguoiDung,
                IdVoucher = idVoucher,
                TongTienGoc = tongTienGoc,
                TongTien = tongTienGoc,
                TienGiam = tienGiam,
                TongTienThanhToan = tongTienThanhToan,
                TrangThaiDonHang = trangThaiDonHang,
                TrangThaiThanhToan = trangThaiThanhToan,
                PhuongThucThanhToan = phuongThucThanhToan,
                NgayThanhToan = ngayThanhToan
            };
            
            await context.DonHangs.AddAsync(donHang);
            await context.SaveChangesAsync();

            // Tạo chi tiết đơn hàng
            foreach (var idKhoaHoc in idKhoaHocs)
            {
                await context.ChiTietDonHangs.AddAsync(new ChiTietDonHang
                {
                    IdDonHang = donHang.Id,
                    IdKhoaHoc = idKhoaHoc
                });
            }

            return donHang;
        }

        private static async Task UpdateDangKyKhoaHocFromOrderAsync(CourseOnlDbContext context, int idDonHang, int idHocVien, int[] idKhoaHocs)
        {
            foreach (var idKhoaHoc in idKhoaHocs)
            {
                var dangKy = await context.DangKyKhoaHocs
                    .FirstOrDefaultAsync(dk => dk.IdHocVien == idHocVien && dk.IdKhoaHoc == idKhoaHoc);
                
                if (dangKy != null)
                {
                    dangKy.IdDonHang = idDonHang;
                }
            }
        }


        private static async Task<TienDoHocTap?> CreateTienDoHocTapAsync(CourseOnlDbContext context, int idHocVien, int idKhoaHoc)
        {
            // Tìm đăng ký khóa học
            var dangKy = await context.DangKyKhoaHocs
                .FirstOrDefaultAsync(dk => dk.IdHocVien == idHocVien && dk.IdKhoaHoc == idKhoaHoc);
            
            if (dangKy == null) return null;

            var tienDo = await context.TienDoHocTaps
                .FirstOrDefaultAsync(td => td.IdDangKyKhoaHoc == dangKy.Id);
            
            if (tienDo == null)
            {
                // Đếm tổng số bài giảng
                var tongSoBaiGiang = await context.BaiGiangs
                    .Where(b => b.IdChuongNavigation!.IdKhoaHoc == idKhoaHoc && b.TrangThai == true)
                    .CountAsync();

                tienDo = new TienDoHocTap
                {
                    IdDangKyKhoaHoc = dangKy.Id,
                    IdKhoaHoc = idKhoaHoc,
                    IdHocVien = idHocVien,
                    SoBaiHocDaHoanThanh = 0,
                    TongSoBaiHoc = tongSoBaiGiang,
                    PhanTramHoanThanh = 0,
                    DaHoanThanh = false,
                    NgayBatDau = dangKy.NgayDangKy ?? DateTime.UtcNow
                };
                await context.TienDoHocTaps.AddAsync(tienDo);
                await context.SaveChangesAsync();
            }
            
            return tienDo;
        }

        private static async Task CreateTienDoHocTapChiTietAsync(CourseOnlDbContext context, int idTienDoHocTap, int idBaiGiang, bool daHoanThanh)
        {
            var exists = await context.TienDoHocTapChiTiets
                .AnyAsync(td => td.IdTienDoHocTap == idTienDoHocTap && td.IdBaiGiang == idBaiGiang);
            
            if (!exists)
            {
                await context.TienDoHocTapChiTiets.AddAsync(new TienDoHocTapChiTiet
                {
                    IdTienDoHocTap = idTienDoHocTap,
                    IdBaiGiang = idBaiGiang,
                    DaHoanThanh = daHoanThanh
                });
            }
        }

        private static async Task UpdateTienDoHocTapAsync(CourseOnlDbContext context, int idTienDoHocTap)
        {
            var tienDo = await context.TienDoHocTaps
                .Include(td => td.TienDoHocTapChiTiets)
                .FirstOrDefaultAsync(td => td.Id == idTienDoHocTap);
            
            if (tienDo != null)
            {
                tienDo.SoBaiHocDaHoanThanh = tienDo.TienDoHocTapChiTiets.Count(td => td.DaHoanThanh == true);
                
                if (tienDo.TongSoBaiHoc > 0)
                {
                    tienDo.PhanTramHoanThanh = (decimal)(tienDo.SoBaiHocDaHoanThanh * 100.0 / tienDo.TongSoBaiHoc);
                }
                
                tienDo.DaHoanThanh = tienDo.SoBaiHocDaHoanThanh == tienDo.TongSoBaiHoc && tienDo.TongSoBaiHoc > 0;
                
                if (tienDo.DaHoanThanh == true && tienDo.NgayHoanThanh == null)
                {
                    tienDo.NgayHoanThanh = DateTime.UtcNow;
                }
            }
        }
    }
}