using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoBD
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurar manejo de excepciones no controladas
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Mostrar ventana de login al iniciar
            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            try
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        private void App_DispatcherUnhandledException(object sender,
                                                     System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Ha ocurrido un error inesperado: {e.Exception.Message}",
                          "Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);

            // Marcar como manejado para evitar que la aplicación se cierre
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cerrar sesión al salir de la aplicación
            if (AuthManager.Instance.IsLoggedIn)
            {
                AuthManager.Instance.Logout();
            }

            base.OnExit(e);
        }
    }
}
