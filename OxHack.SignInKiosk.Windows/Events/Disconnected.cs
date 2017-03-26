using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Events
{
	public class Disconnected
	{
		public Disconnected(bool isFault = false)
		{
			this.IsFault = isFault;
		}

		public bool IsFault
		{
			get;
		}
	}
}
