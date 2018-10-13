using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EAP.Examples
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services			

			// Web API routes
			config.MapHttpAttributeRoutes();

			// Convention-based routing.
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			// Routing by Action Name
			//config.Routes.MapHttpRoute(
			//	name: "DefaultApi",
			//	routeTemplate: "api/{controller}/{action}/{id}",
			//	defaults: new { id = RouteParameter.Optional }
			//);
		}
	}
}
