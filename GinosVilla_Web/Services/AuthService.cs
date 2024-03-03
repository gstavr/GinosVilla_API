using GinosVilla_Utility;
using GinosVilla_Web.Models;
using GinosVilla_Web.Models.Dto;
using GinosVilla_Web.Services.IServices;

namespace GinosVilla_Web.Services
{
    public class AuthService :BaseService,  IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration):base(httpClientFactory) {
            _httpClientFactory = httpClientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO )
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = villaUrl + "/api/v1/UsersAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = villaUrl + "/api/v1/UsersAuth/register"
            });
        }
    }
}
