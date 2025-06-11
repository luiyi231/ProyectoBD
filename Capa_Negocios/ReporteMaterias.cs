using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteMateriasNegocio  // Cambié el nombre para evitar conflicto
    {
        private string connectionString;

        public ReporteMateriasNegocio()
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

        // Obtener planes de estudio por carrera
        public DataTable ObtenerPlanesPorCarrera(int idCarrera)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPlanesPorCarrera", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        // Generar reporte de materias ofertadas
        public DataTable GenerarReporte(int idCarrera, int idPlanEstudio, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ReporteMaterias_Ofertadas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    cmd.Parameters.AddWithValue("@id_plan_estudio", idPlanEstudio);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }
    }
}