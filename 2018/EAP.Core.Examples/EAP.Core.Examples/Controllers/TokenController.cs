using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EAP.Core.Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public TokenController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        private string GenerateToken(string email)
        {
            var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:ServerSecret"]));

            var now = DateTime.UtcNow;
            var issuer = this.configuration["JWT:Issuer"];
            var audience = this.configuration["JWT:Audience"];
            var identity = new ClaimsIdentity();
            identity.AddClaims(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, email),

                }
            );

            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(this.configuration["JWT:ExpireMinutes"]));

            var signingCredentials = new SigningCredentials(serverSecret, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(issuer, audience, identity, now, expires, now, signingCredentials);
            var encodedJwt = handler.WriteToken(token);
            return encodedJwt;
        }      

        [HttpPost("login")]
        public IActionResult Login([FromBody] dynamic body)
        {
            var jwtToken = this.GenerateToken(body.Username.Value);

            var response = new
            {
                ok = true,
                description = "Login",                
                token = jwtToken
            };

            return new OkObjectResult(response);
        }

        [HttpGet("products")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProducts()
        {
            var response = new
            {
                ok = true,
                description = "Get Products",
            };

            return new OkObjectResult(response);
        }

        [HttpGet("customers")]
        [EnableCors("WebPolicy")]
        public IActionResult GetCustomers()
        {
            var response = new
            {
                ok = true,
                description = "Get Customers",
            };

            return new OkObjectResult(response);
        }
    }
}