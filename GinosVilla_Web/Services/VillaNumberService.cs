using GinosVilla_Web.Models;
using GinosVilla_Web.Models.Dto;
using GinosVilla_Web.Services.IServices;
using GinosVilla_Utility;
namespace GinosVilla_Web.Services
{
    public class VillaNumberService : IVillaNumberService
    {   
        private string villaUrl;
        private readonly IBaseService _baseService;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService) 
        {
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> CreateAsync<T>(VillaNumberCreateDTO dto )
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                //Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaNumberAPI",
                Url = villaUrl + $"/api/v1/VillaNumberAPI",
                
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,                
                //Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaNumberAPI/" + id,
                Url = villaUrl + $"/api/v1/VillaNumberAPI/" + id,
               
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                //Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaNumberAPI",
                Url = villaUrl + $"/api/v1/VillaNumberAPI",
                
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,                
                //Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaNumberAPI/" + id,
                Url = villaUrl + $"/api/v1/VillaNumberAPI/" + id,
                
            });
        }

        public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest() 
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                //Url = villaUrl + $"/api/{SD.CurrentAPIVerson}/VillaNumberAPI/" + dto.VillaNo,
                Url = villaUrl + $"/api/v1/VillaNumberAPI/" + dto.VillaNo,
                
            });
        }
    }
}
