using System;
using System.Data;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Xml.Serialization;
using StateMachineToolkit;

namespace StateMachineBuilderDemo
{
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                StateMachineBuilder builder = new StateMachineBuilder();

                builder.NamespaceName = "StateMachineDemo";
                builder.StateMachineName = "TrafficLightBase";
                builder.InitialState = "Off";

                int index = builder.States.Add("Off");
                builder.States[index].Transitions.Add("TurnOn", null, null, "On");

                index = builder.States.Add("On", "Red", HistoryType.Shallow);
                builder.States[index].Transitions.Add("TurnOff", null, null, "Off");

                StateRowCollection substates = builder.States[index].Substates;

                index = substates.Add("Red");
                substates[index].Transitions.Add("TimerElapsed", "CounterEquals4", "ResetCounter", "Green");                
                substates[index].Transitions.Add("TimerElapsed", null, "IncrementCounter", null);

                index = substates.Add("Yellow");
                substates[index].Transitions.Add("TimerElapsed", "CounterEquals2", "ResetCounter", "Red");                
                substates[index].Transitions.Add("TimerElapsed", null, "IncrementCounter", null);

                index = substates.Add("Green");
                substates[index].Transitions.Add("TimerElapsed", "CounterEquals4", "ResetCounter", "Yellow");                
                substates[index].Transitions.Add("TimerElapsed", null, "IncrementCounter", null);
                
                builder.Build();
                
                StringWriter writer = new StringWriter();

                CodeDomProvider provider = new CSharpCodeProvider();
                ICodeGenerator generator = provider.CreateGenerator();
                CodeGeneratorOptions options = new CodeGeneratorOptions();

                options.BracingStyle = "C";

                generator.GenerateCodeFromNamespace(builder.Result, writer, options);

                XmlSerializer serializer = new XmlSerializer(typeof(StateMachineBuilder));
                serializer.Serialize(writer, builder);
                Console.WriteLine(writer.ToString());

                writer.Close();

                Console.Read();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}