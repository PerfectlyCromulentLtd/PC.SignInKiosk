using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Collections.Concurrent;
using Raspberry.IO.GeneralPurpose;
using NLog;
using System.Threading;
using System.IO.Ports;

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
		private Task dummyWorker;

		private readonly OutputPinConfiguration transmitPin;
		private readonly InputPinConfiguration receivePin;
		private readonly InputPinConfiguration busyPin;
		private GpioConnection piConnection;
		private readonly SerialPort serialPort;

		private readonly byte[] getProductAndFirmwareCommand = { 0x7A };
		private readonly byte[] setHiTag2ModeCommand = { 0x76, 0x01 };
		private readonly byte[] readHiTag2UidCommand = { 0x52, 0x00 };
		//private readonly byte[] readHiTag2UidCommand = { 0x55 };

		public TokenReader(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.canTransmit = new AutoResetEvent(false);

			//this.transmitPin = ConnectorPin.P1Pin08.Output();
			//this.receivePin = ConnectorPin.P1Pin10.Input();
			this.busyPin = ConnectorPin.P1Pin12.Input();
			this.busyPin.OnStatusChanged(this.OnBusyChanged);

			this.serialPort = new SerialPort("/dev/ttyAMA0", 9600, Parity.None, 8, StopBits.One);
			this.serialPort.DataReceived += (s, e) => this.logger.Debug(":) Data received over serial port :)");
			this.serialPort.ErrorReceived += (s, e) => this.logger.Error("-! Serial port error: " + Enum.GetName(typeof(SerialError), e.EventType));
		}

		public async Task Start()
		{
			this.logger.Debug("Starting...");

			this.transmitQueue = new BlockingCollection<Action>();
			//this.transmitQueue.Add(() => this.Transmit(this.setHiTag2ModeCommand));

			this.serialPort.Open();

			this.piConnection =
				new GpioConnection(
					//this.transmitPin,
					//this.receivePin,
					this.busyPin
					);

			this.transmitWorker = Task.Run(() => this.TransmitWorkerLoop());
			this.receiveWorker = Task.Run(() => this.ReceiveWorkerLopp());
			this.dummyWorker = Task.Run(() => this.DummyWorkerLoop());

			await Task.FromResult(0);
			this.logger.Debug("... Started.");
		}

		public async Task Stop()
		{
			this.logger.Debug("Stopping...");

			this.transmitQueue.CompleteAdding();
			this.piConnection.Close();
			this.transmitWorker = null;

			this.serialPort.Close();

			await Task.FromResult(0);
			this.logger.Debug("... Stopped.");
		}

		private void OnBusyChanged(bool isBusy)
		{
			//this.logger.Debug($"Is Busy={isBusy}");

			if (!isBusy)
			{
				this.canTransmit.Set();
			}
		}

		private async Task DummyWorkerLoop()
		{
			this.logger.Debug("Dummy worker loop started.");
			while (true)
			{
				await Task.Delay(300);

				this.transmitQueue.Add(() => this.Transmit(this.readHiTag2UidCommand));

				//this.logger.Debug("Queued up dummy command.");
			}
		}

		private void Transmit(byte[] payload)
		{
			//var message = String.Join(", ", payload.Select(item => $"{TokenReader.ToBitString(item)} ({item})"));
			//this.logger.Debug($"Transmitting {message}.");

			this.serialPort.Write(payload, 0, payload.Length);
		}

		private void TransmitWorkerLoop()
		{
			this.logger.Debug("Transmit worker loop started.");

			foreach (var transmission in this.transmitQueue.GetConsumingEnumerable())
			{
				this.canTransmit.WaitOne();
				//this.logger.Debug("Cleared to transmit.  Transmitting...");

				transmission();
			}
		}

		private void ReceiveWorkerLopp()
		{
			this.logger.Debug("Receive worker loop started.");

			while (true)
			{
				var receiveBuffer = new byte[8];
				try
				{
					//byte ack = (byte)this.serialPort.ReadByte();
					//this.logger.Debug($">>>>>> Received byte {TokenReader.ToBitString(ack)} ({ack})");

					this.serialPort.Read(receiveBuffer, 0, 8);
					this.logger.Debug($">>>>>> Received {String.Join(", ", receiveBuffer.Select(item => $"{TokenReader.ToBitString(item)} (0x{item.ToString("X2")})"))}");

					//this.serialPort.Read(receiveBuffer, 0, 8);
					//this.logger.Debug("Received: " + Encoding.ASCII.GetString(receiveBuffer));
				}
				catch (Exception e)
				{
					this.logger.Error($"Error reading from serial port: {e}");
				}
			}
		}

		private static string ToBitString(byte input)
		{
			return
				new string(Enumerable.Range(0, 8).Reverse().Select(i => (int)Math.Pow(2, i)).Select(i => (input & i) != 0 ? '1' : '0').ToArray());
		}
	}
}
