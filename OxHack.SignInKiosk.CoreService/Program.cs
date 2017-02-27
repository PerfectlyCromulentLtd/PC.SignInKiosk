using NLog;
using NLog.Config;
using NLog.Targets;
using OxHack.SignInKiosk.CoreService.SubServices;
using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace OxHack.SignInKiosk.CoreService
{
	class Program
	{
		static void Main(string[] args)
		{
			Program.ConfigureLogging();

			HostFactory.Run(hostConf =>
			{
				hostConf.UseNLog();

				hostConf.Service<Bootstrapper>(serviceConf =>
				{
					serviceConf.ConstructUsing(
						name => new Bootstrapper(
							new SignInEventProcessor(
								new MessagingClient(subscriptions: new[] { typeof(SignInRequestSubmitted), typeof(SignOutRequestSubmitted) }))));
					serviceConf.WhenStarted(async target => await target.Start());
					serviceConf.WhenStopped(async target => await target.Stop());
				});
				//hostConf.RunAsLocalSystem();

				hostConf.SetDescription("Implements the core business logic for the Sign-In Kiosk project.");
				hostConf.SetDisplayName("SignInKiosk.CoreService");
				hostConf.SetServiceName("SignInKiosk.CoreService");
			});
		}

		private static void ConfigureLogging()
		{
			var config = new LoggingConfiguration();

			var consoleTarget = new ConsoleTarget()
			{
				Name = "Console",
				Layout = "\n${date:format=HH\\:mm\\:ss} ${logger}\n\t${message}"
			};
			config.AddTarget(consoleTarget);

			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

			LogManager.Configuration = config;
		}
	}
}
