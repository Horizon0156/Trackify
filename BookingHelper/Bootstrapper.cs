using AutoMapper;
using BookingHelper.DataModels;
using BookingHelper.Deployment;
using BookingHelper.Mocks;
using BookingHelper.UI;
using BookingHelper.ViewModels;
using Horizon.Framework.DialogService;
using Horizon.Framework.Mvvm;
using Horizon.Framework.Xaml.Extensions;
using MahApps.Metro;
using SimpleInjector;
using System;
using System.Windows;

namespace BookingHelper
{
    internal static class Bootstrapper
    {
        private static readonly Container _container = new Container();
        private static readonly DialogService _dialogService = new DialogService();

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
            _container.RegisterSingleton<ICommandFactory, CommandFactory>();
            _container.Register<IBookingsContext, BookingsContext>();
            _container.Register<IProcess, Process>();
            _container.Register<IUpdateChecker, ClickOnceUpdateChecker>();
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

            ThemeManager.ChangeAppStyle(
                app,
                ThemeManager.GetAccent("Red"),
                ThemeManager.GetAppTheme("BaseDark"));
        }

        private static void ShowMainWindow(object sender, StartupEventArgs e)
        {
            var window = _container.GetInstance<BookingHelperWindow>();
            window.Show();

            _dialogService.RegisterMainWindow(window);
        }
    }
}