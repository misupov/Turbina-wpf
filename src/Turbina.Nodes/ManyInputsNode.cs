namespace Turbina.Nodes
{
    public class ManyInputsNode : Node
    {
        [Input]
        public object In0 { get; set; }

        [Input]
        public object In1 { get; set; }

        [Input]
        public object In2 { get; set; }

        [Input]
        public object In3 { get; set; }

        [Input]
        public object In4 { get; set; }

        [Input]
        public object In5 { get; set; }

        [Input]
        public object In6 { get; set; }

        [Input]
        public object In7 { get; set; }

        [Input]
        public object In8 { get; set; }

        [Input]
        public object In9 { get; set; }

        [Output]
        public string Value { get; private set; }

        protected override void Process(ProcessingContext context)
        {
            Value = $"{In0}{In1}{In2}{In3}{In4}{In5}{In6}{In7}{In8}{In9}";
        }
    }
}