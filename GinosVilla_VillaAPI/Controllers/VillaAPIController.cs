using AutoMapper;
using Azure;
using GinosVilla_VillaAPI.Data;
using GinosVilla_VillaAPI.Logging;
using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;
using GinosVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GinosVilla_VillaAPI.Controllers
{
    //[Route("api/VillaAPI")] hardcoded controller name for the API
    [Route("api/[controller]")] // Takes the name of the controller
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;

        // private readonly ApplicationDbContext _db;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        
        // private readonly ILogger<VillaAPIController> _logger { get; }
        
        // Custom Logger Interface
        //private readonly ILogging _logger;

        // public VillaAPIController(ILogging logger) Custom Logging DI
        // public VillaAPIController(ILogger<VillaAPIController> logger) this is how we implement build in logger
        // public VillaAPIController(ApplicationDbContext db, IMapper mapper) 
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            this._mapper = mapper;
            this._dbVilla = dbVilla;
            this._response = new();
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas() // 1# Adding ActionResult defines the type that we need to return 
        public async Task<ActionResult<APIResponse>> GetVillas() // Return APIResponse insteadof the above
        {
            try
            {
                //Custom Logger
                //_logger.Log("Getting all villas", "");
                //_logger.LogInformation("Getting all villas");

                //return Ok(await _db.Villas.ToListAsync()); // 1# When returning the ActionResult we need to say the type that we return for example Ok, NotFound etc

                // With AutoMapper
                //IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();

                //With RepositoryPattern
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();


                _response.Result = this._mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response); // New Return with the object APIRESPONSE

                //return Ok(this._mapper.Map<List<VillaDTO>>(villaList));
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { 
                    ex.ToString()
                };

                return _response;
            }
            
        }

        // Informs that we expect on the Get Method an id value that it will be an integer
        [HttpGet("{id:int}", Name = "GetVilla")] // You can give explicit name to the route so you can use it 
        [Authorize(Roles = "admin")]
        // Bellow different approaches of how to return and apply the Status Code that will be returned
        [ProducesResponseType(StatusCodes.Status200OK)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(VillaDTO))] // Shows what the availabe response types that will be produced in order not to show undocumented
        // [ProducesResponseType(200, Type = typeof(VillaDTO))] // Shows what the availabe response type of 200 and the type that will be returned  in order not to show undocumented (if you remove the VillaDTO from the function ex public ActionResult GetVilla(int id)
        // [ProducesResponseType(StatusCodes.Status400BadRequest)] // Shows what the availabe response types that will be produced in order not to show undocumented
        // [ProducesResponseType(404)] // Shows what the availabe response types that will be produced in order not to show undocumented
        //public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    //Custom Logger
                    //_logger.Log("Getting all villas", "error");

                    // Logger
                    //_logger.LogInformation("Get Villa Error with id " + id);

                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                    //return BadRequest();
                }

                // var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id.Equals(id));

                //With Repository Pattern
                var villa = await _dbVilla.GetAsync(x => x.Id.Equals(id));

                if (villa is null) // Equal to villa == null
                {
                    _response.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(_response);
                    //return NotFound();
                }


                _response.Result = this._mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response); // New Return with the object APIRESPONSE

                //return Ok(this._mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };

                return _response;
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)] // When Created and redirect to the resource 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO) { // The object that you receive is FromBody thats why we say
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {

            try
            {
                if (!ModelState.IsValid) // If [ApiController] Annotation is ennable it wont hit also this breakpoint everything will be done before reaching this END Point
                {
                    return BadRequest(ModelState); // In order to display the error message if the API is missing the [ApiController] Annotation
                }


                //Throw custom error model if the name is not uniquie
                // if (await _db.Villas.FirstOrDefaultAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
                if (await _dbVilla.GetAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    // The custom error message
                    ModelState.AddModelError("ErrorMessages", "Villa Already Exists");
                    return BadRequest(ModelState);
                }

                if (createDTO is null) // Checks if null
                {
                    return BadRequest(createDTO); // Return BadRequest
                }

                //if(villaDTO.Id > 0) // Means that it's not a create Request show we return a BadRequest or return something else with the Status Code
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError); // Example of returning Bad request or not found
                //}

                // Use the AUTOMAPPER To do the mapping as we did manually bellow
                Villa villa = _mapper.Map<Villa>(createDTO);

                // Map the new Villa cause it DTO
                //Villa model = new()
                //{
                //    Amenity = createDTO.Amenity,
                //    Details = createDTO.Details,                
                //    ImageUrl = createDTO.ImageUrl,
                //    Name = createDTO.Name,
                //    Occupancy = createDTO.Occupancy,
                //    Rate = createDTO.Rate,
                //    Sqft = createDTO.Sqft,
                //};

                //await _db.Villas.AddAsync(model);
                //await _db.SaveChangesAsync();
                // Repository Pattern Bellow and above save to db 
                await _dbVilla.CreateAsync(villa);


                // Nice to return the route to the new Villa that is created
                // You need to provide the NAME of the route and the parameters that is needed

                //return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
                // return CreatedAtRoute("GetVilla", villaDTO, villaDTO); 



                _response.Result = this._mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response); // New Return with the object APIRESPONSE
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };

                return _response;
            }
                        
        }


        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "CUSTOM")]
        // public async Task<IActionResult> DeleteVilla(int id) // We can use the Interface so se dont define the type of what we return for example we dont return <VillaDto> that why we dont need the ActionResult
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id) // 
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                // var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id); BEFORE REPOSITORY PATTER
                var villa = await _dbVilla.GetAsync(x => x.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                //_db.Villas.Remove(villa);
                //await _db.SaveChangesAsync();

                await _dbVilla.RemoveAsync(villa);

                // New Response Object            
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);

                //return NoContent();
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };

                return _response;
            }
            
        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                ///// Use the AUTOMAPPER To do the mapping as we did manually bellow
                Villa model = this._mapper.Map<Villa>(updateDTO);

                //_db.Villas.Update(model);
                //await _db.SaveChangesAsync();
                // Repository Pattern Bellow Old pattern above
                await _dbVilla.UpdateAsync(model);


                // New Response Object            
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);

                //return NoContent();
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };

                return _response;
            }            
        }



        // https://jsonpatch.com/ this is the operations of the patch
        // We added also to nuget packages the JsonPatch and the NewtonSoft packages
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {

            if (patchDto is null || id == 0)
            {
                return BadRequest();
            }

            // The AsNoTracking stops the tracking of the ENTITY FRAMEWORK
            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
            var villa = await _dbVilla.GetAsync(x => x.Id == id, tracked: false);

            ///////
            ///// Use the AUTOMAPPER To do the mapping as we did manually bellow
            VillaUpdateDTO villaDTO = this._mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDTO, ModelState);

            ///////
            ///// Use the AUTOMAPPER To do the mapping as we did manually bellow
            Villa model = this._mapper.Map<Villa>(villaDTO);


            //_db.Villas.Update(model);
            //await _db.SaveChangesAsync();
            // Repository Pattern Bellow Old pattern above
            await _dbVilla.UpdateAsync(model);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }

    }
}
