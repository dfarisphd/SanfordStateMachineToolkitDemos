using System;
using System.Threading;
using StateMachineToolkit;

namespace LightSwitchDemo
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			LightSwitch ls = new LightSwitch();

            ls.TurnOn();
            ls.TurnOff();
            ls.Dispose();

            Console.Read();
		}
	}
}
