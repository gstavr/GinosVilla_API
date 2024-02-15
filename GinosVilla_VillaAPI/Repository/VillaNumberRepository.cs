using GinosVilla_VillaAPI.Data;
using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace GinosVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaNumberRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }

        
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.VillaNumbers.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
