using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        [HttpPost("token")]
        public IActionResult Token(LoginViewModel model)
        {
            var identity = GetIdentity(model.Email, model.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid email or password." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var user = _dataRepository.GetUserByEmail(identity.Name);
            var response = new
            {
                access_token = encodedJwt,
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                role = _dataRepository.GetRoleByUserId(user.Id)?.RoleName
            };
            
            return new JsonResult(response);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (_dataRepository.LoginUser(model.Email, model.Password) != null)
            {
                return BadRequest(new { errorText = "Such user already exists!" });
            }

            _dataRepository.RegisterUser(model);
            return Token(new LoginViewModel {Email = model.Email, Password = model.Password });
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User user = _dataRepository.LoginUser(username, password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.RoleName)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
    }
}
