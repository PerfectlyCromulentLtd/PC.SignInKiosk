using Newtonsoft.Json;
using OxHack.SignInKiosk.Domanin.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class SignInApiWrapper
	{
		private readonly Uri serviceBase;

		public SignInApiWrapper(Uri serviceBase)
		{
			this.serviceBase = serviceBase;
		}

		public async Task<IReadOnlyList<SignedInRecord>> GetCurrentlySignedIn()
		{
			string responseBody;
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(this.CurrentlySignedInResource);
				response.EnsureSuccessStatusCode();
				responseBody = await response.Content.ReadAsStringAsync();
			}

			var result = JsonConvert.DeserializeObject<List<SignedInRecord>>(responseBody);

			return result;
		}

		public Uri CurrentlySignedInResource
			=> new Uri(this.serviceBase, "/api/v1/currentlySignedIn/");
	}
}
