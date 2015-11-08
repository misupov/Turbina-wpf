namespace Turbina.Nodes
{
    public class AddDoublesNode : Node
    {
        [Input]
        public double Left { get; set; }

        [Input]
        public double Right { get; set; }

        [Output]
        public double Result { get; set; }

        protected override void Process(ProcessingContext context)
        {
            Result = Left + Right;
        }
    }
}