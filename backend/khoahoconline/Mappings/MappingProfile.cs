using AutoMapper;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Dtos.Notification;
using khoahoconline.Dtos.Review;

namespace khoahoconline.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create your mappings here
            // NguoiDung mappings
            CreateMap<NguoiDungDto, NguoiDung>();
            CreateMap<CreateNguoiDungDto, NguoiDung>();
            CreateMap<NguoiDung, NguoiDungDto>();
            CreateMap<UpdateNguoiDungDto, NguoiDung>();
            CreateMap<NguoiDung, NguoiDungDetailDto>();
            CreateMap<UpdateProfileDto, NguoiDung>();

            // DanhMuc mappings
            CreateMap<DanhMucKhoaHoc, DanhMucDto>();
            CreateMap<DanhMucKhoaHoc, DanhMucDetailDto>()
                .ForMember(dest => dest.SoKhoaHoc, opt => opt.Ignore()); // Set manually in service
            CreateMap<CreateDanhMucDto, DanhMucKhoaHoc>();
            CreateMap<UpdateDanhMucDto, DanhMucKhoaHoc>();

            // Notification mappings
            CreateMap<Notification, NotificationDto>();

            // Review mappings
            CreateMap<DanhGiaKhoaHoc, ReviewDto>()
                .ForMember(dest => dest.HoTenHocVien, opt => opt.MapFrom(src => src.IdHocVienNavigation != null ? src.IdHocVienNavigation.HoTen : "Người dùng"))
                .ForMember(dest => dest.AnhDaiDien, opt => opt.MapFrom(src => src.IdHocVienNavigation != null ? src.IdHocVienNavigation.AnhDaiDien : null));
        }
    }
}