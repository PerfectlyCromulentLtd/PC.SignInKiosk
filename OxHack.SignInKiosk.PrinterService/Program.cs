using NLog;
using NLog.Config;
using NLog.Targets;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace OxHack.SignInKiosk.PrinterService
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

				hostConf.SetDescription($"Listens for {nameof(PersonSignedIn)} and Sign-out messages and logs them to paper using the receipt printer.");
				hostConf.SetDisplayName("SignInKiosk.PrinterService");
				hostConf.SetServiceName("SignInKiosk.PrinterService");
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
