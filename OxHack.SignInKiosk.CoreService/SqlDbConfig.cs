using OxHack.SignInKiosk.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.CoreService
{
	public class SqlDbConfig : IDbConfig
	{
		public string ConnectionString => ConfigurationManager.ConnectionStrings["SignInKioskDatabase"].ConnectionString;
	}
}
