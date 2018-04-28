using System;
using System.Threading;
using UnityEngine;

namespace Arduino
{
	public class ArduinoManager
	{
		private int _pointsInGame;
		private int _ledsPerTeam;
		private ArduinoConnector _connector;
		private float _percentagePerPoint;

		private float _team1Percentage, _team2Percentage;
		private int _team1Points, _team2Points;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:LedBarManager.LedBarManager"/> class.
		/// Default Baud rate 9600.
		/// </summary>
		/// <param name="portName">Port name.</param>
		public ArduinoManager(string portName)
		{
			_connector = new ArduinoConnector(portName);
			Setup ();
			/*Thread t = new Thread (Setup);
			t.Start();
			WaitForThread (t);*/
		}

		private void Setup()
		{
			_connector.Setup();
			while (!_connector.isReady()) { };
		}

		private void WaitForThread (Thread t)
		{
			while (!t.Join (TimeSpan.FromSeconds (5))) {
				t.Abort ();
				Debug.Log ("ASPETTO");
				t = new Thread (Setup);
				t.Start();
			}
		}

		public void WriteOnArduino(string command) {
			_connector.WriteOnArduino(command);
		}

		public string ReadFromArduino() {
			return _connector.ReadFromArduino ();
		}

		/// <summary>
		/// Starts the progress bar to display score.
		/// Default colors are used if not previously set.
		/// </summary>
		/// <param name="pointsInGame">Total number of points in the game.</param>
		public void StartProgressBar(int pointsInGame)
		{
			_pointsInGame = pointsInGame;
			_ledsPerTeam = (int)((_connector.GetLedNumber() / 2.0) + 0.5);
			_percentagePerPoint = (float)_ledsPerTeam / _pointsInGame;
			_team1Percentage = 0;
			_team2Percentage = 0;
			_team1Points = 0;
			_team2Points = 0;
			_connector.StartProgressBar();

		}

		/// <summary>
		/// Adds the point.
		/// </summary>
		/// <param name="team">Team (1 or 2)</param>
		/// <param name="number">Number of points to add (default 1)</param>
		public void AddPoint(int team, int number = 1)
		{

			float toAdd = _percentagePerPoint * number;
			int ledNumber;
			int newTeamPoints;
			switch (team)
			{
			case 1:
				_team1Percentage += toAdd;
				newTeamPoints = _team1Points + (int)_team1Percentage;
				newTeamPoints = newTeamPoints < _pointsInGame ? newTeamPoints : _pointsInGame;
				ledNumber = newTeamPoints - _team1Points;
				_team1Percentage = _team1Percentage - ledNumber;
				_connector.LightUpLed(ledNumber, 1);
				_team1Points = newTeamPoints;
				break;
			case 2:
				_team2Percentage += toAdd;
				newTeamPoints = _team2Points + (int)_team2Percentage;
				newTeamPoints = newTeamPoints < _pointsInGame ? newTeamPoints : _pointsInGame;
				ledNumber = newTeamPoints - _team2Points;
				_team2Percentage = _team2Percentage - ledNumber;
				_connector.LightUpLed(ledNumber, 2);
				_team2Points = newTeamPoints;
				break;
			default:
				throw new ArgumentOutOfRangeException("team", "player does not exist.");

			}
		}


		/// <summary>
		/// Sets the color of the team.
		/// </summary>
		/// <param name="team">Team.</param>
		/// <param name="hexColor">HEX color string.</param>
		public void SetTeamColor(int team, string hexColor)
		{
			if (team != 1 && team != 2)
			{
				throw new ArgumentOutOfRangeException("team", "player does not exist.");
			}

			_connector.SetPlayerColor(team, hexColor);
		}


		/// <summary>
		/// Resets the progress bar.
		/// If game is on it brings back score to 0.
		/// </summary>
		public void ResetProgressBar()
		{
			_connector.StartProgressBar();
		}

		/// <summary>
		/// Shows the effect.
		/// </summary>
		/// <param name="barPortion">Bar portion (0- first player, 1-second player, 2-entire bar).</param>
		/// <param name="effectNumber">Effect number.</param>
		/// <param name="duration">Duration in milliseconds. (default 5000ms)</param>
		public void ShowEffect(int barPortion, int effectNumber, int duration = 5000)
		{
			_connector.ShowEffect(barPortion, effectNumber, duration);
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void SwitchOff(){
			_connector.SwitchOff();
		}

		public void Destroy(){
			_connector.CloseConnector();
		}
	}
}