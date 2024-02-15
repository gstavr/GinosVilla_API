using GinosVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace GinosVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber> // We speficy the generic that it will type of Villa
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
