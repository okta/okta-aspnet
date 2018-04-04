using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace Okta.AspNet.Test.WebApi.Controllers
{
    [Authorize]
    public class MessageController : ApiController
    {
        [HttpGet]
        [Route("~/api/messages")]
        public IEnumerable<string> Get()
        {
            var principal = RequestContext.Principal.Identity as ClaimsIdentity;

            var login = principal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            return new string[]
            {
            $"For {login ?? "your"} eyes only",
            "Your mission, should you choose to accept it..."
            };
        }
    }
}
