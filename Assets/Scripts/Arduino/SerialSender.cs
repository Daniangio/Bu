using System;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.IO.Ports;

namespace Arduino
{
	public class SerialSender
	{
		private string _portName;
		private int _baudRate;
		private SerialPort _stream;

		private Thread _thread;
		private Queue _outputQueue;
		private Queue _inputQueue;

		private bool _looping = true;

		public void StartThread(string portName, int baudRate = 9600)
		{
			_portName = portName;
			_baudRate = baudRate;
			_outputQueue = Queue.Synchronized (new Queue ());
			_inputQueue = Queue.Synchronized (new Queue ());

			_thread = new Thread (ThreadLoop);
			_thread.Start ();
		}

		public void StopThread ()
		{
			lock (this)
			{
				_looping = false;
			}
		}

		public bool IsLooping()
		{
			lock (this) {
				return _looping;
			}
		}

		public void ResetQueues()
		{
			_inputQueue.Clear ();
			_outputQueue.Clear ();
		}

		private void WriteToSerial(string command)
		{
			_stream.Write(command + ";");
			_stream.BaseStream.Flush();
		}

		private string ReadFromSerial(int timeout = 0)
		{
			_stream.ReadTimeout = timeout;
			try
			{
				return _stream.ReadTo(";");
			}
			catch (TimeoutException)
			{
				return null;
			}
		}

		public void SendToArduino (string command)
		{
			_outputQueue.Enqueue (command);
		}

		public string ReadFromArduino ()
		{
			if (_inputQueue.Count == 0)
				return null;

			return (string)_inputQueue.Dequeue ();
		}

		private void ThreadLoop ()
		{
			_stream = new SerialPort (_portName, _baudRate);
			_stream.ReadTimeout = 50;
			_stream.Open();
			while (IsLooping())
			{
				if (_outputQueue.Count != 0)
				{
					string command = (string)_outputQueue.Dequeue();
					WriteToSerial(command);
				}

				string result = ReadFromSerial(50);
				if (result != null)
					_inputQueue.Enqueue (result);
			}

			_stream.Close ();
		}




	}
}
