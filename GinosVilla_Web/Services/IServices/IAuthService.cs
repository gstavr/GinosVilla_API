using GinosVilla_Web.Models.Dto;

namespace GinosVilla_Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
        Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO);
        Task<T> LogoutAsync<T>(TokenDTO tokenDTO);
    }
}
