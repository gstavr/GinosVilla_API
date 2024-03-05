using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;

namespace GinosVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register (RegistrationRequestDTO registrationRequestDTO);
    }
}
