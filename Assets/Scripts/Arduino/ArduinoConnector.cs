using System;
using System.IO.Ports;
using System.Collections;
using UnityEngine;

namespace Arduino
{
	public class ArduinoConnector 
	{
		private string _portName;
		private int _baudRate = 9600;
		private SerialSender _sender;

		public ArduinoConnector (string portName)
		{
			_portName = portName;
		}

		public void Setup()
		{
			_sender = new SerialSender ();
			_sender.StartThread (_portName, _baudRate);

		}


		// ---------------------------------
		// ---       Command Sender     ----
		// ---------------------------------

		public void WriteOnArduino(string command)
		{
			_sender.SendToArduino (command);
		}

		public string ReadFromArduino()
		{
			string response = "";
			string newResponse = _sender.ReadFromArduino ();
			while (newResponse != null) {
				newResponse = ((char)int.Parse (newResponse)).ToString();
				response = response + newResponse;
				newResponse = _sender.ReadFromArduino ();
			}
			if (response != "")
				return response.Substring (0, response.Length - 1);
			return null;
		}

		/// <summary>
		/// Gets the number of leds available to effectively store the points.
		/// </summary>
		/// <returns>The led number.</returns>
		public int GetLedNumber()
		{

			string message = 2.ToString();
			_sender.SendToArduino(message);
			string response = null;
			while (response == null || !response.StartsWith ("3")) {
				response = _sender.ReadFromArduino ();
			}
			int result = int.Parse (response.Split ("," [0]) [1]);
			return result;
		}


		/// <summary>
		/// Request to light a given number of leds for a given player.
		/// </summary>
		/// <param name="ledNumber">Led number.</param>
		/// <param name="playerNumber">Player number (1 or 2)</param>
		public void LightUpLed(int ledNumber, int playerNumber)
		{
			if (ledNumber < 0)
				throw new ArgumentOutOfRangeException("ledNumber", "negative led count");
			if (playerNumber != 1 && playerNumber != 2)
				throw new ArgumentOutOfRangeException("playerNumber", "player does not exists");

			string command = 4.ToString();
			command += "," + ledNumber.ToString ();
			command += "," + playerNumber.ToString ();
			_sender.SendToArduino (command);
		}

		/// <summary>
		/// Sets the color of the player.
		/// </summary>
		/// <param name="playerNumber">Player number.</param>
		/// <param name="color">HEX string of the color.</param>
		public void SetPlayerColor(int playerNumber, string color)
		{
			if (playerNumber != 1 && playerNumber != 2)
				throw new ArgumentOutOfRangeException("playerNumber", "player does not exists");
			if (color.Length != 6)
				throw new ArgumentException("color", "not a valid hex color");

			string command = 5.ToString();
			command += "," + playerNumber.ToString ();
			command += "," + color;
			_sender.SendToArduino (command);

		}

		/// <summary>
		/// Starts the progress bar to display teams score.
		/// Uses default color if SetPlayerColor hasn't been called.
		/// </summary>
		public void StartProgressBar()
		{
			_sender.ResetQueues ();
			_sender.SendToArduino (6.ToString());
		}

		/// <summary>
		/// Shows the effect.
		/// </summary>
		/// <param name="barPortion">Bar portion (0- first player, 1-second player, 3-entire bar)</param>
		/// <param name="effect">Effect number.</param>
		/// <param name="duration">Duration.</param>
		public void ShowEffect(int barPortion, int effect, int duration)
		{
			string command = 7.ToString();
			command += "," + barPortion.ToString ();
			command += "," + effect.ToString ();
			command += "," + duration.ToString ();
			_sender.SendToArduino (command);
		}


		/// <summary>
		/// Checks if Arduino is ready
		/// </summary>
		public bool isReady()
		{
			return true;
			/*string status = null;
			while(status == null)
			{
				status = _sender.ReadFromArduino ();
			}
			if (status.StartsWith ("0"))
				return true;
			
			return false;*/
		}

		/// <summary>
		/// Switch off everything.
		/// </summary>
		public void SwitchOff()
		{
			_sender.SendToArduino (9.ToString());
		}

		public void CloseConnector()
		{
			_sender.StopThread ();
		}


	}
}
