using System;

namespace Turbina
{
    public interface IPin
    {
        string Name { get; }

        Type Type { get; }

        PinDirection Direction { get; }

        Func<object> GetValue { get; }

        Action<object> SetValue { get; }
    }
}