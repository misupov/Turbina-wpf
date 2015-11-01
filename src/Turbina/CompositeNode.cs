using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Turbina
{
    public class CompositeNode : Node
    {
        private readonly BidirectionalGraph<Node, Link> _graph = new BidirectionalGraph<Node, Link>(true);

        public CompositeNode()
        {
        }

        public CompositeNode(NodeDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void AddNode(Node node)
        {
            Argument.NotNull(node, nameof(node));

            if (node.Parent != null)
            {
                throw new ApplicationException("The node is already attached to node graph.");
            }

            if (_graph.AddVertex(node))
            {
                node.Parent = this;
            }
        }

        public override IReadOnlyDictionary<string, IPin> InputPins {
            get
            {
                var result = new Dictionary<string, IPin>();
                foreach (var node in _graph.Roots())
                {
                    foreach (var pin in node.InputPins)
                    {
                        result[node.Title + "." + pin.Key] = pin.Value;
                    }
                }
                return new ReadOnlyDictionary<string, IPin>(result);
            }
        }

        public override IReadOnlyDictionary<string, IPin> OutputPins
        {
            get
            {
                var result = new Dictionary<string, IPin>();
                foreach (var node in _graph.Sinks())
                {
                    foreach (var pin in node.OutputPins)
                    {
                        result[node.Title + "." + pin.Key] = pin.Value;
                    }
                }
                return new ReadOnlyDictionary<string, IPin>(result);
            }
        }

        public void Link<TFrom, TTo>(TFrom fromNode, IPin fromPin, TTo toNode, IPin toPin)
            where TFrom : Node
            where TTo : Node
        {
            Argument.NotNull(fromNode, nameof(fromNode));
            Argument.NotNull(toNode, nameof(toNode));
            Argument.NotNull(fromPin, nameof(fromPin));
            Argument.NotNull(toPin, nameof(toPin));

            _graph.AddEdge(new Link(fromNode, fromPin, toNode, toPin));
        }

        public void Unlink<TTo>(TTo toNode, IPin toPin)
            where TTo : Node
        {
            Argument.NotNull(toNode, nameof(toNode));
            Argument.NotNull(toPin, nameof(toPin));

            _graph.RemoveEdgeIf(link => link.Target == toNode && link.TargetPin == toPin);
        }

        protected internal override void Reset()
        {
            foreach (var node in _graph.Vertices)
            {
                node.Dispatcher.InvokeAsync(() => node.Reset());
            }
        }

        protected internal override void Process(ProcessingContext context)
        {
            foreach (var node in _graph.TopologicalSort())
            {
                PulseInternal(node, context);
            }
        }

        public new void Pulse()
        {
            Dispatcher.InvokeAsync(() =>
            {
                var context = new ProcessingContext();

                // TODO: Cache sort result. Clear cache on graph change.
                foreach (var node in _graph.TopologicalSort())
                {
                    PulseInternal(node, context);
                }
            });
        }

        internal void Pulse(Node node)
        {
            var context = new ProcessingContext();

            // TODO: Cache sort result. Clear cache on graph change.
            var sortedNodes = _graph.TopologicalSort();
            var descendants = new HashSet<Node> {node};
            PopulateDescendants(node, descendants);

            foreach (var n in sortedNodes.Intersect(descendants))
            {
                PulseInternal(n, context);
            }
        }

        private void PulseInternal(Node node, ProcessingContext context)
        {
            if (Dispatcher == node.Dispatcher)
            {
                PulseNode(node, context);
            }
            else
            {
                node.Dispatcher.InvokeAsync(() =>
                {
                    PulseNode(node, context);
                });
            }
        }

        private void PulseNode(Node node, ProcessingContext context)
        {
            if (context.DoNotPulseFurther)
            {
                return;
            }

            foreach (var link in _graph.InEdges(node))
            {
                link.PullValue(node.Dispatcher);
            }

            node.ProcessInternal(context);

            if (context.DoNotPulseFurther)
            {
                return;
            }

            foreach (var link in _graph.OutEdges(node))
            {
                link.PushValue();
            }
        }

        private void PopulateDescendants(Node node, HashSet<Node> descendants)
        {
            foreach (var outEdge in _graph.OutEdges(node))
            {
                if (descendants.Add(outEdge.Target))
                {
                    PopulateDescendants(outEdge.Target, descendants);
                }
            }
        }
    }
}