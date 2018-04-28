using System;

namespace Arduino
{
	public enum Commands
	{
		Acknowledge=0,
		Error=1,
		LedNumberRequest=2,
		LedNumberResponse=3,
		LightUpLed=4,
		SetPlayerColor=5,
		StartProgressBar=6,
		ShowEffect=7,
		GetStatus=8,
		SwitchOff=9,
	};

}