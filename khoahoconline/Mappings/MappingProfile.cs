using AutoMapper;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos.NguoiDung;

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
        }
    }
}
