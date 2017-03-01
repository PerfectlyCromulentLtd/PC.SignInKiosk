﻿using NLog;
using OxHack.SignInKiosk.Domanin.Messages;
using OxHack.SignInKiosk.Messaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.PrinterService.SubServices
{
	class SignInEventPrinter
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly MessagingClient messagingClient;
		private Task spacerWorker;
		private DateTime nextTimeToWriteASpacer;
		private DateTime timeOfLastSpacerPrint;
		private readonly string printTextScript;
		private readonly string printSpacerScript;

		public SignInEventPrinter(MessagingClient messagingClient)
		{
			this.messagingClient = messagingClient;

			this.messagingClient.PersonSignedIn += this.OnPersonSignedIn;
			this.messagingClient.PersonSignedOut += this.OnPersonSignedOut;

			var isLinux = IsLinuxy();
			this.printTextScript = isLinux ? "linux-printText.sh" : "windows-printText.bat";
			this.printSpacerScript = isLinux ? "linux-printSpacer.sh" : "windows-printSpacer.bat";
		}

		private void InitialiseWorker()
		{
			this.nextTimeToWriteASpacer = DateTime.MinValue;
			this.timeOfLastSpacerPrint = DateTime.MinValue;
			this.spacerWorker = Task.Run(async () => await this.SpacerWorkerLoop());
		}

		private void KillWorker()
		{
			this.spacerWorker = null;
		}

		private void OnPersonSignedIn(object sender, PersonSignedIn message)
		{
			this.Print($"IN : {message.Person.DisplayName}");
		}

		private void OnPersonSignedOut(object sender, PersonSignedOut message)
		{
			this.Print($"OUT: {message.Person.DisplayName}");
		}

		private void Print(string text)
		{
			try
			{
				var now = DateTime.Now;
				string message =
					$"@{now.ToString("u").Replace("Z", String.Empty)}{System.Environment.NewLine}{text}";

				var target = new FileInfo("./printFiles/" + now.ToString("s").Replace(':', '_') + ".txt");
				target.Directory.Create();
				using (var writer = target.OpenWrite())
				{
					var bytes = Encoding.UTF8.GetBytes(message);
					writer.Write(bytes, 0, bytes.Length);
				}

				var printProcess = Process.Start(this.printTextScript, $"\"{target.FullName}\"");

				var hasExited = printProcess.WaitForExit(10000);

				if (hasExited && printProcess.ExitCode == 0)
				{
					this.nextTimeToWriteASpacer = DateTime.Now.AddSeconds(10);
				}
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private async Task SpacerWorkerLoop()
		{
			this.logger.Info($"Spacer worker started.  Waiting a few seconds before entering loop.");

			await Task.Delay(TimeSpan.FromSeconds(3));

			while (this.spacerWorker != null)
			{
				if (this.nextTimeToWriteASpacer > this.timeOfLastSpacerPrint)
				{
					var targetTime = this.nextTimeToWriteASpacer;
					var delayDuration = TimeSpan.FromSeconds(Math.Max(0, (targetTime - DateTime.Now).TotalSeconds));
					this.logger.Info($"Spacer timer reset.  Printing spacer in {delayDuration.TotalSeconds} seconds...");
					await Task.Delay(delayDuration);

					if (this.nextTimeToWriteASpacer == targetTime)
					{
						this.timeOfLastSpacerPrint = targetTime;
						Process.Start(this.printSpacerScript);
					}
				}
				else
				{
					await Task.Delay(1000);
				}
			}
		}

		private static bool IsLinuxy()
		{
			bool isLinuxy = false;

			int platformId = (int)Environment.OSVersion.Platform;
			if (platformId == 4 || platformId == 6 || platformId == 128)
			{
				isLinuxy = true;
			}

			return isLinuxy;
		}

		public async Task Start()
		{
			this.InitialiseWorker();
			await this.messagingClient.Connect();
		}

		public async Task Stop()
		{
			this.KillWorker();
			await this.messagingClient.Disconnect();
		}
	}
}
