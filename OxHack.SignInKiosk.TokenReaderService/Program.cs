using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace OxHack.SignInKiosk.TokenReaderService
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
					serviceConf.ConstructUsing(name => new Bootstrapper());
					serviceConf.WhenStarted(async target => await target.Start());
					serviceConf.WhenStopped(async target => await target.Stop());
				});
				//hostConf.RunAsLocalSystem();

				hostConf.SetDescription("Translates RFID token reads into RabbitMQ \"TokenRead\" messages.");
				hostConf.SetDisplayName("SignInKioskTokenReaderService");
				hostConf.SetServiceName("SignInKioskTokenReaderService");
			});
		}

		private static void ConfigureLogging()
		{
			var config = new LoggingConfiguration();

			var consoleTarget = new ConsoleTarget()
			{
				Name = "Console",
				Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}"
			};
			config.AddTarget(consoleTarget);

			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

			LogManager.Configuration = config;
		}
	}
}
