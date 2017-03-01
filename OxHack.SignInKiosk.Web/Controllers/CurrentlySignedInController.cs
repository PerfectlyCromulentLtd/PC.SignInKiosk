using Microsoft.AspNetCore.Mvc;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domanin.Models;
using System.Collections.Generic;

namespace OxHack.SignInKiosk.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
    public class CurrentlySignedInController : Controller
    {
		private readonly SignInService signInService;

		public CurrentlySignedInController(SignInService signInService)
		{
			this.signInService = signInService;
		}

		// GET api/values
		[HttpGet]
        public IEnumerable<SignedInRecord> Get()
        {
			var currentlySignedIn = this.signInService.GetCurrentlySignedIn();

			return currentlySignedIn;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
