using GinosVilla_Web.Models;
using GinosVilla_Web.Models.Dto;
using GinosVilla_Web.Services.IServices;
using GinosVilla_Utility;
namespace GinosVilla_Web.Services
{
    public class VillaService : IVillaService
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaUrl;
        private readonly IBaseService _baseService;
        public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService) 
        {
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _httpClient = httpClient;
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaAPI",              
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,                
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaAPI/" + id,                
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaAPI",                
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,                
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaAPI/" + id,
                
            });
        }

        public async Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest() 
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaAPI/" + dto.Id,                
                ContentType = SD.ContentType.MultipartFormData

            });
        }
    }
}
