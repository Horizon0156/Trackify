using System;
using System.Threading.Tasks;

namespace Horizon.MvvmFramework.Extensions
{
    /// <summary>
    /// Provides common task extensions.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Continues with the given action if the task faults in an unknown state.
        /// </summary>
        /// <param name="task"> The task. </param>
        /// <param name="action"> The action. </param>
        public static void OnUnobservedException(this Task task, Action<AggregateException> action)
        {
            task.ContinueWith(t => action.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}