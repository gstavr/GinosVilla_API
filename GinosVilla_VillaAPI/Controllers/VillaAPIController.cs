using GinosVilla_VillaAPI.Data;
using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace GinosVilla_VillaAPI.Controllers
{
    //[Route("api/VillaAPI")] hardcoded controller name for the API
    [Route("api/[controller]")] // Takes the name of the controller
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas() // 1# Adding ActionResult defines the type that we need to return 
        {
            return Ok(VillaStore.villaList); // 1# When returning the ActionResult we need to say the type that we return for example Ok, NotFound etc
            // return VillaStore.villaList;  
        }

        // Informs that we expect on the Get Method an id value that it will be an integer
        [HttpGet("{id:int}", Name = "GetVilla")] // You can give explicit name to the route so you can use it 
        // Bellow different approaches of how to return and apply the Status Code that will be returned
        [ProducesResponseType(StatusCodes.Status200OK)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(VillaDTO))] // Shows what the availabe response types that will be produced in order not to show undocumented

        // [ProducesResponseType(200, Type = typeof(VillaDTO))] // Shows what the availabe response type of 200 and the type that will be returned  in order not to show undocumented (if you remove the VillaDTO from the function ex public ActionResult GetVilla(int id)
        // [ProducesResponseType(StatusCodes.Status400BadRequest)] // Shows what the availabe response types that will be produced in order not to show undocumented
        // [ProducesResponseType(404)] // Shows what the availabe response types that will be produced in order not to show undocumented
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id.Equals(id));
            if(villa is not VillaDTO) // Equal to villa == null
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // When Created and redirect to the resource 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO) { // The object that you receive is FromBody thats why we say it

            if(!ModelState.IsValid) // If [ApiController] Annotation is ennable it wont hit also this breakpoint everything will be done before reaching this END Point
            {
                return BadRequest(ModelState); // In order to display the error message if the API is missing the [ApiController] Annotation
            }


            //Throw custom error model if the name is not uniquie
            if(VillaStore.villaList.FirstOrDefault(x=>x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                // The custom error message
                ModelState.AddModelError("CustomError", "Villa Already Exists");
                return BadRequest(ModelState);
            }
            
            if (villaDTO is null) // Checks if null
            {
                return BadRequest(villaDTO); // Return BadRequest
            }

            if(villaDTO.Id > 0) // Means that it's not a create Request show we return a BadRequest or return something else with the Status Code
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Example of returning Bad request or not found
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

            // Add to the villa store
            VillaStore.villaList.Add(villaDTO);
            

            // Nice to return the route to the new Villa that is created
            // You need to provide the NAME of the route and the parameters that is needed
            return CreatedAtRoute("GetVilla", new { id= villaDTO.Id }, villaDTO);
            // return CreatedAtRoute("GetVilla", villaDTO, villaDTO); 
        }


        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id) // We can use the Interface so se dont define the type of what we return for example we dont return <VillaDto> that why we dont need the ActionResult
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x=> x.Id == id);
            if(villa != null)
            {
                return NotFound();
            }

            VillaStore.villaList.Remove(villa);

            return NoContent();
        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x=>x.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;


            return NoContent();
        }



        // https://jsonpatch.com/ this is the operations of the patch
        // We added also to nuget packages the JsonPatch and the NewtonSoft packages
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDto)
        {
            if(patchDto is null || id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x=>x.Id==id);
            if(villa == null)
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villa, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();    


        }

    }
}
