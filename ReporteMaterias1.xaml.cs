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
    /// Lógica de interacción para ReporteMaterias1.xaml
    /// </summary>
    public partial class ReporteMaterias1 : UserControl
    {
        private Capa_Negocios.ReporteMateriasNegocio objReporteNegocio;
        private Gestion objGestion;

        public ReporteMaterias1()
        {
            InitializeComponent();

            objReporteNegocio = new Capa_Negocios.ReporteMateriasNegocio();
            objGestion = new Gestion();

            Loaded += ReporteMaterias_Loaded;
        }

        private void ReporteMaterias_Loaded(object sender, RoutedEventArgs e)
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

                cbPlanEstudio.ItemsSource = null;
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
                    DataTable planes = objReporteNegocio.ObtenerPlanesPorCarrera(idCarrera);
                    cbPlanEstudio.ItemsSource = planes.DefaultView;

                    dgReporte.ItemsSource = null;
                    lblTotalMaterias.Text = "";
                    lblInfoReporte.Text = "Seleccione el plan de estudios y la gestión, luego genere el reporte";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar planes de estudio: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                cbPlanEstudio.ItemsSource = null;
            }
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarSelecciones())
                return;

            try
            {
                int idCarrera = Convert.ToInt32(cbCarrera.SelectedValue);
                int idPlanEstudio = Convert.ToInt32(cbPlanEstudio.SelectedValue);
                int idGestion = Convert.ToInt32(cbGestion.SelectedValue);

                DataTable reporte = objReporteNegocio.GenerarReporte(idCarrera, idPlanEstudio, idGestion);

                // Agregar columna Cupo con valor fijo 20
                if (reporte.Columns["Cupo"] == null)
                {
                    reporte.Columns.Add("Cupo", typeof(int));
                }

                foreach (DataRow row in reporte.Rows)
                {
                    row["Cupo"] = 20;
                }

                if (reporte.Rows.Count > 0)
                {
                    dgReporte.ItemsSource = reporte.DefaultView;

                    string carrera = cbCarrera.Text;
                    string plan = cbPlanEstudio.Text;
                    string gestion = cbGestion.Text;

                    lblInfoReporte.Text = $"📈 Reporte: {carrera} - {plan} - {gestion}";
                    lblTotalMaterias.Text = $"Total de materias ofertadas: {reporte.Rows.Count}";

                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    dgReporte.ItemsSource = null;
                    lblInfoReporte.Text = "❌ No se encontraron materias ofertadas para los criterios seleccionados";
                    lblTotalMaterias.Text = "";
                    btnImprimir.IsEnabled = false;

                    MessageBox.Show("No se encontraron materias ofertadas para los criterios seleccionados.",
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

            if (cbPlanEstudio.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un plan de estudios.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPlanEstudio.Focus();
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
                    printDialog.PrintDocument(idocument.DocumentPaginator, "Reporte de Materias Ofertadas");

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

            Paragraph titulo = new Paragraph(new Run("REPORTE DE MATERIAS OFERTADAS"))
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
            info.Inlines.Add(new Run($"Plan de Estudios: {cbPlanEstudio.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Gestión: {cbGestion.Text}") { FontWeight = FontWeights.Bold });
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}"));
            info.Margin = new Thickness(0, 0, 0, 20);
            documento.Blocks.Add(info);

            // Tabla SIN columna de estudiantes
            Table tabla = new Table() { CellSpacing = 0 };
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2.5, GridUnitType.Star) }); // Materia
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });   // Grupo
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });   // Docente
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(3, GridUnitType.Star) });   // Horarios
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(0.8, GridUnitType.Star) }); // Cupo

            TableRowGroup grupoFilas = new TableRowGroup();

            // Encabezado CON columna de cupo
            TableRow encabezado = new TableRow();
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Materia")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Grupo")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Docente")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Horarios")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Cells.Add(new TableCell(new Paragraph(new Run("Cupo")) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center }));
            encabezado.Background = Brushes.LightGray;

            foreach (TableCell celda in encabezado.Cells)
            {
                celda.BorderBrush = Brushes.Black;
                celda.BorderThickness = new Thickness(1);
                celda.Padding = new Thickness(5);
            }

            grupoFilas.Rows.Add(encabezado);

            // Filas de datos CON columna de cupo
            if (dgReporte.ItemsSource is DataView vista)
            {
                foreach (DataRowView fila in vista)
                {
                    TableRow filaTabla = new TableRow();
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["Materia"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["Grupo"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["Docente"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["Horarios"].ToString()))));
                    filaTabla.Cells.Add(new TableCell(new Paragraph(new Run(fila["Cupo"].ToString())) { TextAlignment = TextAlignment.Center }));

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
            cbPlanEstudio.SelectedIndex = -1;
            cbGestion.SelectedIndex = -1;

            dgReporte.ItemsSource = null;
            lblInfoReporte.Text = "Seleccione los filtros y genere el reporte";
            lblTotalMaterias.Text = "";

            btnImprimir.IsEnabled = false;
        }
    }
}
