using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteAsistenciaNegocio
    {
        private string connectionString;

        public ReporteAsistenciaNegocio()
        {
            connectionString = new Capa_Datos.gDatos().ObtenerCadenaConexion();
        }

        // Obtener todas las carreras
        public DataTable ObtenerCarreras()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerCarreras", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        // Obtener materias por carrera
        public DataTable ObtenerMateriasPorCarrera(int idCarrera)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerMateriasPorCarrera", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        // Obtener fechas de asistencia por materia y gestión
        public DataTable ObtenerFechasAsistencia(int idMateria, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerFechasAsistencia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_materia", idMateria);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        // Generar reporte de asistencia
        public DataTable GenerarReporteAsistencia(int idCarrera, int idMateria, int idGestion, DateTime fecha)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ReporteAsistencia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    cmd.Parameters.AddWithValue("@id_materia", idMateria);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }
    }
}