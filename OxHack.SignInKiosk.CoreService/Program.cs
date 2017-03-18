using NLog;
using NLog.Config;
using NLog.Targets;
using OxHack.SignInKiosk.CoreService.SubServices;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Messaging;
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
					var messagingClient =
						new MessagingClient(subscriptions:
							new[] {
								typeof(SignInRequestSubmitted),
								typeof(SignOutRequestSubmitted),
								typeof(PersonSignedIn),
								typeof(PersonSignedOut)
							});

					serviceConf.ConstructUsing(
						name => new Bootstrapper(
							new SignInEventProcessor(
								messagingClient,
								new SignInService(new SqlDbConfig()),
								new TokenHolderService(new SqlDbConfig())),
								new OffsiteStorageService(new SignInService(new SqlDbConfig()), messagingClient)
						));
					serviceConf.WhenStarted(async target => await target.Start());
					serviceConf.WhenStopped(async target => await target.Stop());
				});
				hostConf.DependsOn("LanmanServer");
				hostConf.RunAsLocalSystem();
				hostConf.StartAutomaticallyDelayed();
				hostConf.EnableServiceRecovery(recovery =>
					recovery
						.RestartService(delayInMinutes: 1)
						.SetResetPeriod(days: 1)
						.OnCrashOnly());

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
