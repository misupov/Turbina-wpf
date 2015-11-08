using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Turbina
{
    public class CompositeNode : Node
    {
        private readonly BidirectionalGraph<Node, Link> _graph = new BidirectionalGraph<Node, Link>(true);
        private readonly Dictionary<Node, ExecutionPath> _executionPaths = new Dictionary<Node, ExecutionPath>();
        private ExecutionPath _defaultExecutionPath;

        public CompositeNode()
        {
        }

        private void OnGraphChanged()
        {
            ResetCache();
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

            lock (_graph)
            {
                if (_graph.AddVertex(node))
                {
                    OnGraphChanged();
                    node.Parent = this;
                }
            }
        }

        public void RemoveNode(Node node)
        {
            Argument.NotNull(node, nameof(node));

            lock (_graph)
            {
                if (_graph.RemoveVertex(node))
                {
                    _graph.RemoveEdgeIf(link => link.Source == node || link.Target == node);
                    OnGraphChanged();
                    node.Parent = null;
                }
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

            lock (_graph)
            {
                _graph.AddEdge(new Link(fromNode, fromPin, toNode, toPin));
//                Debug.WriteLine("Link: " + fromNode.Title + "." + fromPin.Name + " ->" + toNode.Title + "." + toPin.Name);
                OnGraphChanged();
            }
        }

        public void Unlink<TTo>(TTo toNode, IPin toPin)
            where TTo : Node
        {
            Argument.NotNull(toNode, nameof(toNode));
            Argument.NotNull(toPin, nameof(toPin));

            lock (_graph)
            {
                _graph.RemoveEdgeIf(link => link.Target == toNode && link.TargetPin == toPin);
//                Debug.WriteLine("Unlink: " + toNode.Title + "." + toPin.Name);
                OnGraphChanged();
            }
        }

        public void ResetNodes()
        {
            Reset();
        }

        protected internal override void Reset()
        {
            lock (_graph)
            {
                foreach (var node in _graph.Vertices)
                {
                    node.Dispatcher.InvokeAsync(() => node.Reset());
                }
            }
        }

        protected internal override void Process(ProcessingContext context)
        {
            foreach (var node in context.TopologicallySortedNodes)
            {
                PulseInternal(node, context);
            }
        }

        public new void Pulse()
        {
            if (Parent != null)
            {
                Parent.Pulse(this);
            }
            else
            {
                Dispatcher.InvokeAsync(() =>
                {
                    Process(new ProcessingContext(GetDefaultExecutionPath()));
                });
            }
        }

        internal void Pulse(Node node)
        {
            var context = new ProcessingContext(GetExecutionPath(node));

            foreach (var n in GetExecutionPath(node).TopologicallySortedNodes)
            {
                PulseInternal(n, context);
            }
        }

        private ExecutionPath GetDefaultExecutionPath()
        {
            lock (_graph)
            {
                return _defaultExecutionPath ?? (_defaultExecutionPath = new ExecutionPath(_graph.TopologicalSort().ToArray()));
            }
        }

        private ExecutionPath GetExecutionPath(Node node)
        {
            lock (_graph)
            {
                ExecutionPath path;
                if (!_executionPaths.TryGetValue(node, out path))
                {
                    var sortedNodes = _graph.TopologicalSort();
                    var descendants = new HashSet<Node> { node };
                    PopulateDescendants(node, descendants);
                    _executionPaths[node] = path = new ExecutionPath(sortedNodes.Intersect(descendants).ToArray());
                }

                return path;
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
            Link[] inEdges;
            Link[] outEdges;
            lock (_graph)
            {
                inEdges = _graph.InEdges(node).ToArray();
                outEdges = _graph.OutEdges(node).ToArray();
            }

            // TODO: point of optimization
            if (node != context.ExecutionPath.TopologicallySortedNodes[0] &&
                context.ExecutionPath.Nodes.Intersect(inEdges.Select(link => link.Source))
                    .All(n => context.DeadNodes.Contains(n)))
            {
                context.DeadNodes.Add(node);
                node.ProcessInternal(context);
            }
            else
            {
                foreach (var link in inEdges)
                {
                    link.PullValue(node.Dispatcher);
                }

                node.ProcessInternal(context);

                if (context.DoNotPulseFurther)
                {
                    context.DeadNodes.Add(node);
                    context.DoNotPulseFurther = false;
                    return;
                }

                foreach (var link in outEdges)
                {
                    link.PushValue();
                }
            }
        }

        private void ResetCache()
        {
            _defaultExecutionPath = null;
            _executionPaths.Clear();
        }

        public bool TryGetSource(Node targetNode, IPin targetPin, out Node sourceNode, out IPin sourcePin)
        {
            lock (_graph)
            {
                var link = _graph.InEdges(targetNode).FirstOrDefault(l => l.TargetPin == targetPin);
                if (link != null)
                {
                    sourceNode = link.Source;
                    sourcePin = link.SourcePin;
                    return true;
                }

                sourceNode = null;
                sourcePin = null;
                return false;
            }
        }
    }
}