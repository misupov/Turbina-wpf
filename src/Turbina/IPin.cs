using System;

namespace Turbina
{
    public interface IPin
    {
        string Name { get; }

        Type Type { get; }

        Func<object> GetValue { get; }

        Action<object> SetValue { get; }
    }
}