using System;
using System.Diagnostics;

namespace Turbina
{
    public static class Argument
    {
        [DebuggerHidden]
        public static void NotNull<T>(T obj, string argumentName)
            where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        [DebuggerHidden]
        public static void NotNull<T>(T obj, string argumentName, string message)
            where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }
    }
}