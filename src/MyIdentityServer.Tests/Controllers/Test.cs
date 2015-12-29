using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyIdentityServer.Tests.Controllers
{
    [Route("test")]
    public class TestController : ApiController
    {
        public IHttpActionResult Get()
        {
            var caller = User as ClaimsPrincipal;

            return Json(new
            {
                message = "OK computer"
            });

            //return Json(new
            //{
            //    message = "OK computer",
            //    client = caller.FindFirst("client_id").Value
            //});
        }
    }
}
