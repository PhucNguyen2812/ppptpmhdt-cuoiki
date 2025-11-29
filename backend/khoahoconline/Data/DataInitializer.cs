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

                // create default roles
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "ADMIN"))
                {
                    await context.VaiTros.AddAsync(new VaiTro { TenVaiTro = "ADMIN", TrangThai = true });
                }

                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "GIANGVIEN"))
                {
                    await context.VaiTros.AddAsync(new VaiTro { TenVaiTro = "GIANGVIEN", TrangThai = true });
                }
                if (!await context.VaiTros.AnyAsync(vt => vt.TenVaiTro == "HOCVIEN"))
                {
                    await context.VaiTros.AddAsync(new VaiTro { TenVaiTro = "HOCVIEN", TrangThai = true });
                }

                await context.SaveChangesAsync();

                // Lấy vai trò ADMIN
                var vaiTro = await context.VaiTros
                    .FirstOrDefaultAsync(vt => vt.TenVaiTro == "ADMIN");

                if (vaiTro == null)
                {
                    Console.WriteLine("✗ ADMIN role not found!");
                    return;
                }

                // create default admin user - check if any admin exists
                var adminExists = await context.NguoiDungVaiTros
                    .AnyAsync(ndvt => ndvt.IdVaiTro == vaiTro.Id);

                if (!adminExists)
                {
                    var nguoiDung = new NguoiDung
                    {
                        Email = "admin@gmail.com",
                        MatKhau = PasswordHelper.HashPassword("admin123"),
                        TrangThai = true,
                        NgayTao = DateTime.Now,
                        HoTen = "Administrator"
                    };

                    await context.NguoiDungs.AddAsync(nguoiDung);
                    await context.SaveChangesAsync();

                    // Create relationship in NguoiDungVaiTro table
                    var nguoiDungVaiTro = new NguoiDungVaiTro
                    {
                        IdNguoiDung = nguoiDung.Id,
                        IdVaiTro = vaiTro.Id
                    };
                    await context.NguoiDungVaiTros.AddAsync(nguoiDungVaiTro);
                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                throw;
            }
        }
    }
}