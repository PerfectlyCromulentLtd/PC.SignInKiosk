using System;

namespace OxHack.SignInKiosk.Database
{
	internal class DummyConfig : IDbConfig
	{
		public string ConnectionString
		{
			get => "";
		}
	}
}