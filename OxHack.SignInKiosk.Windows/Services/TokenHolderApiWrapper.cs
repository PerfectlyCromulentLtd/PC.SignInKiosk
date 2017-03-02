using Newtonsoft.Json;
using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class TokenHolderApiWrapper
	{
		private readonly Uri serviceBase;

		public TokenHolderApiWrapper(Uri serviceBase)
		{
			this.serviceBase = serviceBase;
		}

		public async Task<TokenHolder> GetTokenHolderByTokenId(string tokenId)
		{
			string responseBody;
			using (var client = new HttpClient())
			{
				var uri = new Uri(this.TokenHolderResource, Uri.EscapeUriString(tokenId));

				var response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseBody = await response.Content.ReadAsStringAsync();
			}

			var result = JsonConvert.DeserializeObject<TokenHolder>(responseBody);

			return result;
		}

		private Uri TokenHolderResource
			=> new Uri(this.serviceBase, "/api/v1/tokenHolders/");
	}
}
