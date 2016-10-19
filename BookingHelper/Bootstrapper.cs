using AutoMapper;
using BookingHelper.DataModels;
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
        private static Container _container = new Container();
        private static DialogService _dialogService = new DialogService();

        [STAThread]
        public static int Main()
        {
            var app = new Application();
            InitializeMetroTheme(app);
            app.Startup += ShowMainWindow;

            InitializeMappings();
            InitializeDialogService();
            InitializeDependencyInjection();

            return app.Run();
        }

        private static void InitializeDependencyInjection()
        {
            _container.RegisterSingleton<IDialogService>(_dialogService);
            _container.Register<IBookingsContext, BookingsContext>();
            _container.RegisterInitializer<MainWindow>(InitializeMainWindow);
        }

        private static void InitializeDialogService()
        {
            _dialogService.RegisterDialog<MessageViewModel, MessageWindow>();
            _dialogService.RegisterCustomWindowActivator(t => (Window)_container.GetInstance(t));
        }

        private static void InitializeMainWindow(MainWindow window)
        {
            _dialogService.RegisterMainWindow(window);
            window.DataContext = _container.GetInstance<MainWindowViewModel>();
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
            _container.GetInstance<MainWindow>().Show();
        }
    }
}