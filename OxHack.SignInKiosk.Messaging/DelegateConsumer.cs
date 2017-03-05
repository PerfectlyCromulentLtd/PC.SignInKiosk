using MassTransit;
using NLog;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	public class DelegateConsumer<T> : IConsumer<T> where T : class
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly Action<T> action;

		public DelegateConsumer(Action<T> action)
		{
			this.action = action;
		}

		public async Task Consume(ConsumeContext<T> context)
		{
			try
			{
				this.action(context.Message);
				await Task.FromResult(0);
			}
			catch (Exception e)
			{
				this.logger.Error(e);
				throw;
			}
		}
	}
}