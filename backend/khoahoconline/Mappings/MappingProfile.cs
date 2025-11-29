using AutoMapper;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Dtos.DanhMuc;

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
        }
    }
}