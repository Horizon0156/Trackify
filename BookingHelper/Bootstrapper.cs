using AutoMapper;
using BookingHelper.DataModels;
using BookingHelper.Deployment;
using BookingHelper.Mocks;
using BookingHelper.UI;
using BookingHelper.ViewModels;
using Horizon.Framework.Mvvm;
using MahApps.Metro;
using SimpleInjector;
using System;
using System.Reflection;
using System.Windows;
using Horizon.Framework.Extensions;
using Horizon.Framework.Services;
using log4net;
using MahApps.Metro.Controls.Dialogs;

namespace BookingHelper
{
    internal static class Bootstrapper
    {
        private static readonly Container _container = new Container();
        private static readonly DialogService _dialogService = new DialogService();
        private static readonly MessageServiceEventManager _messageService = new MessageServiceEventManager();

        [STAThread]
        public static int Main()
        {
            var app = new Application();
            app.Startup += ShowMainWindow;

            InitializeMetroTheme(app);
            InitializeMappings();
            InitializeDialogService();
            InitializeDependencyInjection();

            return app.Run();
        }

        private static void InitializeDependencyInjection()
        {
            _container.RegisterSingleton<IDialogService>(_dialogService);
            _container.RegisterSingleton<IMessageService>(_messageService);
            _container.RegisterSingleton<ICommandFactory, CommandFactory>();
            _container.Register<IBookingsContext, BookingsContext>();
            _container.Register<IProcess, Process>();
            _container.Register<IUpdateChecker, ClickOnceUpdateChecker>();

            _container.RegisterSingleton(() => LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name));
        }

        private static void InitializeDialogService()
        {
            _dialogService.RegisterCustomWindowActivator(t => (Window)_container.GetInstance(t));
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

            _dialogService.RegisterMainWindow(window);
            _messageService.MessageAnnouncement += window.HandleMessageAnnouncement;

            window.Show();
        }
    }
}