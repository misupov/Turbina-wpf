using System;
using System.Threading;

namespace Turbina.Nodes
{
    public class ConsoleOutputNode : Node
    {
        [Input]
        public object Input { get; set; }

        [Output]
        public string Output { get; set; }

        protected override void Process(ProcessingContext context)
        {
            Output = Input?.ToString();
            Console.Out.WriteLine(Output);
        }
    }
}