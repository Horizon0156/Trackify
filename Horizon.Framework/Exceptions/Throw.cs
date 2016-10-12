using JetBrains.Annotations;
using System;

namespace Horizon.Framework.Exceptions
{
    /// <summary>
    /// This class provides Helper to throw commom Exceptions such as
    /// <see cref="ArgumentException"/>, <see cref="ArgumentNullException"/>, ...
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Throws an exception if the provided argument is null.
        /// </summary>
        /// <param name="argument"> The argument which will be checked. </param>
        /// <param name="argumentName"> The name of the argument, which will be checked. </param>
        /// <exception cref="ArgumentNullException">If the provided argument is null. </exception>
        public static void IfArgumentIsNull(object argument, [CanBeNull] string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}