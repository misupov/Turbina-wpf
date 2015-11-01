using System;
using System.Diagnostics;
using System.Reflection;

namespace Turbina
{
    [DebuggerDisplay("Property Pin: {Name,nq} of type {Type.Name,nq}")]
    internal class PropertyPin : IPin
    {
        private readonly Node _node;
        private readonly PropertyInfo _propertyInfo;

        public PropertyPin(Node node, PropertyInfo propertyInfo)
        {
            _node = node;
            _propertyInfo = propertyInfo;
        }

        public string Name => _propertyInfo.Name;

        public Type Type => _propertyInfo.PropertyType;

        public Func<object> GetValue => () => _propertyInfo.GetValue(_node);

        public Action<object> SetValue => value =>_propertyInfo.SetValue(_node, value);
    }
}