namespace Turbina.Nodes
{
    public class StringConstNode : Node
    {
        [Output]
        [Input]
        public string Value { get; set; }

        protected override void Process(ProcessingContext context)
        {
        }
    }
}