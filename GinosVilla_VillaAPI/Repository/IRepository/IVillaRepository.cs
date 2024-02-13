using GinosVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace GinosVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository:IRepository<Villa> // We speficy the generic that it will type of Villa
    {
        Task<Villa> UpdateAsync(Villa entity);
    }
}
