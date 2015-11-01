using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Turbina
{
    public interface INodeTypesRepository
    {
        void Register<TNode>() where TNode : Node;

        IReadOnlyList<Type> NodeTypes { get; }
    }

    internal class NodeTypesRepository : INodeTypesRepository
    {
        private readonly HashSet<Type> _nodeTypes = new HashSet<Type>();

        public void Register<TNode>() where TNode : Node
        {
            _nodeTypes.Add(typeof (TNode));
        }

        public IReadOnlyList<Type> NodeTypes => new ReadOnlyCollection<Type>(_nodeTypes.ToList());
    }
}