using Capa_Negocios;
using Capa_Presentacion;
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
    /// Lógica de interacción para Inscripcion.xaml
    /// </summary>
    public partial class InscripcionUser : UserControl
    {
        Inscripcion objInscripcion = new Inscripcion();

        public InscripcionUser()
        {
            InitializeComponent();
            Loaded += MainWindow_Load;
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            Gestion objGestion = new Gestion();
            cbGestion.ItemsSource = objGestion.ObtenerTodas().DefaultView;
            cbGestion.DisplayMemberPath = "Gestion";
            cbGestion.SelectedValuePath = "codGestion";
            cbGestion.SelectedIndex = -1;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegistro.Text) || cbGestion.SelectedIndex == -1)
            {
                MessageBox.Show("Debe ingresar el registro del estudiante y seleccionar una gestión.");
                return;
            }

            int idEstudiante = int.Parse(txtRegistro.Text);
            int codGestion = Convert.ToInt32(cbGestion.SelectedValue);

            Estudiante objEst = new Estudiante { IdEstudiante = idEstudiante };
            lblNombre.Text = objEst.ObtenerNombreCompleto();

            objInscripcion.IdEstudiante = idEstudiante;

            lstMateriasInscritas.ItemsSource = objInscripcion.ObtenerMateriasInscritas(codGestion).DefaultView;
            lstMateriasInscritas.DisplayMemberPath = "Materia";
            lstMateriasInscritas.SelectedValuePath = "codEd";

            CargarMateriasOfertadas();
        }

        private void CargarMateriasOfertadas()
        {
            int idEstudiante = int.Parse(txtRegistro.Text);
            int codGestion = Convert.ToInt32(cbGestion.SelectedValue);

            objInscripcion.IdEstudiante = idEstudiante;

            try
            {
                DataTable tabla = objInscripcion.ObtenerMateriasOfertadas(codGestion);

                // SOLUCIÓN: Agregar la columna Seleccionar al DataTable
                if (!tabla.Columns.Contains("Seleccionar"))
                {
                    tabla.Columns.Add("Seleccionar", typeof(bool));
                    // Inicializar todos los valores como false
                    foreach (DataRow row in tabla.Rows)
                    {
                        row["Seleccionar"] = false;
                    }
                }

                // Limpiar columnas existentes del DataGrid
                dgvMateriasOfertadas.Columns.Clear();

                // Asignar el DataSource
                dgvMateriasOfertadas.ItemsSource = tabla.DefaultView;
                dgvMateriasOfertadas.AutoGenerateColumns = true;

                // Configurar la columna de selección para que aparezca primero
                dgvMateriasOfertadas.LoadingRow += (s, e) =>
                {
                    // Este evento se ejecuta cuando se cargan las filas
                };

                // Alternativa: Configurar columnas manualmente
                ConfigurarColumnasDataGrid(tabla);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materias ofertadas: " + ex.Message);
            }
        }

        private void ConfigurarColumnasDataGrid(DataTable tabla)
        {
            dgvMateriasOfertadas.AutoGenerateColumns = false;
            dgvMateriasOfertadas.Columns.Clear();

            // Columna de selección (Checkbox)
            DataGridCheckBoxColumn chkColumn = new DataGridCheckBoxColumn
            {
                Header = "Seleccionar",
                Binding = new System.Windows.Data.Binding("Seleccionar"),
                Width = 100
            };
            dgvMateriasOfertadas.Columns.Add(chkColumn);

            // Columna de Materia
            DataGridTextColumn materiaColumn = new DataGridTextColumn
            {
                Header = "Materia",
                Binding = new System.Windows.Data.Binding("Materia"),
                Width = 200,
                IsReadOnly = true
            };
            dgvMateriasOfertadas.Columns.Add(materiaColumn);

            // Columna de Grupo
            DataGridTextColumn grupoColumn = new DataGridTextColumn
            {
                Header = "Grupo",
                Binding = new System.Windows.Data.Binding("Grupo"),
                Width = 100,
                IsReadOnly = true
            };
            dgvMateriasOfertadas.Columns.Add(grupoColumn);

            // Columna de Docente
            DataGridTextColumn docenteColumn = new DataGridTextColumn
            {
                Header = "Docente",
                Binding = new System.Windows.Data.Binding("Docente"),
                Width = 200,
                IsReadOnly = true
            };
            dgvMateriasOfertadas.Columns.Add(docenteColumn);

            // Columna oculta para codEd
            DataGridTextColumn codEdColumn = new DataGridTextColumn
            {
                Header = "codEd",
                Binding = new System.Windows.Data.Binding("codEd"),
                Visibility = Visibility.Hidden
            };
            dgvMateriasOfertadas.Columns.Add(codEdColumn);
        }

        private void btnInscribir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegistro.Text) || cbGestion.SelectedIndex == -1)
            {
                MessageBox.Show("Debe ingresar el registro del estudiante y la gestión.");
                return;
            }

            int idEstudiante = int.Parse(txtRegistro.Text);
            objInscripcion.IdEstudiante = idEstudiante;

            List<int> materiasSeleccionadas = new List<int>();

            // SOLUCIÓN MEJORADA: Verificar que ItemsSource no sea null
            if (dgvMateriasOfertadas.ItemsSource != null)
            {
                foreach (DataRowView row in dgvMateriasOfertadas.ItemsSource)
                {
                    if (row.Row.Table.Columns.Contains("Seleccionar") &&
                        row["Seleccionar"] != DBNull.Value &&
                        (bool)row["Seleccionar"] == true)
                    {
                        materiasSeleccionadas.Add(Convert.ToInt32(row["codEd"]));
                    }
                }
            }

            if (materiasSeleccionadas.Count == 0)
            {
                MessageBox.Show("Debe seleccionar al menos una materia.");
                return;
            }
            else if (materiasSeleccionadas.Count > 6)
            {
                MessageBox.Show("No puede inscribirse en más de 6 materias.");
                return;
            }

            string listaCodEd = string.Join(",", materiasSeleccionadas);

            try
            {
                bool exito = objInscripcion.Inscribir(listaCodEd);
                if (exito)
                {
                    MessageBox.Show("Materias inscritas correctamente.");
                    btnBuscar_Click(null, null);
                }
                else
                {
                    MessageBox.Show("No se pudo inscribir al estudiante.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inscribir: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lstMateriasInscritas.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una materia para eliminar.");
                return;
            }

            int idEstudiante = int.Parse(txtRegistro.Text);
            objInscripcion.IdEstudiante = idEstudiante;

            int codEd = Convert.ToInt32(lstMateriasInscritas.SelectedValue);

            if (MessageBox.Show("¿Está seguro que desea eliminar esta inscripción?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    objInscripcion.CodEd = codEd;
                    bool exito = objInscripcion.Eliminar();
                    if (exito)
                    {
                        MessageBox.Show("Inscripción eliminada correctamente.");
                        btnBuscar_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la inscripción.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar inscripción: " + ex.Message);
                }
            }
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (lstMateriasInscritas.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione una materia ya inscrita que desea modificar.");
                return;
            }

            int idEstudiante = int.Parse(txtRegistro.Text);
            objInscripcion.IdEstudiante = idEstudiante;

            int codEdActual = Convert.ToInt32(lstMateriasInscritas.SelectedValue);
            int? codEdNueva = null;

            // SOLUCIÓN MEJORADA: Verificar que ItemsSource no sea null
            if (dgvMateriasOfertadas.ItemsSource != null)
            {
                foreach (DataRowView row in dgvMateriasOfertadas.ItemsSource)
                {
                    if (row["Seleccionar"] != DBNull.Value && (bool)row["Seleccionar"] == true)
                    {
                        if (codEdNueva != null)
                        {
                            MessageBox.Show("Solo debe seleccionar una nueva materia para reemplazar.");
                            return;
                        }
                        codEdNueva = Convert.ToInt32(row["codEd"]);
                    }
                }
            }

            if (codEdNueva == null)
            {
                MessageBox.Show("Debe seleccionar una nueva materia de reemplazo.");
                return;
            }

            if (MessageBox.Show("¿Está seguro de reemplazar esta inscripción?", "Confirmar modificación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    bool exito = objInscripcion.ActualizarEdicion(codEdActual, codEdNueva.Value);
                    if (exito)
                    {
                        MessageBox.Show("Inscripción modificada correctamente.");
                        btnBuscar_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo modificar la inscripción.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al modificar inscripción: " + ex.Message);
                }
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            txtRegistro.Clear();
            cbGestion.SelectedIndex = -1;
            lblNombre.Text = "";

            lstMateriasInscritas.ItemsSource = null;
            dgvMateriasOfertadas.ItemsSource = null;
            dgvMateriasOfertadas.Columns.Clear();

            objInscripcion = new Inscripcion();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAsistencia_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnnota_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
