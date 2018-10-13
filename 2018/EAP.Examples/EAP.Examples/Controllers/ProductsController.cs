using EAP.Examples.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EAP.Examples.Controllers
{
	public class ProductsController : ApiController
    {
	    Product[] products = new Product[]
	    {
		    new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
		    new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
		    new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
	    };

		// http://localhost:53796/api/products
		public IEnumerable<Product> GetAllProducts()
	    {
		    return products;
	    }

		// http://localhost:53796/api/products/1
		public IHttpActionResult GetProduct(int id)
	    {
		    var product = products.FirstOrDefault((p) => p.Id == id);
		    if (product == null)
		    {
			    return NotFound();
		    }
		    return Ok(product);
	    }
	}
}
