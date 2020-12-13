using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;
using Profound.Data.ViewModels;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IDataRepository _dataRepository;

        public AccountController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _dataRepository.LoginUser(email, password);
            if (user == null)
            {
                return NoContent();
            }
            await Authenticate(email);
            return CreatedAtAction("Login", user);
        }

        [HttpPost("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = _dataRepository.LoginUser(model.Email, model.Password);
            if(user == null)
            {
                _dataRepository.RegisterUser(model);
                await Authenticate(model.Email);
                return CreatedAtAction("Register", user);
            }
            else
            {
                return NoContent();
            }
        }

        private async Task Authenticate(string userName)
        {            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}