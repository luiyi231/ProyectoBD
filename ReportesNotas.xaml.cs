using Capa_Negocios;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ProyectoBD
{
    /// <summary>
    /// </summary>
    public partial class ReportesNotas : UserControl
    {
        private ReporteNotasEstudianteNegocio objReporteNegocio;
        private int idEstudianteSeleccionado;

        public ReportesNotas()
        {
            InitializeComponent();
            objReporteNegocio = new ReporteNotasEstudianteNegocio();
            Loaded += ReporteMateriasEstudiante_Loaded;
        }

        private void ReporteMateriasEstudiante_Loaded(object sender, RoutedEventArgs e)
        {
            lblFechaReporte.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm}";
            txtRU.Focus();
        }

        private void txtRU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnBuscar_Click(sender, e);
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRU.Text))
            {
                MessageBox.Show("Debe ingresar un número de registro (RU).", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRU.Focus();
                return;
            }

            try
            {
                // Buscar estudiante
                DataTable estudiante = objReporteNegocio.BuscarEstudiantePorRU(txtRU.Text.Trim());

                if (estudiante.Rows.Count > 0)
                {
                    // Mostrar información del estudiante
                    DataRow row = estudiante.Rows[0];
                    idEstudianteSeleccionado = Convert.ToInt32(row["ID_Estudiante"]);

                    // Construir nombre completo a partir de nombre y apellido
                    string nombreCompleto = $"{row["Nombre"]} {row["Apellido"]}";

                    lblNombreEstudiante.Text = $"👤 {nombreCompleto}";
                    lblCarreraEstudiante.Text = $"🎓 {row["NombreCarrera"]} - CI: {row["CI"]}";

                    // Mostrar panel de información
                    borderInfoEstudiante.Visibility = Visibility.Visible;
                    panelBotones.Visibility = Visibility.Visible;

                    // Limpiar reporte anterior
                    dgReporte.ItemsSource = null;
                    borderEstadisticas.Visibility = Visibility.Collapsed;
                    btnImprimir.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show($"No se encontró ningún estudiante con el RU: {txtRU.Text}",
                        "Estudiante no encontrado", MessageBoxButton.OK, MessageBoxImage.Information);

                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el estudiante: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (idEstudianteSeleccionado == 0)
            {
                MessageBox.Show("Debe buscar un estudiante primero.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Generar reporte de materias
                DataTable reporte = objReporteNegocio.GenerarReporteMaterias(idEstudianteSeleccionado);

                if (reporte.Rows.Count > 0)
                {
                    dgReporte.ItemsSource = reporte.DefaultView;

                    // Calcular estadísticas
                    CalcularEstadisticas(reporte);

                    // Mostrar estadísticas
                    borderEstadisticas.Visibility = Visibility.Visible;
                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    dgReporte.ItemsSource = null;
                    borderEstadisticas.Visibility = Visibility.Collapsed;
                    btnImprimir.IsEnabled = false;

                    MessageBox.Show("El estudiante no tiene materias registradas.",
                        "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalcularEstadisticas(DataTable reporte)
        {
            int totalMaterias = reporte.Rows.Count;
            int materiasAprobadas = 0;
            int materiasReprobadas = 0;
            double sumaNotas = 0;
            int materiasConNota = 0;

            foreach (DataRow row in reporte.Rows)
            {
                string estado = row["Estado"].ToString();

                if (estado == "APROBADO")
                    materiasAprobadas++;
                else if (estado == "REPROBADO")
                    materiasReprobadas++;

                // Calcular promedio solo con materias que tienen nota
                if (row["NotaFinal"] != DBNull.Value)
                {
                    double nota = Convert.ToDouble(row["NotaFinal"]);
                    if (nota > 0)
                    {
                        sumaNotas += nota;
                        materiasConNota++;
                    }
                }
            }

            double promedioGeneral = materiasConNota > 0 ? sumaNotas / materiasConNota : 0;

            // Actualizar labels
            lblTotalMaterias.Text = $"📊 Total de materias: {totalMaterias}";
            lblMateriasAprobadas.Text = $"✅ Aprobadas: {materiasAprobadas}";
            lblMateriasReprobadas.Text = $"❌ Reprobadas: {materiasReprobadas}";
            lblPromedioGeneral.Text = $"📈 Promedio general: {promedioGeneral:F2}";
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
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Reporte de Materias del Estudiante");

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
            Paragraph titulo = new Paragraph(new Run("REPORTE DE MATERIAS CURSADAS POR ESTUDIANTE"))
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
            infoEstudiante.Inlines.Add(new Run("INFORMACIÓN DEL ESTUDIANTE") { FontWeight = FontWeights.Bold, FontSize = 14 });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"Nombre: {lblNombreEstudiante.Text.Replace("👤 ", "")}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"Carrera: {lblCarreraEstudiante.Text.Replace("🎓 ", "")}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"RU: {txtRU.Text}") { FontWeight = FontWeights.Bold });
            infoEstudiante.Inlines.Add(new LineBreak());
            infoEstudiante.Inlines.Add(new Run($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}"));
            flowDocument.Blocks.Add(infoEstudiante);

            // Estadísticas
            if (borderEstadisticas.Visibility == Visibility.Visible)
            {
                Paragraph estadisticas = new Paragraph()
                {
                    Margin = new Thickness(0, 0, 0, 20)
                };
                estadisticas.Inlines.Add(new Run("ESTADÍSTICAS") { FontWeight = FontWeights.Bold, FontSize = 14 });
                estadisticas.Inlines.Add(new LineBreak());
                estadisticas.Inlines.Add(new Run(lblTotalMaterias.Text.Replace("📊 ", "")) { FontWeight = FontWeights.SemiBold });
                estadisticas.Inlines.Add(new LineBreak());
                estadisticas.Inlines.Add(new Run(lblMateriasAprobadas.Text.Replace("✅ ", "")) { FontWeight = FontWeights.SemiBold });
                estadisticas.Inlines.Add(new LineBreak());
                estadisticas.Inlines.Add(new Run(lblMateriasReprobadas.Text.Replace("❌ ", "")) { FontWeight = FontWeights.SemiBold });
                estadisticas.Inlines.Add(new LineBreak());
                estadisticas.Inlines.Add(new Run(lblPromedioGeneral.Text.Replace("📈 ", "")) { FontWeight = FontWeights.SemiBold });
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
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Código
            tabla.Columns.Add(new TableColumn { Width = new GridLength(2.5, GridUnitType.Star) }); // Materia
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Gestión
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.6, GridUnitType.Star) }); // Grupo
            tabla.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) }); // Docente
            tabla.Columns.Add(new TableColumn { Width = new GridLength(0.8, GridUnitType.Star) }); // Nota Final
            tabla.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) }); // Estado
            tabla.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) }); // Observaciones

            // Crear grupo de filas
            TableRowGroup tableRowGroup = new TableRowGroup();

            // Fila de encabezados
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGray;

            string[] headers = { "Código", "Materia", "Gestión", "Grupo", "Docente", "Nota", "Estado", "Observaciones" };
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

                    // Código
                    dataRow.Cells.Add(CrearCelda(row["CodigoMateria"].ToString(), TextAlignment.Center));
                    // Materia
                    dataRow.Cells.Add(CrearCelda(row["NombreMateria"].ToString()));
                    // Gestión
                    dataRow.Cells.Add(CrearCelda(row["Gestion"].ToString(), TextAlignment.Center));
                    // Grupo
                    dataRow.Cells.Add(CrearCelda(row["Grupo"].ToString(), TextAlignment.Center));
                    // Docente
                    dataRow.Cells.Add(CrearCelda(row["NombreDocente"].ToString()));
                    // Nota Final
                    string nota = row["NotaFinal"] == DBNull.Value ? "-" : row["NotaFinal"].ToString();
                    dataRow.Cells.Add(CrearCelda(nota, TextAlignment.Center));
                    // Estado
                    string estado = row["Estado"].ToString();
                    TableCell celdaEstado = CrearCelda(estado, TextAlignment.Center);

                    // Colorear según el estado
                    if (estado == "APROBADO")
                        celdaEstado.Background = Brushes.LightGreen;
                    else if (estado == "REPROBADO")
                        celdaEstado.Background = Brushes.LightCoral;
                    else if (estado == "EN CURSO")
                        celdaEstado.Background = Brushes.LightYellow;

                    dataRow.Cells.Add(celdaEstado);
                    // Observaciones
                    dataRow.Cells.Add(CrearCelda(row["Observaciones"].ToString()));

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
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            try
            {
                // Limpiar campos
                txtRU.Text = "";
                idEstudianteSeleccionado = 0;

                // Ocultar paneles
                borderInfoEstudiante.Visibility = Visibility.Collapsed;
                panelBotones.Visibility = Visibility.Collapsed;
                borderEstadisticas.Visibility = Visibility.Collapsed;

                // Limpiar datos
                dgReporte.ItemsSource = null;

                // Deshabilitar botón de imprimir
                btnImprimir.IsEnabled = false;

                // Actualizar fecha
                lblFechaReporte.Text = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm}";

                // Enfocar campo RU
                txtRU.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar el formulario: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}