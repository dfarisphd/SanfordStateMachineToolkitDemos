using System;
using System.Threading;
using StateMachineToolkit;

namespace DelegateSchedulerTester
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
        public delegate void DoSomethingDelegate(int arg);

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            DelegateScheduler ds = new DelegateScheduler();

            ds.Add(60, 100, new DoSomethingDelegate(DoSomething), 100);
            ds.Add(DelegateScheduler.Infinity, 250, new DoSomethingDelegate(DoSomething), 250);
            ds.Add(DelegateScheduler.Infinity, 333, new DoSomethingDelegate(DoSomething), 333);

            ds.Start();

            Thread.Sleep(5000);

            ds.Add(10, 1000, new DoSomethingDelegate(DoSomething), 1000);

            Thread.Sleep(60000);

            ds.Stop();

            ds.Dispose();
		}

        static void DoSomething(int arg)
        {
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(arg);
        }
	}
}
