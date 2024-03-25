using GinosVilla_Utility;
using GinosVilla_Web.Models;
using GinosVilla_Web.Models.Dto;
using GinosVilla_Web.Services;
using GinosVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GinosVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {

            LoginRequestDTO loginRequestDTO = new();
            
            return View(loginRequestDTO);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {

            APIResponse response = await _authService.LoginAsync<APIResponse>(loginRequestDTO);
            if(response is not null && response.IsSuccess)
            {
                TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.AccessToken);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x=>x.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                _tokenProvider.SetToken(model);
                //HttpContext.Session.SetString(SD.AccessToken, model.AccessToken);
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
                return View(loginRequestDTO);
            }

            
        }

        [HttpGet]
        public IActionResult Register() {

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.Admin, Value = SD.Admin},
                new SelectListItem{Text = SD.Customer, Value = SD.Customer},
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            if (string.IsNullOrEmpty(registrationRequestDTO.Role))
            {
                registrationRequestDTO.Role = SD.Customer;
            }

            APIResponse result= await _authService.RegisterAsync<APIResponse>(registrationRequestDTO);

            if(result is not null && result.IsSuccess )
            {
                return RedirectToAction("Login");
            }


            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.Admin, Value = SD.Admin},
                new SelectListItem{Text = SD.Customer, Value = SD.Customer},
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            var token = _tokenProvider.GetToken();
            await _authService.LogoutAsync<APIResponse>(token);
            _tokenProvider.ClearToken();
            

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
