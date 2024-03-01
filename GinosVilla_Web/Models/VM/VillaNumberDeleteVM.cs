using GinosVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GinosVilla_Web.Models.VM
{
    public class VillaNumberDeleteVM
    {
        public VillaNumberDTO VillaNumber { get; set; }
        public VillaNumberDeleteVM() { 
            VillaNumber = new VillaNumberDTO();        
        }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
