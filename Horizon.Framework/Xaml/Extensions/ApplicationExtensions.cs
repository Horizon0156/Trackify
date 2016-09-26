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
        public static void InjectResourceDictionary(this Application app, string assemblyName, string xamlPath)
        {
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(string.Format("pack://application:,,,/{0};component/{1}", assemblyName, xamlPath));

            app.Resources.MergedDictionaries.Add(dictionary);
        }
    }
}