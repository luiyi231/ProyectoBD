using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Negocios
{
    public class Inscripcion
    {
        private int idEstudiante;
        private int codEd;
        private string connectionString = new Capa_Datos.gDatos().ObtenerCadenaConexion();

        public int IdEstudiante
        {
            get => idEstudiante;
            set => idEstudiante = value;
        }

        public int CodEd
        {
            get => codEd;
            set => codEd = value;
        }

        public bool Inscribir(string listaCodEd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InscribirMaterias", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@lista_codEd", listaCodEd);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public bool Eliminar()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_EliminarInscripcion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@codEd", codEd);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public bool ActualizarEdicion(int codEdActual, int codEdNueva)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarInscripcionEdicion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@codEdActual", codEdActual);
                    cmd.Parameters.AddWithValue("@codEdNueva", codEdNueva);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public DataTable ObtenerMateriasInscritas(int codGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ListarMateriasInscritas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@codGestion", codGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable ObtenerMateriasOfertadas(int codGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_MateriasOfertadasParaEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@codGestion", codGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }
    }
}
