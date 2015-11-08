using System.Collections.Generic;

namespace Turbina
{
    public class ProcessingContext
    {
        public bool DoNotPulseFurther { get; set; }

        internal IEnumerable<Node> TopologicallySortedNodes => ExecutionPath.TopologicallySortedNodes;

        internal ExecutionPath ExecutionPath { get; }

        internal HashSet<Node> DeadNodes = new HashSet<Node>(ReferenceEqualityComparer<Node>.Instance);

        internal ProcessingContext(ExecutionPath executionPath)
        {
            ExecutionPath = executionPath;
        }
    }
}