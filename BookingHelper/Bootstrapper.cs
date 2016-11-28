using AutoMapper;
using BookingHelper.DataModels;
using BookingHelper.Messages;
using BookingHelper.Mocks;
using BookingHelper.UI;
using BookingHelper.ViewModels;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Services;
using Horizon.MvvmFramework.Wpf.Extensions;
using log4net;
using MahApps.Metro;
using SimpleInjector;
using System;
using System.Reflection;
using System.Windows;

namespace BookingHelper
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
            _container.GetInstance<BookingHelperWindow>().Show();
        }

        private void ChangeAppStyle(AccentColorChangedMessage message)
        {
            ThemeManager.ChangeAppStyle(
                this,
                ThemeManager.GetAccent(message.NewAccentColor),
                ThemeManager.GetAppTheme("BaseDark"));
        }

        private void InitializeDependencyInjection()
        {
            _container.RegisterSingleton<IMessenger>(_messageHub);
            _container.RegisterSingleton<ICommandFactory, CommandFactory>();
            _container.RegisterSingleton<ISettings, UserSettings>();
            _container.Register<IBookingsContext, BookingsContext>();
            _container.Register<IProcess, Process>();
            _container.RegisterSingleton(() => LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name));
#if DEBUG
            _container.Verify();
#endif
        }

        private void InitializeMappings()
        {
            Mapper.Initialize(config => config.CreateMap<Booking, BookingModel>().ReverseMap());
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
            _messageHub.Register<AccentColorChangedMessage>(ChangeAppStyle);
            ChangeAppStyle(new AccentColorChangedMessage(settings.AccentColor));
        }
    }
}