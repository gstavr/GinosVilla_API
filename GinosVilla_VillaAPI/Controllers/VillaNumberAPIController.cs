using AutoMapper;
using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;
using GinosVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GinosVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new ();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbers = await _dbVillaNumber.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };

                return _response;
            }
        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {

            try
            {
                if(id == 0)
                {
                    _response.StatusCode =HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo.Equals(id));

                if(villaNumber is null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }


                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
                _response.StatusCode =HttpStatusCode.NotFound;

                return _response;
            }

        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if(await _dbVillaNumber.GetAsync(x=>x.VillaNo.Equals(createDTO.VillaNo)) is not null){
                    
                    ModelState.AddModelError("CustomError", "VillaNumber already exists");

                    return BadRequest(ModelState);
                }

                if(await _dbVilla.GetAsync(x=>x.Id.Equals(createDTO.VillaID)) is null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is Invalid");

                    return BadRequest(ModelState);
                }

                if(createDTO is null)
                {
                    return BadRequest(createDTO);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode=HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);




            }catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>{ ex.Message.ToString() };
                _response.StatusCode = HttpStatusCode.NotFound;

                return _response;
            }
        }


        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {

            try
            {
                if(id == 0)
                {
                    return BadRequest();
                }

                var villaNumber = await _dbVillaNumber.GetAsync(x=>x.VillaNo == id);

                if(villaNumber is null)
                {
                    return NotFound();
                }

                await _dbVillaNumber.RemoveAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };

                return _response;


            }
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO is null || id != updateDTO.VillaNo)
                {
                    return BadRequest();
                }

                if (await _dbVilla.GetAsync(x => x.Id.Equals(updateDTO.VillaID)) is null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is Invalid");

                    return BadRequest(ModelState);
                }


                VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);
                await _dbVillaNumber.UpdateAsync(villaNumber);

                _response.StatusCode =HttpStatusCode.OK;
                
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
                _response.IsSuccess = false;

                return _response;

            }

        }

    }
}
