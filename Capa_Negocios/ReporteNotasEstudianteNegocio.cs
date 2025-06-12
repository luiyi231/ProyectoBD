using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteNotasEstudianteNegocio
    {
        private string connectionString;

        public ReporteNotasEstudianteNegocio()
        {
            connectionString = new Capa_Datos.gDatos().ObtenerCadenaConexion();
        }

        /// <summary>
        /// Obtiene todas las gestiones disponibles
        /// </summary>
        /// <returns>DataTable con las gestiones</returns>
        public DataTable ObtenerGestiones()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerGestiones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        /// <summary>
        /// Busca un estudiante por su número de registro (RU)
        /// </summary>
        /// <param name="ru">Número de registro del estudiante</param>
        /// <returns>DataTable con la información del estudiante</returns>
        public DataTable BuscarEstudiantePorRU(string ru)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_BuscarEstudiantePorRU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RU", ru);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        /// <summary>
        /// Genera el reporte de notas para un estudiante en una gestión específica
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <param name="idGestion">ID de la gestión</param>
        /// <returns>DataTable con el reporte de notas</returns>
        

        /// <summary>
        /// Obtiene las estadísticas generales de un estudiante en una gestión
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <param name="idGestion">ID de la gestión</param>
        /// <returns>DataTable con las estadísticas</returns>
        public DataTable ObtenerEstadisticasEstudiante(int idEstudiante, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerEstadisticasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@ID_Gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        /// <summary>
        /// Verifica si un estudiante existe por su RU
        /// </summary>
        /// <param name="ru">Número de registro del estudiante</param>
        /// <returns>True si existe, False si no existe</returns>
        public bool ExisteEstudiante(string ru)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_VerificarExistenciaEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RU", ru);
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }
        public DataTable GenerarReporteNotas(int idEstudiante, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Primero verificar los datos con el procedimiento de debug
                using (SqlCommand cmdDebug = new SqlCommand("sp_VerificarDatosEstudiante", conn))
                {
                    cmdDebug.CommandType = CommandType.StoredProcedure;
                    cmdDebug.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmdDebug.Parameters.AddWithValue("@ID_Gestion", idGestion);

                    // Esto te ayudará a ver qué datos existen en la consola de SQL Server
                    try
                    {
                        cmdDebug.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en debug: {ex.Message}");
                    }
                }

                // Ahora ejecutar el reporte principal
                using (SqlCommand cmd = new SqlCommand("sp_GenerarReporteNotasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@ID_Gestion", idGestion);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        // Método alternativo para obtener todas las notas (sin filtro de gestión)
        public DataTable ObtenerTodasNotasEstudiante(int idEstudiante)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTodasNotasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }
    }
}