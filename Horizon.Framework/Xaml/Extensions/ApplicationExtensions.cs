using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System;
using System.Windows;

namespace Horizon.Framework.Xaml.Extensions
{
    /// <summary>
    /// Extensiosn for a WPF application.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Injects the resource dictionary.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="xamlPath">The xaml path.</param>
        /// <exception cref="ArgumentNullException"> If any of the provided arguments is null. </exception>
        public static void InjectResourceDictionary([NotNull] this Application app, [NotNull] string assemblyName, [NotNull] string xamlPath)
        {
            Throw.IfArgumentIsNull(app, nameof(app));
            Throw.IfArgumentIsNull(assemblyName, nameof(assemblyName));
            Throw.IfArgumentIsNull(xamlPath, nameof(xamlPath));

            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(string.Format("pack://application:,,,/{0};component/{1}", assemblyName, xamlPath));

            app.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}