using Asp.Versioning;
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
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace GinosVilla_VillaAPI.Controllers.v2
{
    //[Route("api/VillaAPI")] hardcoded controller name for the API
    //[Route("api/[controller]")] // Takes the name of the controller
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;

        // private readonly ApplicationDbContext _db;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _mapper = mapper;
            _dbVilla = dbVilla;
            _response = new();
        }


        [HttpGet]
      
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]       
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy, [FromQuery] string? search,
            int pageSize = 0, int pageNumber = 1) // Return APIResponse insteadof the above with 2 attributes in the Query
        {
            try
            {
                                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();

                if (occupancy > 0)
                {
                    villaList = await _dbVilla.GetAllAsync(x => x.Occupancy == occupancy, pageSize: pageSize, pageNumber: pageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(x => x.Name.ToLower().Contains(search));
                }

                
                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize };
                
                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response); // New Return with the object APIRESPONSE

                
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
        [ProducesResponseType(StatusCodes.Status200OK)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Shows what the availabe response types that will be produced in order not to show undocumented
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(VillaDTO))] // Shows what the availabe response types that will be produced in order not to show undocumented
        
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                    
                }
                                
                var villa = await _dbVilla.GetAsync(x => x.Id.Equals(id));

                if (villa is null) // Equal to villa == null
                {
                    _response.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(_response);
                    
                }


                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response); 
                
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
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDTO createDTO)
        {

            try
            {
                if (!ModelState.IsValid) // If [ApiController] Annotation is ennable it wont hit also this breakpoint everything will be done before reaching this END Point
                {
                    return BadRequest(ModelState); // In order to display the error message if the API is missing the [ApiController] Annotation
                }
              
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
                
                Villa villa = _mapper.Map<Villa>(createDTO);

                
                await _dbVilla.CreateAsync(villa);

                if(createDTO.Image is not null)
                {
                    string fileName = villa.Id + Path.GetExtension(createDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    
                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    villa.ImageLocalPath = filePath;
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400";
                }

                await _dbVilla.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
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
        [Authorize(Roles = "admin")]
        
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id) // 
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

               
                var villa = await _dbVilla.GetAsync(x => x.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(villa.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                await _dbVilla.RemoveAsync(villa);

                     
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);

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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                
                Villa villa = _mapper.Map<Villa>(updateDTO);

                if (updateDTO.Image is not null)
                {
                    if (!string.IsNullOrEmpty(villa.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = updateDTO.Id + Path.GetExtension(updateDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        updateDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    villa.ImageLocalPath = filePath;
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400";
                }

                await _dbVilla.UpdateAsync(villa);

                     
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);

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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {

            if (patchDto is null || id == 0)
            {
                return BadRequest();
            }            
            var villa = await _dbVilla.GetAsync(x => x.Id == id, tracked: false);
                       
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDTO, ModelState);

          
            Villa model = _mapper.Map<Villa>(villaDTO);


            await _dbVilla.UpdateAsync(model);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }

    }
}
