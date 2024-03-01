using GinosVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GinosVilla_Web.Models.VM
{
    public class VillaNumberUpdateVM
    {
        public VillaNumberUpdateDTO VillaNumber { get; set; }
        public VillaNumberUpdateVM() { 
            VillaNumber = new VillaNumberUpdateDTO();        
        }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
