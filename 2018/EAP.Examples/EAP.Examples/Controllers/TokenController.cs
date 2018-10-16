// https://www.red-gate.com/simple-talk/dotnet/net-development/jwt-authentication-microservices-net/
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace EAP.Examples.Controllers
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class TokenController : ApiController
	{
		private static string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
		public static string GenerateToken(string username)
		{
			byte[] key = Convert.FromBase64String(Secret);
			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
			SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
				Expires = DateTime.UtcNow.AddMinutes(30),
				SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
			return handler.WriteToken(token);
		}


		public static ClaimsPrincipal ValidateToken(string token)
		{
			try
			{
				JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
				JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
				if (jwtToken == null)
					return null;

				byte[] key = Convert.FromBase64String(Secret);
				TokenValidationParameters parameters = new TokenValidationParameters()
				{
					RequireExpirationTime = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};

				SecurityToken securityToken;
				return tokenHandler.ValidateToken(token, parameters, out securityToken);				
			}
			catch (Exception e)
			{
				return null;
			}
		}	

		[HttpPost]
		public HttpResponseMessage Login(User user)
		{

			if (user.Username != "admin")
			{
				return Request.CreateResponse(HttpStatusCode.NotFound, "The user was not found.");
			}

			bool credentials = user.Password.Equals("123456789");

			if (!credentials)
			{
				return Request.CreateResponse(HttpStatusCode.Forbidden, "The username/password combination was wrong.");
			}

			return Request.CreateResponse(HttpStatusCode.OK, GenerateToken(user.Username));
		}

		[HttpGet]
		public HttpResponseMessage NoToken()
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}
		[HttpGet]
		public HttpResponseMessage Validate()
		{
			var token = Request.Headers.Authorization.Parameter;
			var result = ValidateToken(token);
			if (result == null)
			{
				return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}

			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}
