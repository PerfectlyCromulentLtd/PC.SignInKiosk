using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Events
{
	public class VisibilityChanged
	{
		public VisibilityChanged(bool isVisible)
		{
			this.IsVisible = isVisible;
		}

		public bool IsVisible
		{
			get;
		}
	}
}
