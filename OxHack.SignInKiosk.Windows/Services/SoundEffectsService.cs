using Caliburn.Micro;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Events;
using System;
using Windows.UI.Xaml.Controls;

namespace OxHack.SignInKiosk.Services
{
	class SoundEffectsService :
		IHandle<PersonSignedIn>,
		IHandle<PersonSignedOut>,
		IHandle<Disconnected>,
		IHandle<Connected>,
		IHandle<TokenRead>
	{
		private readonly Uri tokenReadSound = new Uri("ms-appx:///Assets/TokenRead.wav");
		private readonly Uri connectedSound = new Uri("ms-appx:///Assets/Connected.wav");
		private readonly Uri disconnectedSound = new Uri("ms-appx:///Assets/Disconnected.wav");
		private readonly Uri signedInSound = new Uri("ms-appx:///Assets/SignedIn.wav");
		private readonly Uri signedOutSound = new Uri("ms-appx:///Assets/SignedOut.wav");

		public SoundEffectsService(IEventAggregator eventAggregator)
		{
			this.TokenReadSoundPlayer = new MediaElement()
			{
				AutoPlay = false,
				Source = this.tokenReadSound
			};

			this.NavigationSoundPlayer = new MediaElement()
			{
				AutoPlay = true
			};

			eventAggregator.Subscribe(this);
		}

		public void Handle(TokenRead message)
		{
			this.TokenReadSoundPlayer.Play();
		}

		public void Handle(Connected message)
		{
			this.NavigationSoundPlayer.Source = this.connectedSound;
		}

		public void Handle(Disconnected message)
		{
			this.NavigationSoundPlayer.Source = this.disconnectedSound;
		}

		public void Handle(PersonSignedIn message)
		{
			this.NavigationSoundPlayer.Source = this.signedInSound;
		}

		public void Handle(PersonSignedOut message)
		{
			this.NavigationSoundPlayer.Source = this.signedOutSound;
		}

		public MediaElement TokenReadSoundPlayer
		{
			get;
		}

		public MediaElement NavigationSoundPlayer
		{
			get;
		}
	}
}