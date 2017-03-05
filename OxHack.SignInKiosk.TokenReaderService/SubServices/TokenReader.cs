using NLog;
using OxHack.SignInKiosk.TokenReaderService.Events;
using Prism.Events;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService.SubServices
{
	class TokenReader
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly IEventAggregator eventAggregator;
		private BlockingCollection<Action> transmitQueue;
		private readonly AutoResetEvent canTransmit;

		private Task transmitWorker;
		private Task receiveWorker;
		private Task pollingWorker;

		private readonly InputPinConfiguration busyPin;
		private GpioConnection piConnection;
		private readonly SerialPort serialPort;

		private readonly byte[] factoryResetCommand = { 0x46, 0x55, 0xAA };
		private readonly byte[] getProductAndFirmwareCommand = { 0x7A };
		private readonly byte[] setHiTag2ModeCommand = { 0x76, 0x01 };
		private readonly byte[] setEM400xModeCommand = { 0x76, 0x03 };
		private readonly byte[] readHiTag2UidCommand = { 0x52, 0x00 };
		private readonly byte[] readHiTag2ConfigurationCommand = { 0x52, 0x03 };
		private readonly byte[] readCardUidCommand = { 0x55 };
		private const byte WriteEepromCommand = 0x50;

		private const int HiTag2PasswordEepromAddress = 0x08;
		private const int EnableUidEepromAddress = 0x0C;

		private const byte TokenReadMask = 0xC6;

		public TokenReader(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;

			this.canTransmit = new AutoResetEvent(false);

			this.busyPin = ConnectorPin.P1Pin12.Input();

			this.serialPort = new SerialPort("/dev/ttyAMA0", 9600, Parity.None, 8, StopBits.One);
		}

		public async Task Start()
		{
			this.logger.Debug("Starting...");

			this.transmitQueue = new BlockingCollection<Action>();

			this.serialPort.Open();
			this.piConnection =
				new GpioConnection(
					new GpioConnectionSettings()
					{
						PollInterval = TimeSpan.FromMilliseconds(5)
					},
					this.busyPin);
			this.piConnection.PinStatusChanged += this.OnPinStatusChanged;

			this.receiveWorker = Task.Run(() => this.ReceiveWorkerLoop());
			this.pollingWorker = Task.Run(() => this.PollingWorkerLoop());
			this.transmitWorker = Task.Run(() => this.TransmitWorkerLoop());

			await Task.FromResult(0);
			this.logger.Debug("... Started.");
		}

		public async Task Stop()
		{
			this.logger.Debug("Stopping...");

			this.transmitQueue.CompleteAdding();
			this.transmitWorker = null;

			this.busyPin.OnStatusChanged(null);
			this.piConnection.PinStatusChanged -= this.OnPinStatusChanged;
			this.piConnection.Close();
			this.serialPort.Close();

			await Task.FromResult(0);
			this.logger.Debug("... Stopped.");
		}

		private void OnPinStatusChanged(object sender, PinStatusEventArgs e)
		{
			if (e.Configuration == this.busyPin)
			{
				if (!e.Enabled)
				{
					this.canTransmit.Set();
				}
			}
		}

		private async Task PollingWorkerLoop()
		{
			await Task.Delay(2000);

			//for (int i = 0; i < 4; i++)
			//{
			//	this.FactoryReset();
			//	this.EnableUID();
			//	this.SetHiTag2Mode();
			//	//this.SetEM400xMode();
			//	this.EnableUID();
			//}

			while (true)
			{
				await Task.Delay(50);

				if (this.transmitQueue.Count < 3)
				{
					//this.ReadHiTag2Uid();
					this.ReadCardUid();
				}
			}
		}

		private void TransmitWorkerLoop()
		{
			//this.logger.Debug("Transmit worker loop started.");

			foreach (var transmission in this.transmitQueue.GetConsumingEnumerable())
			{
				this.canTransmit.WaitOne();
				//this.logger.Debug("Cleared to transmit.  Transmitting...");

				transmission();
			}
		}

		private void ReceiveWorkerLoop()
		{
			//this.logger.Debug("Receive worker loop started.");

			var receiveBuffer = new byte[8];
			while (true)
			{
				Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
				try
				{
					this.serialPort.Read(receiveBuffer, 0, 8);
					var ack = receiveBuffer[0];
					var wasTokenRead = (ack & TokenReader.TokenReadMask) == TokenReader.TokenReadMask;

					uint tokenId = 0;

					if (wasTokenRead)
					{
						tokenId = BitConverter.ToUInt32(receiveBuffer, 1);

						//this.logger.Debug($"ACK: 0x{ack.ToString("X2")} {TokenReader.ToBitString(ack)} TokenId: {tokenId}");
					}

					this.eventAggregator.GetEvent<TokenReadEvent>().Publish(tokenId);

					//if (!wasTokenRead && ack != 0xC0)
					//{
					//	this.logger.Debug($">>>>>> Received {String.Join(", ", receiveBuffer.Select(item => $"{TokenReader.ToBitString(item)} (0x{item.ToString("X2")})"))}");
					//}
				}
				catch (Exception e)
				{
					this.logger.Error($"Error reading from serial port: {e}");
				}
			}
		}

		private void ReadHiTag2Configuration()
		{
			this.transmitQueue.Add(() => this.Transmit(this.readHiTag2ConfigurationCommand));
		}

		private void ReadHiTag2Uid()
		{
			this.transmitQueue.Add(() => this.Transmit(this.readHiTag2UidCommand));
		}

		private void ReadCardUid()
		{
			this.transmitQueue.Add(() => this.Transmit(this.readCardUidCommand));
		}

		private void FactoryReset()
		{
			this.logger.Debug("Performing Factory Reset.");
			this.transmitQueue.Add(() => this.Transmit(this.factoryResetCommand));
		}

		private void SetHiTag2Mode()
		{
			this.logger.Debug("Setting HiTag2 Mode.");

			var password = new byte[] { 0x21, 0xC3, 0x53, 0x83 };
			//var password = new byte[] { 0x4D, 0x39, 0x4B, 0x52 };
			//var password = new byte[] { 0xC9, 0x00, 0x00, 0xAA };
			//var password = new byte[] { 0x4D, 0x49, 0x4B, 0x52 };

			this.SetHiTag2Password(password);
			//this.SetHiTag2Password("MIKR");

			this.transmitQueue.Add(() => this.Transmit(this.setHiTag2ModeCommand));
		}

		private void SetHiTag2Password(string password)
		{
			var data = Encoding.ASCII.GetBytes(password ?? String.Empty).Take(4).ToArray();
			this.SetHiTag2Password(data);
		}

		private void SetHiTag2Password(byte[] data)
		{
			this.logger.Debug("Setting HiTag2 password.");
			this.WriteEeprom(TokenReader.HiTag2PasswordEepromAddress, data);
		}

		private void SetEM400xMode()
		{
			this.logger.Debug("Setting EM400X Mode.");
			this.transmitQueue.Add(() => this.Transmit(this.setEM400xModeCommand));
		}

		private void EnableUID()
		{
			this.logger.Debug("Enabling UID reading.");
			byte[] data = { 0xFF, 0xFF, 0xFF, 0xFF };
			this.WriteEeprom(TokenReader.EnableUidEepromAddress, data);
		}

		private void Transmit(byte[] payload)
		{
			//var message = String.Join(", ", payload.Select(item => $"{TokenReader.ToBitString(item)} ({item})"));
			//this.logger.Debug($"Transmitting {message}.");

			this.serialPort.Write(payload, 0, payload.Length);
		}

		private void WriteEeprom(int startAddress, byte[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				byte[] payload = { TokenReader.WriteEepromCommand, (byte)(startAddress + i), data[i] };
				this.transmitQueue.Add(() => this.Transmit(payload));
			}
		}

		private static string ToBitString(byte input)
			=> new string(Enumerable.Range(0, 8).Reverse().Select(i => (int)Math.Pow(2, i)).Select(i => (input & i) != 0 ? '1' : '0').ToArray());
	}
}
