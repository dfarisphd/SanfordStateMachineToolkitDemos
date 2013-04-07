using System;

namespace TestExample
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
            Example e = new Example();

            Console.WriteLine();
            e.SendA();      
      
            Console.WriteLine();
            e.SendE();  

            Console.WriteLine();
            e.SendE(); 

            Console.WriteLine();
            e.SendA();

            Console.WriteLine();
            e.SendH();

            Console.WriteLine();
            e.SendH();

            e.Dispose();

            Console.Read();
		}
	}
}
