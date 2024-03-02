using GinosVilla_VillaAPI.Models;
using GinosVilla_VillaAPI.Models.Dto;
using GinosVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GinosVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/UsersAuth")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepository) {
            _userRepository = userRepository;
            this._response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepository.Login(model);

            if (loginResponse.User is null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");

                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginResponse;

            return Ok(_response);                        
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique =  _userRepository.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists!");
                return BadRequest(_response);
            }

            var user = _userRepository.Register(model);

            if (user is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering!");
                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            //_response.Result = user;

            return Ok(_response);
            
        }
    }
}
