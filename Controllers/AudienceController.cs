using FTBAPISERVER.Entities;
using FTBAPISERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FTBAPISERVER.Controllers
{
    [RoutePrefix("api/audience")]
    public class AudienceController : ApiController
    {
        [Route("")]
        public IHttpActionResult Post(AudienceModel audienceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Audience newAudience = AudiencesStore.AddAudience(audienceModel.Name);

            return Ok<Audience>(newAudience);

        }


        [HttpGet]
        public IHttpActionResult Login()
        {
            return Ok("HADSGDJDJSLASDIASJDUAHJDSJHJHJASDJASDASDASD");
        }
        public bool authorizeADUser()
        {
            return true;
        }

        
    }
}
