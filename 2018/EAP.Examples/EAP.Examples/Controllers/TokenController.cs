// https://www.red-gate.com/simple-talk/dotnet/net-development/jwt-authentication-microservices-net/
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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


		public static ClaimsPrincipal GetPrincipal(string token)
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
				ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
				return principal;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public static string ValidateToken(string token)
		{
			string username = null;
			ClaimsPrincipal principal = GetPrincipal(token);
			if (principal == null)
				return null;
			ClaimsIdentity identity = null;
			try
			{
				identity = (ClaimsIdentity)principal.Identity;
			}
			catch (NullReferenceException)
			{
				return null;
			}
			Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
			username = usernameClaim.Value;
			return username;
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
		public HttpResponseMessage Validate(string token, string username)
		{
			bool exists = username == "admin";
			if (!exists) return Request.CreateResponse(HttpStatusCode.NotFound, "The user was not found.");

			string tokenUsername = ValidateToken(token);
			if (username.Equals(tokenUsername)) return Request.CreateResponse(HttpStatusCode.OK);

			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}
	}
}
