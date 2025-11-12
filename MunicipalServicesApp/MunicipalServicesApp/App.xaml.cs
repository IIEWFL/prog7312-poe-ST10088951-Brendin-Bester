using System;
using System.Windows;
using System.Windows.Threading;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Register global exception handlers
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);

            // Load persisted domain data (best-effort; swallow errors to avoid blocking UI)
            try { PersistenceService.LoadIssues(); } catch { }
            try { EventPersistenceService.Load(); } catch { }
            try { AnnouncementPersistence.Load(); } catch { }
            try { ServiceRequestPersistence.Load(); } catch { }

            // Show main window
            var main = new MainWindow();
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save persisted domain data (best-effort)
            try { AnnouncementPersistence.Save(); } catch { }
            try { EventPersistenceService.Save(); } catch { }
            try { PersistenceService.SaveIssues(); } catch { }
            try { ServiceRequestPersistence.Save(); } catch { }

            // Detach handlers
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}", "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // attempt to persist before continuing/closing
                try { ServiceRequestPersistence.Save(); } catch { }
                try { AnnouncementPersistence.Save(); } catch { }
                try { EventPersistenceService.Save(); } catch { }
                try { PersistenceService.SaveIssues(); } catch { }
            }
            catch { }
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show($"A fatal error occurred: {ex?.Message ?? "Unknown error"}", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // best-effort save
                try { ServiceRequestPersistence.Save(); } catch { }
                try { AnnouncementPersistence.Save(); } catch { }
                try { EventPersistenceService.Save(); } catch { }
                try { PersistenceService.SaveIssues(); } catch { }
            }
            catch { }
        }
    }
}