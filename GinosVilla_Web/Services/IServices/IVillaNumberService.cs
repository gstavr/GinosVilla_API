﻿using GinosVilla_Web.Models.Dto;

namespace GinosVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        //Task<T> GetAllAsync<T>(string token);
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreateDTO dto);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
