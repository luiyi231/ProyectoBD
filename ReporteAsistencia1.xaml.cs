using Capa_Negocios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoBD
{
    /// <summary>
    /// Lógica de interacción para ReporteAsistencia1.xaml
    /// </summary>
    public partial class ReporteAsistencia1 : UserControl
    {
        private ReporteAsistenciasNegocio objReporteNegocio;

        public ReporteAsistencia1()
        {
            InitializeComponent();
            objReporteNegocio = new ReporteAsistenciasNegocio();
            Loaded += ReporteAsistencias_Loaded;
        }

        private void ReporteAsistencias_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosIniciales();
            lblFechaReporte.Text = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        private void CargarDatosIniciales()
        {
            try
            {
                // Cargar carreras
                DataTable carreras = objReporteNegocio.ObtenerCarreras();
                cbCarrera.ItemsSource = carreras.DefaultView;

                // Cargar gestiones
                DataTable gestiones = objReporteNegocio.ObtenerGestiones();
                cbGestion.ItemsSource = gestiones.DefaultView;

                // Limpiar otros controles
                cbMateria.ItemsSource = null;
                dgReporte.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos iniciales: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbCarrera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCarrera.SelectedValue != null)
            {
                try
                {
                    int idCarrera = Convert.ToInt32(cbCarrera.SelectedValue);
                    DataTable materias = objReporteNegocio.ObtenerMateriasPorCarrera(idCarrera);
                    cbMateria.ItemsSource = materias.DefaultView;

                    // Limpiar reporte anterior
                    dgReporte.ItemsSource = null;
                    lblTotalEstudiantes.Text = "";
                    lblPromedioAsistencia.Text = "";
                    lblInfoReporte.Text = "Seleccione la materia y gestión, luego genere el reporte";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar materias: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                cbMateria.ItemsSource = null;
            }
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarSelecciones())
                return;

            try
            {
                int idMateria = Convert.ToInt32(cbMateria.SelectedValue);
                int idGestion = Convert.ToInt32(cbGestion.SelectedValue);

                // Generar reporte
                DataTable reporte = objReporteNegocio.GenerarReporteAsistencias(idMateria, idGestion);

                if (reporte.Rows.Count > 0)
                {
                    dgReporte.ItemsSource = reporte.DefaultView;

                    // Calcular estadísticas
                    int totalEstudiantes = reporte.Rows.Count;
                    double promedioAsistencia = Convert.ToDouble(reporte.Compute("AVG(PorcentajeAsistencia)", ""));

                    // Obtener información de la materia
                    DataTable infoMateria = objReporteNegocio.ObtenerInfoMateria(idMateria, idGestion);
                    string nombreMateria = infoMateria.Rows.Count > 0 ? infoMateria.Rows[0]["NombreMateria"].ToString() : cbMateria.Text;

                    // Actualizar labels informativos
                    string carrera = cbCarrera.Text;
                    string gestion = cbGestion.Text;

                    lblInfoReporte.Text = $"📈 Reporte: {nombreMateria} - {carrera} - {gestion}";
                    lblTotalEstudiantes.Text = $"Total de estudiantes: {totalEstudiantes}";
                    lblPromedioAsistencia.Text = $"Promedio de asistencia: {promedioAsistencia:F2}%";

                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    dgReporte.ItemsSource = null;
                    lblInfoReporte.Text = "❌ No se encontraron registros de asistencia para los criterios seleccionados";
                    lblTotalEstudiantes.Text = "";
                    lblPromedioAsistencia.Text = "";
                    btnImprimir.IsEnabled = false;

                    MessageBox.Show("No se encontraron registros de asistencia para los criterios seleccionados.",
                        "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarSelecciones()
        {
            if (cbCarrera.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una carrera.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbCarrera.Focus();
                return false;
            }

            if (cbMateria.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una materia.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbMateria.Focus();
                return false;
            }

            if (cbGestion.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una gestión.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbGestion.Focus();
                return false;
            }

            return true;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (dgReporte.Items.Count == 0)
            {
                MessageBox.Show("No hay datos para imprimir.", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // Crear documento para imprimir
                FlowDocument flowDocument = CrearDocumentoParaImprimir();

                // Configurar diálogo de impresión
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    // Configurar el documento para la impresión
                    flowDocument.PageHeight = printDialog.PrintableAreaHeight;
                    flowDocument.PageWidth = printDialog.PrintableAreaWidth;
                    flowDocument.PagePadding = new Thickness(50);
                    flowDocument.ColumnGap = 0;
                    flowDocument.ColumnWidth = printDialog.PrintableAreaWidth - 100;

                    // Crear un DocumentPaginator
                    IDocumentPaginatorSource idpSource = flowDocument;

                    // Imprimir el documento
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Reporte de Asistencias");

                    MessageBox.Show("El reporte se ha enviado a la impresora correctamente.",
                        "Impresión exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir el reporte: {ex.Message}",
                    "Error de impresión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CrearDocumentoParaImprimir()
        {
            FlowDocument flowDocument = new FlowDocument();

            // Título del documento
            Paragraph titulo = new Paragraph(new Run("REPORTE DE ASISTENCIAS POR MATERIA"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            flowDocument.Blocks.Add(titulo);

            // Información del reporte
            Paragraph info = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            info.Inlines.Add(new Run($"Carrera: {cbCarrera.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Materia: {cbMateria.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Gestión: {cbGestion.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}"));
            flowDocument.Blocks.Add(info);

            // Estadísticas
            if (!string.IsNullOrEmpty(lblTotalEstudiantes.Text))
            {
                Paragraph estadisticas = new Paragraph()
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };
                estadisticas.Inlines.Add(new Run(lblTotalEstudiantes.Text) { FontWeight = FontWeights.SemiBold });
                estadisticas.Inlines.Add(new LineBreak());
                estadisticas.Inlines.Add(new Run(lblPromedioAsistencia.Text) { FontWeight = FontWeights.SemiBold });
                flowDocument.Blocks.Add(estadisticas);
            }

            // Crear tabla con los datos
            Table tabla = new Table()
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Definir columnas
            tabla.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) }); // Estudiante
            tabla.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) }); // CI
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.7, GridUnitType.Star) }); // RU
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.7, GridUnitType.Star) }); // Grupo
            tabla.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) }); // Docente
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Total Clases
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Asistidas
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Faltadas
            tabla.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) }); // % Asistencia

            // Crear grupo de filas
            TableRowGroup tableRowGroup = new TableRowGroup();

            // Fila de encabezados
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGray;

            string[] headers = { "Estudiante", "CI", "RU", "Grupo", "Docente", "Total", "Asist.", "Faltas", "% Asist." };
            foreach (string header in headers)
            {
                TableCell cell = new TableCell(new Paragraph(new Run(header) { FontWeight = FontWeights.Bold }))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Center
                };
                headerRow.Cells.Add(cell);
            }
            tableRowGroup.Rows.Add(headerRow);

            // Filas de datos
            if (dgReporte.ItemsSource is DataView dataView)
            {
                foreach (DataRowView row in dataView)
                {
                    TableRow dataRow = new TableRow();

                    // Estudiante
                    dataRow.Cells.Add(CrearCelda(row["Estudiante"].ToString()));
                    // CI
                    dataRow.Cells.Add(CrearCelda(row["CI"].ToString()));
                    // RU
                    dataRow.Cells.Add(CrearCelda(row["RU"].ToString()));
                    // Grupo
                    dataRow.Cells.Add(CrearCelda(row["Grupo"].ToString(), TextAlignment.Center));
                    // Docente
                    dataRow.Cells.Add(CrearCelda(row["Docente"].ToString()));
                    // Total Clases
                    dataRow.Cells.Add(CrearCelda(row["TotalClases"].ToString(), TextAlignment.Center));
                    // Asistidas
                    dataRow.Cells.Add(CrearCelda(row["ClasesAsistidas"].ToString(), TextAlignment.Center));
                    // Faltadas
                    dataRow.Cells.Add(CrearCelda(row["ClasesFaltadas"].ToString(), TextAlignment.Center));
                    // Porcentaje
                    double porcentaje = Convert.ToDouble(row["PorcentajeAsistencia"]);
                    TableCell celdaPorcentaje = CrearCelda($"{porcentaje:F2}%", TextAlignment.Center);

                    // Colorear según el porcentaje
                    if (porcentaje < 70)
                        celdaPorcentaje.Background = Brushes.LightCoral;
                    else if (porcentaje < 85)
                        celdaPorcentaje.Background = Brushes.LightYellow;
                    else
                        celdaPorcentaje.Background = Brushes.LightGreen;

                    dataRow.Cells.Add(celdaPorcentaje);

                    tableRowGroup.Rows.Add(dataRow);
                }
            }

            tabla.RowGroups.Add(tableRowGroup);
            flowDocument.Blocks.Add(tabla);

            // Pie de página
            Paragraph piePagina = new Paragraph(new Run("Reporte generado por Sistema de Gestión Académica"))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            flowDocument.Blocks.Add(piePagina);

            return flowDocument;
        }

        private TableCell CrearCelda(string texto, TextAlignment alineacion = TextAlignment.Left)
        {
            return new TableCell(new Paragraph(new Run(texto)))
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                TextAlignment = alineacion
            };
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Limpiar selecciones
                cbCarrera.SelectedIndex = -1;
                cbMateria.SelectedIndex = -1;
                cbGestion.SelectedIndex = -1;

                // Limpiar datos
                cbMateria.ItemsSource = null;
                dgReporte.ItemsSource = null;

                // Restablecer labels
                lblInfoReporte.Text = "Seleccione carrera, materia y gestión para generar el reporte";
                lblTotalEstudiantes.Text = "";
                lblPromedioAsistencia.Text = "";

                // Deshabilitar botón de imprimir
                btnImprimir.IsEnabled = false;

                // Actualizar fecha
                lblFechaReporte.Text = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar el formulario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Converter para colorear las celdas según el porcentaje de asistencia
    public class AsistenciaColorConverter : IValueConverter
    {
        public static AsistenciaColorConverter Instance { get; } = new AsistenciaColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal porcentaje)
            {
                if (porcentaje < 70)
                    return "Bajo";
                else if (porcentaje < 85)
                    return "Medio";
                else
                    return "Alto";
            }
            return "Medio";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
