using System;

namespace Turbina.Editors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorForAttribute : Attribute
    {
        public EditorForAttribute(params Type[] nodeTypes)
        {
            NodeTypes = nodeTypes;
        }

        public Type[] NodeTypes { get; }
    }
}