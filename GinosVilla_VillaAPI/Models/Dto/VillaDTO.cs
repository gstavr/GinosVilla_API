using System.ComponentModel.DataAnnotations;

namespace GinosVilla_VillaAPI.Models.Dto
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required!!!! Custom Message Error")] // Makes the Field Required with custom error message when is being validated
        [StringLength(30)]
        [MaxLength(30)]        
        public string Name { get; set; }
    }
}
