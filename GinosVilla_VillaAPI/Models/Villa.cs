using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GinosVilla_VillaAPI.Models
{
    public class Villa
    {
        [Key] // Say to the DataBase that this is the PRIMARY KEY Automatically
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // We want to be an Identity Column (Automatically manage the id)
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
        public string Amenity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
