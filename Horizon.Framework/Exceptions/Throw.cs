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

        /// <summary>
        /// Throws an exception if an argument is not valid.
        /// </summary>
        /// <param name="isArgumentValid"> Flag wheather the argument is valid. </param>
        /// <param name="message"> The exception message, why the argument is not valid. </param>
        /// <param name="argumentName"> The name of the argument, which was checked. </param>
        /// <exception cref="ArgumentNullException">If the provided message is null. </exception>
        /// /// <exception cref="ArgumentException">If the argument is not valid. </exception>
        public static void IfArgumentIsNotValid(bool isArgumentValid, [NotNull] string message, [CanBeNull] string argumentName)
        {
            Throw.IfArgumentIsNull(message, nameof(message));

            if (!isArgumentValid)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if an operation is invalid.
        /// </summary>
        /// <param name="isOperationInvalid"> Flag wheather the operation is invalid. </param>
        /// <param name="message"> The message, used to point out the invalid operation. </param>
        /// <exception cref="ArgumentNullException">If the provided message is null. </exception>
        /// /// <exception cref="InvalidOperationException">If the operation is not valid. </exception>
        public static void IfOperationIsInvalid(bool isOperationInvalid, [NotNull] string message)
        {
            Throw.IfArgumentIsNull(message, nameof(message));

            if (isOperationInvalid)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}