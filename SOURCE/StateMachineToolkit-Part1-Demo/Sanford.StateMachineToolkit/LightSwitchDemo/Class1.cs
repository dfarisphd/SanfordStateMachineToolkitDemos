using System;
using System.Threading;
using Sanford.StateMachineToolkit;

namespace LightSwitchDemo
{
	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
			LightSwitch ls = new LightSwitch();

            ls.TransitionCompleted += new TransitionCompletedEventHandler(HandleTransitionCompleted);

            ls.Send((int)LightSwitch.EventID.TurnOn);
            ls.Send((int)LightSwitch.EventID.TurnOff);
            ls.Send((int)LightSwitch.EventID.TurnOn);
            ls.Send((int)LightSwitch.EventID.TurnOff);
       //     ls.Execute();

            Console.Read();
        }

        private static void HandleTransitionCompleted(object sender, TransitionCompletedEventArgs e)
        {
            Console.WriteLine("Transition Completed:");
            Console.WriteLine("\tState ID: {0}", ((LightSwitch.StateID)(e.StateID)).ToString());
            Console.WriteLine("\tEvent ID: {0}", ((LightSwitch.EventID)(e.EventID)).ToString());

            if(e.Error != null)
            {
                Console.WriteLine("\tException: {0}", e.Error.Message);
            }
            else
            {
                Console.WriteLine("\tException: No exception was thrown.");
            }

            if(e.ActionResult != null)
            {
                Console.WriteLine("\tAction Result: {0}", e.ActionResult.ToString());
            }
            else
            {
                Console.WriteLine("\tAction Result: No action result.");
            }
        }
    }
}
