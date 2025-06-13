using Capa_Negocios;
using ProyectoBD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Capa_Presentacion
{
    public partial class MainWindow : Window
    {
        private bool isMenuOpen = false;

        public MainWindow()
        {
            InitializeComponent();

            // Configurar atajos de teclado
            this.KeyDown += MainWindow_KeyDown;

            // Mostrar página de inicio por defecto
            ShowWelcomePage();
        }

        private void ShowWelcomePage()
        {
            // Crear una página de bienvenida
            var welcomePanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var welcomeIcon = new TextBlock
            {
                Text = "🎓",
                FontSize = 72,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var welcomeTitle = new TextBlock
            {
                Text = "Bienvenido al Sistema de Gestión",
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var welcomeSubtitle = new TextBlock
            {
                Text = "Selecciona una opción del menú para comenzar",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Opacity = 0.7
            };

            var instructionText = new TextBlock
            {
                Text = "💡 Presiona el botón ☰ o usa Ctrl+M para abrir el menú",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0),
                Opacity = 0.6
            };

            welcomePanel.Children.Add(welcomeIcon);
            welcomePanel.Children.Add(welcomeTitle);
            welcomePanel.Children.Add(welcomeSubtitle);
            welcomePanel.Children.Add(instructionText);

            Contenido.Content = welcomePanel;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Atajo Ctrl+M para abrir/cerrar menú
            if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ToggleMenu();
            }
            // Escape para cerrar menú
         
            else if (e.Key == Key.Escape && isMenuOpen)
            {
                CloseMenu();
            }
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if (isMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        private void OpenMenu()
        {
            if (!isMenuOpen)
            {
                SideMenu.Visibility = Visibility.Visible;
                Overlay.Visibility = Visibility.Visible;

                // Animación del overlay
                var overlayAnimation = new DoubleAnimation(0, 0.3, TimeSpan.FromMilliseconds(300));
                Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

                // Animación del menú
                var slideIn = (Storyboard)this.Resources["SlideInMenu"];
                slideIn.Begin(SideMenu);

                isMenuOpen = true;
            }
        }

        private void CloseMenu()
        {
            if (isMenuOpen)
            {
                // Animación del overlay
                var overlayAnimation = new DoubleAnimation(0.3, 0, TimeSpan.FromMilliseconds(300));
                overlayAnimation.Completed += (s, e) =>
                {
                    Overlay.Visibility = Visibility.Collapsed;
                };
                Overlay.BeginAnimation(UIElement.OpacityProperty, overlayAnimation);

                // Animación del menú
                var slideOut = (Storyboard)this.Resources["SlideOutMenu"];
                slideOut.Completed += (s, e) =>
                {
                    SideMenu.Visibility = Visibility.Collapsed;
                };
                slideOut.Begin(SideMenu);

                isMenuOpen = false;
            }
        }

        private void Overlay_Click(object sender, MouseButtonEventArgs e)
        {
            CloseMenu();
        }

        // Métodos para los botones del menú
        private void BtnInscripcion_Click(object sender, RoutedEventArgs e)
        {
            Contenido.Content = new InscripcionUser();
            CloseMenu();

            // Efecto visual al seleccionar opción
            AnimateContentChange();
        }

        private void BtnReporteMaterias_Click(object sender, RoutedEventArgs e)
        {
            Contenido.Content = new ReporteMaterias1();
            CloseMenu();
            AnimateContentChange();
        }

        private void BtnReporteAsistencia_Click(object sender, RoutedEventArgs e)
        {
            Contenido.Content = new ReporteAsistencia1();
            CloseMenu();
            AnimateContentChange();
        }

        private void BtnReporteNotasEstudiante_Click(object sender, RoutedEventArgs e)
        {
            Contenido.Content = new ReporteNotasEstudiante();
            CloseMenu();
            AnimateContentChange();
        }
        private void BtnReporteNotase_Click(object sender, RoutedEventArgs e)
        {
            Contenido.Content = new ReportesNotas();
            CloseMenu();
            AnimateContentChange();
        }

        private void AnimateContentChange()
        {
            // Animación suave al cambiar contenido
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
            fadeOut.Completed += (s, e) =>
            {
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                Contenido.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };
            Contenido.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        // Método para manejar el cierre de la ventana
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}