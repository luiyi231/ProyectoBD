using Capa_Negocios;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica de interacción para ReporteAsistencia.xaml
    /// </summary>
    public partial class ReporteAsistencia1 : UserControl
    {
        private Capa_Negocios.ReporteAsistenciaNegocio objReporteNegocio;
        private Gestion objGestion;

        public ReporteAsistencia1()
        {
            InitializeComponent();

            objReporteNegocio = new Capa_Negocios.ReporteAsistenciaNegocio();
            objGestion = new Gestion();

            Loaded += ReporteAsistencia_Loaded;
        }

        private void ReporteAsistencia_Loaded(object sender, RoutedEventArgs e)
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
                DataTable gestiones = objGestion.ObtenerTodas();
                cbGestion.ItemsSource = gestiones.DefaultView;
                cbGestion.DisplayMemberPath = "Gestion";
                cbGestion.SelectedValuePath = "codGestion";

                cbMateria.ItemsSource = null;
                cbFecha.ItemsSource = null;
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

                    cbFecha.ItemsSource = null;
                    dgReporte.ItemsSource = null;
                    lblTotalEstudiantes.Text = "";
                    lblEstadisticasAsistencia.Text = "";
                    lblInfoReporte.Text = "Seleccione la materia, gestión y fecha, luego genere el reporte";
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
                cbFecha.ItemsSource = null;
            }
        }

        private void cbMateria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Limpiar controles dependientes
            cbFecha.ItemsSource = null;
            dgReporte.ItemsSource = null;
            lblTotalEstudiantes.Text = "";
            lblEstadisticasAsistencia.Text = "";

            if (cbMateria.SelectedValue != null)
            {
                lblInfoReporte.Text = "Seleccione la gestión y fecha, luego genere el reporte";
                CargarFechas();
            }
        }

        private void cbGestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Limpiar reporte cuando cambie la gestión
            cbFecha.ItemsSource = null;
            dgReporte.ItemsSource = null;
            lblTotalEstudiantes.Text = "";
            lblEstadisticasAsistencia.Text = "";

            if (cbGestion.SelectedValue != null)
            {
                CargarFechas();
            }
        }

        private void CargarFechas()
        {
            if (cbMateria.SelectedValue != null && cbGestion.SelectedValue != null)
            {
                try
                {
                    int idMateria = Convert.ToInt32(cbMateria.SelectedValue);
                    int idGestion = Convert.ToInt32(cbGestion.SelectedValue);

                    DataTable fechas = objReporteNegocio.ObtenerFechasAsistencia(idMateria, idGestion);
                    cbFecha.ItemsSource = fechas.DefaultView;
                    cbFecha.DisplayMemberPath = "FechaFormateada";
                    cbFecha.SelectedValuePath = "Fecha";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar fechas: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarSelecciones())
                return;

            try
            {
                int idCarrera = Convert.ToInt32(cbCarrera.SelectedValue);
                int idMateria = Convert.ToInt32(cbMateria.SelectedValue);
                int idGestion = Convert.ToInt32(cbGestion.SelectedValue);
                DateTime fecha = Convert.ToDateTime(cbFecha.SelectedValue);

                DataTable reporte = objReporteNegocio.GenerarReporteAsistencia(idCarrera, idMateria, idGestion, fecha);

                if (reporte.Rows.Count > 0)
                {
                    dgReporte.ItemsSource = reporte.DefaultView;

                    string carrera = cbCarrera.Text;
                    string materia = cbMateria.Text;
                    string gestion = cbGestion.Text;
                    string fechaStr = fecha.ToString("dd/MM/yyyy");

                    lblInfoReporte.Text = $"📈 Reporte: {materia} - {carrera} - {gestion} - {fechaStr}";
                    lblTotalEstudiantes.Text = $"Total de estudiantes: {reporte.Rows.Count}";

                    // Calcular estadísticas de asistencia
                    int presentes = 0;
                    int ausentes = 0;
                    int sinRegistro = 0;

                    foreach (DataRow row in reporte.Rows)
                    {
                        string asistencia = row["Asistencia"].ToString();
                        if (asistencia == "P")
                            presentes++;
                        else if (asistencia == "F")
                            ausentes++;
                        else if (asistencia == "N/R")
                            sinRegistro++;
                    }

                    double porcentajeAsistencia = reporte.Rows.Count > 0 ?
                        (double)presentes / reporte.Rows.Count * 100 : 0;

                    string estadisticas = $"Presentes: {presentes} | Ausentes: {ausentes}";
                    if (sinRegistro > 0)
                        estadisticas += $" | Sin registro: {sinRegistro}";
                    estadisticas += $" | % Asistencia: {porcentajeAsistencia:F1}%";

                    lblEstadisticasAsistencia.Text = estadisticas;

                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    dgReporte.ItemsSource = null;
                    lblInfoReporte.Text = "❌ No se encontraron estudiantes matriculados para los criterios seleccionados";
                    lblTotalEstudiantes.Text = "";
                    lblEstadisticasAsistencia.Text = "";
                    btnImprimir.IsEnabled = false;

                    MessageBox.Show("No se encontraron estudiantes matriculados para los criterios seleccionados.",
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

            if (cbFecha.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una fecha.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbFecha.Focus();
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
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument documento = CrearDocumentoImpresion();
                    documento.PageHeight = printDialog.PrintableAreaHeight;
                    documento.PageWidth = printDialog.PrintableAreaWidth;
                    documento.PagePadding = new Thickness(50);
                    documento.ColumnGap = 0;
                    documento.ColumnWidth = printDialog.PrintableAreaWidth;

                    IDocumentPaginatorSource idocument = documento as IDocumentPaginatorSource;
                    printDialog.PrintDocument(idocument.DocumentPaginator, "Reporte de Asistencia");

                    MessageBox.Show("Documento enviado a la impresora correctamente.", "Impresión",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error de Impresión",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CrearDocumentoImpresion()
        {
            FlowDocument documento = new FlowDocument();

            Paragraph titulo = new Paragraph(new Run("REPORTE DE ASISTENCIA DE ESTUDIANTES"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            documento.Blocks.Add(titulo);

            Paragraph info = new Paragraph();
            info.Inlines.Add(new Run($"Carrera: {cbCarrera.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Materia: {cbMateria.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Gestión: {cbGestion.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Fecha: {Convert.ToDateTime(cbFecha.SelectedValue):dd/MM/yyyy}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}"));
            info.Margin = new Thickness(0, 0, 0, 20);
            documento.Blocks.Add(info);

            // Estadísticas
            Paragraph estadisticas = new Paragraph(new Run(lblEstadisticasAsistencia.Text))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = Brushes.DarkBlue
            };
            documento.Blocks.Add(estadisticas);

            // Tabla de asistencia
            Table tabla = new Table() { CellSpacing = 0 };
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(3, GridUnitType.Star) }); // Nombre
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) }); // CI
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) }); // RU
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) }); // Asistencia

            TableRowGroup grupoFilas = new TableRowGroup();

            // Encabezado
            TableRow encabezado = new TableRow();
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Nombre Completo"))
            { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("CI"))
            { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("RU"))
            { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Asistencia"))
            { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Background = Brushes.LightGray;

            foreach (TableCell celda in encabezado.Cells)
            {
                celda.BorderBrush = Brushes.Black;
                celda.BorderThickness = new Thickness(1);
                celda.Padding = new Thickness(5);
            }

            grupoFilas.Rows.Add(encabezado);

            // Filas de datos
            if (dgReporte.ItemsSource is DataView vista)
            {
                foreach (DataRowView fila in vista)
                {
                    TableRow filaTabla = new TableRow();
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["NombreCompleto"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["CI"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["RU"].ToString()))));

                    // Celda de asistencia con color
                    string asistencia = fila["Asistencia"].ToString();
                    Run runAsistencia = new Run(asistencia);
                    if (asistencia == "P")
                        runAsistencia.Foreground = Brushes.Green;
                    else if (asistencia == "F")
                        runAsistencia.Foreground = Brushes.Red;
                    else if (asistencia == "N/R")
                        runAsistencia.Foreground = Brushes.Orange;

                    runAsistencia.FontWeight = FontWeights.Bold;
                    filaTabla.Cells.Add(new TableCell(new Paragraph(runAsistencia)
                    { TextAlignment = TextAlignment.Center }));

                    foreach (TableCell celda in filaTabla.Cells)
                    {
                        celda.BorderBrush = Brushes.Gray;
                        celda.BorderThickness = new Thickness(0.5);
                        celda.Padding = new Thickness(5);
                    }

                    grupoFilas.Rows.Add(filaTabla);
                }
            }

            tabla.RowGroups.Add(grupoFilas);
            documento.Blocks.Add(tabla);

            return documento;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            cbCarrera.SelectedIndex = -1;
            cbMateria.SelectedIndex = -1;
            cbGestion.SelectedIndex = -1;
            cbFecha.SelectedIndex = -1;

            dgReporte.ItemsSource = null;
            lblInfoReporte.Text = "Seleccione los filtros y genere el reporte";
            lblTotalEstudiantes.Text = "";
            lblEstadisticasAsistencia.Text = "";

            btnImprimir.IsEnabled = false;
        }
    }
}