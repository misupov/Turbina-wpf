using System;
using System.Diagnostics;

namespace Turbina.Nodes
{
    public class RandomNode : Node
    {
        private readonly Random _random = new Random();
        private int I;

        [Input]
        public object Trigger { get; set; }

        [Output]
        public string Value { get; private set; }

        protected override void Process(ProcessingContext context)
        {
            Debug.WriteLine(I++);
            Value = _random.Next().ToString();
        }
    }
}