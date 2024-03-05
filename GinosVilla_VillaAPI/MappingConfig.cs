using AutoMapper;
using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;

namespace GinosVilla_VillaAPI
{
    /// <summary>
    /// This will be used for AUTO mapping
    /// </summary>
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();


            // VillaNumber Mapping
            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();

            // User
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            
        }
    }
}
