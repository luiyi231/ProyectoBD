using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD
{
    public sealed class AuthManager
    {
        private static AuthManager _instance = null;
        private static readonly object _lock = new object();

        // Credenciales hardcodeadas
        private const string VALID_EMAIL = "luigimateoencinas@gmail.com";
        private const string VALID_PASSWORD = "12345678";

        // Propiedades del usuario logueado
        public bool IsLoggedIn { get; private set; }
        public string CurrentUserEmail { get; private set; }
        public DateTime LoginTime { get; private set; }

        // Constructor privado para implementar Singleton
        private AuthManager()
        {
            IsLoggedIn = false;
            CurrentUserEmail = string.Empty;
        }

        /// <summary>
        /// Instancia única del AuthManager (Singleton)
        /// </summary>
        public static AuthManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new AuthManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Intenta autenticar al usuario con las credenciales proporcionadas
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>True si la autenticación es exitosa, False en caso contrario</returns>
        public bool Login(string email, string password)
        {
            try
            {
                // Validar parámetros
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }

                // Validar credenciales
                if (email.Trim().ToLower() == VALID_EMAIL.ToLower() && password == VALID_PASSWORD)
                {
                    // Establecer estado de usuario logueado
                    IsLoggedIn = true;
                    CurrentUserEmail = email.Trim();
                    LoginTime = DateTime.Now;

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        public void Logout()
        {
            IsLoggedIn = false;
            CurrentUserEmail = string.Empty;
            LoginTime = DateTime.MinValue;
        }

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        /// <returns>Información del usuario o null si no está logueado</returns>
        public UserInfo GetCurrentUser()
        {
            if (!IsLoggedIn)
                return null;

            return new UserInfo
            {
                Email = CurrentUserEmail,
                LoginTime = LoginTime,
                SessionDuration = DateTime.Now - LoginTime
            };
        }

        /// <summary>
        /// Verifica si la sesión sigue siendo válida
        /// </summary>
        /// <param name="maxSessionHours">Máximo número de horas para mantener la sesión activa</param>
        /// <returns>True si la sesión es válida</returns>
        public bool IsSessionValid(int maxSessionHours = 8)
        {
            if (!IsLoggedIn)
                return false;

            var sessionDuration = DateTime.Now - LoginTime;
            return sessionDuration.TotalHours <= maxSessionHours;
        }
    }

    /// <summary>
    /// Clase para almacenar información del usuario
    /// </summary>
    public class UserInfo
    {
        public string Email { get; set; }
        public DateTime LoginTime { get; set; }
        public TimeSpan SessionDuration { get; set; }

        public string DisplayName => Email.Split('@')[0]; // Obtiene la parte antes del @
    }
}
