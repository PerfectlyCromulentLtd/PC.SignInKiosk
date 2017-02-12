using MassTransit;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	internal class DelegateConsumer<T> : IConsumer<T> where T : class
	{
		private readonly Action<T> action;

		public DelegateConsumer(Action<T> action)
		{
			this.action = action;
		}

		public async Task Consume(ConsumeContext<T> context)
		{
			this.action(context.Message);
		}
	}
}