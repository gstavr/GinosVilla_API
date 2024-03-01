using GinosVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GinosVilla_Web.Models.VM
{
    public class VillaNumberCreateVM
    {
        public VillaNumberCreateDTO VillaNumber { get; set; }
        public VillaNumberCreateVM() { 
            VillaNumber = new VillaNumberCreateDTO();        
        }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
