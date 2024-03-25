using GinosVilla_Utility;
using GinosVilla_Web.Models;
using GinosVilla_Web.Models.Dto;
using GinosVilla_Web.Services.IServices;

namespace GinosVilla_Web.Services
{
    public class AuthService :  IAuthService
    {
        
        private string villaUrl;
        private readonly IBaseService _baseService;
        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBaseService baseService) 
        {   
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }
        public async Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO )
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/UsersAuth/login"
            }, withBearer: false);
        }

        public async Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/UsersAuth/register"
            }, withBearer: false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO tokenDTO)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = tokenDTO,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/UsersAuth/revoke"
            });
        }
    }
}
