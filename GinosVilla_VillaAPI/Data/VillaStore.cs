using GinosVilla_VillaAPI.Models.Dto;

namespace GinosVilla_VillaAPI.Data
{
    /// <summary>
    /// This was used as a temporary database for Testing Purposes it is removed from the app
    /// </summary>
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
        {
            new VillaDTO() {Id=1, Name="Pool View", Sqft=100, Occupancy=4},
            new VillaDTO() {Id=2, Name="Beach View", Sqft=300, Occupancy=3},
        };
    }
}
