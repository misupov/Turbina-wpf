namespace Turbina.Nodes
{
    public class StringConcatNode : Node
    {
        [Input]
        public string Left { get; set; }

        [Input]
        public string Right { get; set; }

        [Output]
        public string Value { get; private set; }

        protected override void Process(ProcessingContext context)
        {
            Value = Left + Right;
        }
    }
}