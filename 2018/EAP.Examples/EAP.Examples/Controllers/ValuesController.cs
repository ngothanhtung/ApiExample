using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace EAP.Examples.Controllers
{
	public class ValuesController : ApiController
	{
		// GET api/values
		public IHttpActionResult Get()
		{
			var product = new[]
			{
				new
				{
					Id = Guid.NewGuid(),
					Name = "iPhone X",
					Price = 1000
				},
				new
				{
					Id = Guid.NewGuid(),
					Name = "iPhone XS",
					Price = 1500
				}
			};
			return Json(product);
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}
