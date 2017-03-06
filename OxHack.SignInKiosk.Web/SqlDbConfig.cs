using System;
using OxHack.SignInKiosk.Database;
using Microsoft.Extensions.Configuration;

namespace OxHack.SignInKiosk.Web
{
	public class SqlDbConfig : IDbConfig
	{
		private readonly IConfigurationRoot config;

		public SqlDbConfig(IConfigurationRoot config)
		{
			this.config = config;
		}

		public string ConnectionString
		{
			get
			{
				return
					Environment.GetEnvironmentVariable("ConnectionString")
					?? this.config["DbConnectionString"];
			}
		}
	}
}