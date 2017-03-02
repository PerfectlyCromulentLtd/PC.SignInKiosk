using Microsoft.AspNetCore.Mvc;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domain.Models;
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

		[HttpGet]
        public IEnumerable<SignedInRecord> Get()
        {
			var currentlySignedIn = this.signInService.GetCurrentlySignedIn();

			return currentlySignedIn;
        }
    }
}
