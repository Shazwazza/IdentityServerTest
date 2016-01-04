using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using System.ServiceModel;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace MyIdentityServer.Umbraco.Controllers
{
    [Authorize]
    public class MyApiController : UmbracoApiController
    {        
        public IEnumerable<string> GetStuff()
        {
            return new string[] { "value1", "value2" };
        }
    }

    //public class AuthAttribute : System.Web.Http.AuthorizeAttribute
    //{
    //    /// <summary>
    //    /// Processes requests that fail authorization.
    //    /// </summary>
    //    /// <param name="actionContext">The context.</param>
    //    protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
    //    {
    //        var user = actionContext.RequestContext.Principal;
    //        if (user.Identity.IsAuthenticated)
    //        {
    //            // 403 we know who you are, but you haven't been granted access
    //            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden);
    //        }
    //        else
    //        {
    //            // 401 who are you? go login and then try again
    //            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
    //        }
    //    }
        
    //}
}