using Capa_Presentacion;
using System;
using System.Windows;
using System.Windows.Input;

namespace ProyectoBD
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Permitir login con Enter
            txtPassword.KeyDown += TxtPassword_KeyDown;
            txtEmail.KeyDown += TxtEmail_KeyDown;

            // Prellenar los campos para pruebas (opcional - quitar en producción)
            txtEmail.Text = "luigimateoencinas@gmail.com";
            txtPassword.Password = "12345678";
        }

        private void TxtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ocultar mensaje de error previo
                lblError.Visibility = Visibility.Collapsed;

                // Validar campos vacíos
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MostrarError("Por favor ingrese su correo electrónico");
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MostrarError("Por favor ingrese su contraseña");
                    txtPassword.Focus();
                    return;
                }

                // Validar credenciales usando el AuthManager
                if (AuthManager.Instance.Login(txtEmail.Text.Trim(), txtPassword.Password))
                {
                    // Login exitoso - abrir ventana principal
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Cerrar ventana de login
                    this.Close();
                }
                else
                {
                    MostrarError("Credenciales incorrectas. Verifique su email y contraseña.");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visibility = Visibility.Visible;
        }

        // Opcional: Cerrar aplicación si se cierra la ventana de login
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!AuthManager.Instance.IsLoggedIn)
            {
                Application.Current.Shutdown();
            }
        }
    }
}