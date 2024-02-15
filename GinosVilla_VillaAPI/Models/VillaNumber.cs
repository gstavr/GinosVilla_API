using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace GinosVilla_VillaAPI.Models
{
    public class VillaNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // Explain that will be the key but it VillaNo will be provided by the user
        public int VillaNo { get; set; }

        [ForeignKey("Villa")] // Add a ForeignKey from the Villa you need the VillaID and the Villa properties
        public int VillaID { get; set; }
        public Villa Villa { get; set; }

        public string SpecialDetails { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
    }
}
