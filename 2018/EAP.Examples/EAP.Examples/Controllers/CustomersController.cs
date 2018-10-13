using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json;

namespace EAP.Examples.Controllers
{
	public class CustomersController : ApiController
	{
		[Route("khachhang")] // http://localhost:53796/khachhang

		[HttpGet]
		// [ActionName("GetCustomers")]
		
		public HttpResponseMessage GetCustomers()
		{
			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
			var result = new { Message = "Hello" };
			response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.Unicode, "application/json");
			response.Headers.CacheControl = new CacheControlHeaderValue()
			{
				MaxAge = TimeSpan.FromMinutes(20)
			};
			return response;
		}

		// Dapper
		public IHttpActionResult GetOrders(int id)
		{
			var connectionString = "";
			try
			{
				using (var db = new SqlConnection(connectionString))
				{
					string sql = "SELECT * FROM Orders";
					var parameters = new DynamicParameters();
					parameters.Add("@CurriculumId", id);
					var items = db.Query<dynamic>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

					var response = new
					{
						message = "OK",
						description = "Get orders",
						courses = items
					};

					return Ok(response);
				}
			}
			catch (Exception e)
			{
				var response = new
				{
					message = "Error",
					error = e.Message
				};

				return Json(response);
			}
		}
	}
}
