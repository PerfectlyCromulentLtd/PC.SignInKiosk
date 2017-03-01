﻿using Caliburn.Micro;
using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using OxHack.SignInKiosk.Views;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk
{
	public sealed partial class App
	{
		private WinRTContainer container;

		public App()
		{
			this.InitializeComponent();
			this.Suspending += this.SuspendedHandler;
		}

		protected override void Configure()
		{
			base.Configure();

			this.container = new WinRTContainer();
			this.container.RegisterWinRTServices();

			this.container.PerRequest<StartViewModel>();
			this.container.PerRequest<NameEntryViewModel>();
			this.container.PerRequest<SignedInGreetingViewModel>();
			this.container.PerRequest<ManualSignOutViewModel>();

			this.container.Singleton<MessageBrokerService>();
			this.container.Singleton<MessageOrchestratorService>();
			this.container.Singleton<TokenHolderInfoService>();
			this.container.Singleton<SignInService>();
			this.container.PerRequest<ToastService>();

			// TODO: Move this hardcoded URI to somesort of configuration file
			this.container.RegisterHandler(typeof(SignInApiWrapper), null, c => new SignInApiWrapper(new Uri("http://SignInKioskWebApi:5001")));

			this.container.RegisterHandler(typeof(ToastNotifier), null, c => ToastNotificationManager.CreateToastNotifier());
		}

		protected override object GetInstance(Type service, string key)
		{
			var instance = this.container.GetInstance(service, key);

			if (instance == null)
			{
				throw new InvalidOperationException($"Could not locate any instances of '{service.Name}' with key '{ key }'.");
			}

			return instance;
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return this.container.GetAllInstances(service);
		}

		protected override void BuildUp(object instance)
		{
			this.container.BuildUp(instance);
		}

		protected override void PrepareViewFirst(Frame rootFrame)
		{
			this.container.RegisterNavigationService(rootFrame);
		}

		protected async override void OnLaunched(LaunchActivatedEventArgs e)
		{
			//#if DEBUG
			//			if (System.Diagnostics.Debugger.IsAttached)
			//			{
			//				this.DebugSettings.EnableFrameRateCounter = true;
			//			}
			//#endif

			if (e.PreviousExecutionState != ApplicationExecutionState.Running)
			{
				this.DisplayRootView<StartView>();
				await this.EnsureSingletonsAreRunning();
			}
		}

		private async Task EnsureSingletonsAreRunning()
		{
			var messageBrokerService = this.container.GetInstance<MessageBrokerService>();

			try
			{
				await messageBrokerService.Connect();
			}
			catch (Exception e)
			{
				// TODO: log error
			}

			this.container.GetInstance<MessageOrchestratorService>();
			this.container.GetInstance<TokenHolderInfoService>();
		}

		private void SuspendedHandler(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();

			// TODO: Save application state and stop any background activity
			deferral.Complete();
		}
	}
}