using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Turbina
{
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        internal static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return (object)x == (object)y;
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode((object)obj);
        }
    }
}