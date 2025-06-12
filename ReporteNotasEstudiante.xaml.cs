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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoBD
{
    /// <summary>
    /// Lógica de interacción para ReporteNotasEstudiante1.xaml
    /// </summary>
    public partial class ReporteNotasEstudiante : UserControl
    {
        private ReporteNotasEstudianteNegocio objReporteNegocio;
        private int idEstudianteActual = 0;

        public ReporteNotasEstudiante ()
        {
            InitializeComponent();
            objReporteNegocio = new ReporteNotasEstudianteNegocio();
            Loaded += ReporteNotasEstudiante_Loaded;
        }

        private void ReporteNotasEstudiante_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosIniciales();
            lblFechaReporte.Text = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        private void CargarDatosIniciales()
        {
            try
            {
                // Cargar gestiones
                DataTable gestiones = objReporteNegocio.ObtenerGestiones();
                cbGestion.ItemsSource = gestiones.DefaultView;

                // Limpiar otros controles
                LimpiarDatosEstudiante();
                dgReporte.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos iniciales: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtRU_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Limpiar datos del estudiante cuando cambie el RU
            if (string.IsNullOrWhiteSpace(txtRU.Text))
            {
                LimpiarDatosEstudiante();
                btnGenerar.IsEnabled = false;
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRU.Text))
            {
                MessageBox.Show("Debe ingresar el número de registro (RU) del estudiante.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRU.Focus();
                return;
            }

            try
            {
                // Buscar información del estudiante
                DataTable estudiante = objReporteNegocio.BuscarEstudiantePorRU(txtRU.Text.Trim());

                if (estudiante.Rows.Count > 0)
                {
                    DataRow row = estudiante.Rows[0];
                    idEstudianteActual = Convert.ToInt32(row["ID_Estudiante"]);

                    // Llenar datos del estudiante
                    txtNombreCompleto.Text = $"{row["Nombre"]} {row["Apellido"]}";
                    txtCI.Text = row["CI"].ToString();
                    txtCarrera.Text = row["NombreCarrera"].ToString();

                    btnGenerar.IsEnabled = true;
                    lblInfoReporte.Text = "✅ Estudiante encontrado. Seleccione la gestión y genere el reporte.";
                }
                else
                {
                    LimpiarDatosEstudiante();
                    MessageBox.Show("No se encontró ningún estudiante con el RU especificado.",
                        "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el estudiante: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método mejorado para GenerarReporteNotas con debug


        // Método modificado en el evento btnGenerar_Click
        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatos())
                return;

            try
            {
                int idGestion = Convert.ToInt32(cbGestion.SelectedValue);

                // DEBUG: Mostrar los IDs que se están usando
                MessageBox.Show($"Debug Info:\nID Estudiante: {idEstudianteActual}\nID Gestión: {idGestion}",
                    "Debug", MessageBoxButton.OK, MessageBoxImage.Information);

                // Primero intentar obtener todas las notas del estudiante
                DataTable todasNotas = objReporteNegocio.ObtenerTodasNotasEstudiante(idEstudianteActual);

                if (todasNotas.Rows.Count > 0)
                {
                    // Mostrar todas las notas primero para debug
                    string debug = "Notas encontradas:\n";
                    foreach (DataRow row in todasNotas.Rows)
                    {
                        debug += $"- {row["NombreMateria"]} ({row["GestionCompleta"]}) - Gestión ID: {row["ID_Gestion"]}\n";
                    }
                    MessageBox.Show(debug, "Debug - Todas las notas", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Ahora generar reporte de la gestión específica
                DataTable reporte = objReporteNegocio.GenerarReporteNotas(idEstudianteActual, idGestion);

                if (reporte.Rows.Count > 0)
                {
                    dgReporte.ItemsSource = reporte.DefaultView;

                    // Calcular estadísticas
                    int totalMaterias = reporte.Rows.Count;
                    int materiasAprobadas = reporte.AsEnumerable()
                        .Count(row => row.Field<string>("Estado") == "Aprobado");
                    int materiasReprobadas = reporte.AsEnumerable()
                        .Count(row => row.Field<string>("Estado") == "Reprobado");
                    int materiasSinNota = reporte.AsEnumerable()
                        .Count(row => row.Field<string>("Estado") == "Sin Nota");

                    // Calcular promedio general (solo de materias con nota)
                    var materiasConNota = reporte.AsEnumerable()
                        .Where(row => !string.IsNullOrEmpty(row.Field<string>("NotaFinal")) &&
                                     row.Field<string>("NotaFinal") != "Sin Nota");

                    double promedioGeneral = 0;
                    if (materiasConNota.Any())
                    {
                        promedioGeneral = materiasConNota
                            .Average(row => Convert.ToDouble(row.Field<string>("NotaFinal")));
                    }

                    // Actualizar información
                    string gestion = cbGestion.Text;
                    lblInfoReporte.Text = $"📈 Reporte: {txtNombreCompleto.Text} - {gestion}";

                    string estadisticas = $"Total: {totalMaterias} | Aprobadas: {materiasAprobadas} | " +
                                        $"Reprobadas: {materiasReprobadas} | Sin Nota: {materiasSinNota}";

                    if (materiasConNota.Any())
                    {
                        estadisticas += $" | Promedio General: {promedioGeneral:F2}";
                    }

                    lblEstadisticas.Text = estadisticas;
                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    dgReporte.ItemsSource = null;
                    lblInfoReporte.Text = "❌ No se encontraron registros de materias para los criterios seleccionados";
                    lblEstadisticas.Text = "";
                    btnImprimir.IsEnabled = false;

                    // Mostrar información adicional para debug
                    string debugMsg = $"No se encontraron notas para:\n" +
                                    $"- Estudiante ID: {idEstudianteActual}\n" +
                                    $"- Gestión ID: {idGestion}\n" +
                                    $"- Gestión Texto: {cbGestion.Text}";

                    if (todasNotas.Rows.Count > 0)
                    {
                        debugMsg += $"\n\nPero el estudiante SÍ tiene notas en otras gestiones.\n" +
                                   $"Verifique que la gestión seleccionada sea correcta.";
                    }
                    else
                    {
                        debugMsg += $"\n\nEl estudiante NO tiene notas registradas en ninguna gestión.";
                    }

                    MessageBox.Show(debugMsg, "Debug - Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}\n\nDetalles: {ex.ToString()}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtRU.Text))
            {
                MessageBox.Show("Debe ingresar el número de registro (RU) del estudiante.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRU.Focus();
                return false;
            }

            if (idEstudianteActual == 0)
            {
                MessageBox.Show("Debe buscar al estudiante primero.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnBuscar.Focus();
                return false;
            }

            if (cbGestion.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una gestión.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbGestion.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarDatosEstudiante()
        {
            txtNombreCompleto.Text = "";
            txtCI.Text = "";
            txtCarrera.Text = "";
            idEstudianteActual = 0;
            btnGenerar.IsEnabled = false;
            btnImprimir.IsEnabled = false;
            dgReporte.ItemsSource = null;
            lblEstadisticas.Text = "";
            lblInfoReporte.Text = "Ingrese el RU del estudiante y seleccione la gestión";
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Limpiar todos los campos
                txtRU.Text = "";
                cbGestion.SelectedIndex = -1;
                LimpiarDatosEstudiante();

                // Actualizar fecha
                lblFechaReporte.Text = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar el formulario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Reporte de Notas del Estudiante");

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
            Paragraph titulo = new Paragraph(new Run("REPORTE DE NOTAS POR ESTUDIANTE"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            flowDocument.Blocks.Add(titulo);

            // Información del estudiante
            Paragraph infoEstudiante = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            infoEstudiante.Inlines.Add(new Run($"Estudiante: {txtNombreCompleto.Text}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"RU: {txtRU.Text}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"CI: {txtCI.Text}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"Carrera: {txtCarrera.Text}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"Gestión: {cbGestion.Text}") { FontWeight = FontWeights.Bold });
            flowDocument.Blocks.Add(infoEstudiante);

            // Crear tabla para las notas
            Table tabla = new Table();
            tabla.CellSpacing = 2;
            tabla.Background = Brushes.White;

            // Definir columnas
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(3, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            tabla.Columns.Add(new TableColumn() { Width = new GridLength(2, GridUnitType.Star) });

            // Crear grupo de filas
            TableRowGroup rowGroup = new TableRowGroup();

            // Encabezado de la tabla
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGray;
            headerRow.FontWeight = FontWeights.Bold;

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Materia"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Docente"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Grupo"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Gestión"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Nota Final"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Estado"))));

            rowGroup.Rows.Add(headerRow);

            // Agregar datos del DataGrid
            if (dgReporte.ItemsSource != null)
            {
                foreach (DataRowView rowView in dgReporte.ItemsSource)
                {
                    TableRow dataRow = new TableRow();

                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(rowView["NombreMateria"].ToString()))));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(rowView["NombreDocente"].ToString()))));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(rowView["NombreGrupo"].ToString()))));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(rowView["GestionCompleta"].ToString()))));

                    TableCell celdaNota = new TableCell(new Paragraph(new Run(rowView["NotaFinal"].ToString())));
                    TableCell celdaEstado = new TableCell(new Paragraph(new Run(rowView["Estado"].ToString())));

                    // Aplicar colores según el estado
                    string estado = rowView["Estado"].ToString();
                    if (estado == "Reprobado")
                    {
                        celdaNota.Background = Brushes.LightCoral;
                        celdaEstado.Background = Brushes.LightCoral;
                    }
                    else if (estado == "Aprobado")
                    {
                        celdaNota.Background = Brushes.LightGreen;
                        celdaEstado.Background = Brushes.LightGreen;
                    }
                    else
                    {
                        celdaNota.Background = Brushes.LightYellow;
                        celdaEstado.Background = Brushes.LightYellow;
                    }

                    dataRow.Cells.Add(celdaNota);
                    dataRow.Cells.Add(celdaEstado);

                    rowGroup.Rows.Add(dataRow);
                }
            }

            tabla.RowGroups.Add(rowGroup);
            flowDocument.Blocks.Add(tabla);

            // Agregar estadísticas
            if (!string.IsNullOrEmpty(lblEstadisticas.Text))
            {
                Paragraph estadisticas = new Paragraph(new Run(lblEstadisticas.Text))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 20, 0, 0),
                    TextAlignment = TextAlignment.Center
                };
                flowDocument.Blocks.Add(estadisticas);
            }

            // Pie de página
            Paragraph pieDocumento = new Paragraph(new Run($"Reporte generado el {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm}"))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            flowDocument.Blocks.Add(pieDocumento);

            return flowDocument;
        }
    }
}
