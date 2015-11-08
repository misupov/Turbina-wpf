using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Turbina
{
    [DebuggerDisplay("Property Pin: {Name,nq} of type {Type.Name,nq}")]
    internal class PropertyPin : IPin
    {
        private readonly Node _node;
        private readonly PropertyInfo _propertyInfo;

        public PropertyPin(Node node, PinDirection direction, PropertyInfo propertyInfo)
        {
            _node = node;
            Direction = direction;
            _propertyInfo = propertyInfo;
        }

        public string Name => _propertyInfo.Name;

        public Type Type => _propertyInfo.PropertyType;

        public PinDirection Direction { get; }

        public Func<object> GetValue => () => _propertyInfo.GetValue(_node);

        public Action<object> SetValue => value =>_propertyInfo.SetValue(_node, Cast(value));

        private object Cast(object value)
        {
            if (Type == typeof (object))
            {
                return value;
            }

            if (Type == typeof (string))
            {
                return value?.ToString();
            }

            if (value != null && value.GetType() == Type)
            {
                return value;
            }

            if (value == null)
            {
                return Type.IsPrimitive ? Activator.CreateInstance(Type) : null;
            }

            var typeConverter = TypeDescriptor.GetConverter(Type);
            if (typeConverter.IsValid(value))
            {
                return typeConverter.ConvertFrom(value);
            }

            return Type.IsPrimitive ? Activator.CreateInstance(Type) : null;
        }
    }
}