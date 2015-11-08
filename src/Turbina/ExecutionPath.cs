using System.Collections.Generic;

namespace Turbina
{
    internal class ExecutionPath
    {
        public ExecutionPath(Node[] topologicallySortedNodes)
        {
            TopologicallySortedNodes = topologicallySortedNodes;
            Nodes = new HashSet<Node>(topologicallySortedNodes, ReferenceEqualityComparer<Node>.Instance);
        }

        public Node[] TopologicallySortedNodes { get; }

        public HashSet<Node> Nodes { get; }
    }
}