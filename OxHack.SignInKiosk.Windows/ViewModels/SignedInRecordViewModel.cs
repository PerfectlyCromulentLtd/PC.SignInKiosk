using Humanizer;
using OxHack.SignInKiosk.Domain.Models;
using System;

namespace OxHack.SignInKiosk.ViewModels
{
	public class SignedInRecordViewModel
	{
		public SignedInRecordViewModel(SignedInRecord model)
		{
			this.Model = model;
		}

		public string DisplayName
			=> this.Model.DisplayName;

		public string SignInTime
			=> $"Signed-in @ {this.Model.SignInTime.ToString("t")}{(this.Model.SignInTime.Date != DateTime.Now.Date ? " (" + this.Model.SignInTime.Humanize() + ") " : String.Empty)}";

		public string AdditionalInformation
			=> $"[ {(this.Model.IsVisitor ? "visitor" : "member")} ]{(this.Model.TokenId != null ? " [ signed-in with fob ]" : String.Empty)}";

		internal string TokenId
			=> this.Model.TokenId;

		public SignedInRecord Model
		{
			get;
		}
	}
}
