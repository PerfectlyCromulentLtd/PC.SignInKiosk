using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace OxHack.SignInKiosk.MessageBrokerProxyService
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
				hostConf.RunAsLocalSystem();

				hostConf.SetDescription("A WCF service the Tablet can use in order to receiver and publish messages to the RabbitMQ message broker.");
				hostConf.SetDisplayName("SignInKiosk.MessageBrokerProxy");
				hostConf.SetServiceName("SignInKiosk.MessageBrokerProxy");
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
