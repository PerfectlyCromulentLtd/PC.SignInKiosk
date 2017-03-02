using Microsoft.AspNetCore.Mvc;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Collections.Generic;

namespace OxHack.SignInKiosk.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class TokenHoldersController : Controller
	{
		private readonly TokenHolderService tokenHolderService;

		public TokenHoldersController(TokenHolderService tokenHolderService)
		{
			this.tokenHolderService = tokenHolderService;
		}

		[HttpGet("{tokenId}")]
		public TokenHolder Get(string tokenId)
		{
			try
			{
				return this.tokenHolderService.GetTokenHolderByTokenId(tokenId);
			}
			catch (Exception ex)
			{
				// TODO: Log error
				throw;
			}
		}
	}
}
