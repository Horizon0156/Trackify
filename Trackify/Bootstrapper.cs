using AutoMapper;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Services;
using Horizon.MvvmFramework.Wpf.Extensions;
using log4net;
using MahApps.Metro;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using Trackify.DataModels;
using Trackify.Factories;
using Trackify.Mocks;
using Trackify.UI;
using Trackify.ViewModels;
using Container = SimpleInjector.Container;

namespace Trackify
{
    internal class Bootstrapper : Application
    {
        private readonly Container _container;
        private readonly MessageHub _messageHub;

        private Bootstrapper()
        {
            _container = new Container();
            _messageHub = new MessageHub();
        }

        [STAThread]
        public static int Main()
        {
            var app = new Bootstrapper();
            app.InitializeDependencyInjection();
            app.InitializeMetroTheme();
            app.InitializeMappings();

            return app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _container.GetInstance<TrackifyWindow>().Show();
        }

        private void InitializeDependencyInjection()
        {
            _container.RegisterSingleton<IMessenger>(_messageHub);
            _container.RegisterSingleton<ICommandFactory, CommandFactory>();
            _container.RegisterSingleton<ISettings, UserSettings>();
            _container.RegisterSingleton<IViewModelFactory, ViewModelFactory>();
            _container.RegisterSingleton<IDatabaseContext, DatabaseContext>();
            _container.RegisterSingleton(() => LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name));
            _container.Register<IProcess, Process>();
        }

        private void InitializeMappings()
        {
            Mapper.Initialize(config => config.CreateMap<TimeAcquisition, TimeAcquisitionModel>().ReverseMap());
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }

        private void InitializeMetroTheme()
        {
            this.InjectResourceDictionary("MahApps.Metro", "Styles/Controls.xaml");
            this.InjectResourceDictionary("MahApps.Metro", "Styles/Fonts.xaml");
            this.InjectResourceDictionary("MahApps.Metro", "Styles/Colors.xaml");

            // Setting unused Accents is required to theme the message box properly
            this.InjectResourceDictionary("MahApps.Metro", "Styles/Accents/Blue.xaml");
            this.InjectResourceDictionary("MahApps.Metro", "Styles/Accents/BaseLight.xaml");

            var settings = _container.GetInstance<ISettings>();
            settings.PropertyChanged += UpdateTheme;
            UpdateTheme(settings.AccentColor);
        }

        private void UpdateTheme(object sender, PropertyChangedEventArgs e)
        {
            var settings = (ISettings) sender;

            if (e.PropertyName == nameof(settings.AccentColor))
            {
                UpdateTheme(settings.AccentColor);
            }
        }

        private void UpdateTheme(string accentColor)
        {
            ThemeManager.ChangeAppStyle(
                this,
                ThemeManager.GetAccent(accentColor),
                ThemeManager.GetAppTheme("BaseDark"));
        }
    }
}