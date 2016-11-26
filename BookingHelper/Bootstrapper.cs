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
    internal static class Bootstrapper
    {
        private static readonly Container _container = new Container();
        private static readonly MessageHub _messageHub = new MessageHub();

        [STAThread]
        public static int Main()
        {
            var app = new Application();
            app.Startup += ShowMainWindow;

            InitializeMetroTheme(app);
            InitializeMappings();
            InitializeDependencyInjection();

            return app.Run();
        }

        private static void InitializeDependencyInjection()
        {
            _container.RegisterSingleton<IMessenger>(_messageHub);
            _container.RegisterSingleton<ICommandFactory, CommandFactory>();
            _container.Register<IBookingsContext, BookingsContext>();
            _container.Register<IProcess, Process>();

            _container.RegisterSingleton(() => LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name));
        }

        private static void InitializeMappings()
        {
            Mapper.Initialize(config => config.CreateMap<Booking, BookingModel>().ReverseMap());
            Mapper.AssertConfigurationIsValid();
        }

        private static void InitializeMetroTheme(Application app)
        {
            app.InjectResourceDictionary("MahApps.Metro", "Styles/Controls.xaml");
            app.InjectResourceDictionary("MahApps.Metro", "Styles/Fonts.xaml");
            app.InjectResourceDictionary("MahApps.Metro", "Styles/Colors.xaml");

            // Setting unused Accents is required to theme the message box properly
            app.InjectResourceDictionary("MahApps.Metro", "Styles/Accents/Blue.xaml");
            app.InjectResourceDictionary("MahApps.Metro", "Styles/Accents/BaseLight.xaml");

            ThemeManager.ChangeAppStyle(
                app,
                ThemeManager.GetAccent("Red"),
                ThemeManager.GetAppTheme("BaseDark"));
        }

        private static void ShowMainWindow(object sender, StartupEventArgs e)
        {
            var window = _container.GetInstance<BookingHelperWindow>();
            _messageHub.Register<PrepareNewEntryMessage>(window.PrepareNewEntry);

            window.Show();
        }
    }
}