using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ApiExample.Models;

namespace ApiExample.Controllers
{
    public class ProductsController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();
        public IEnumerable<Product> GetAllProducts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            return db.Products.ToList();
        }

        public IHttpActionResult GetProduct(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
