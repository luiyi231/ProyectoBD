using Capa_Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteNotasEstudianteNegocio : TcpServiceBase
    {
        /// <summary>
        /// Obtiene todas las gestiones disponibles
        /// </summary>
        /// <returns>DataTable con las gestiones</returns>
        public DataTable ObtenerGestiones()
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_GESTIONES"
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo gestiones: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Busca un estudiante por su número de registro (RU)
        /// </summary>
        /// <param name="ru">Número de registro del estudiante</param>
        /// <returns>DataTable con la información del estudiante</returns>
        public DataTable BuscarEstudiantePorRU(string ru)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "BUSCAR_ESTUDIANTE_POR_RU",
                    RU = ru
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error buscando estudiante por RU: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Obtiene las estadísticas generales de un estudiante en una gestión
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <param name="idGestion">ID de la gestión</param>
        /// <returns>DataTable con las estadísticas</returns>
        public DataTable ObtenerEstadisticasEstudiante(int idEstudiante, int idGestion)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "OBTENER_ESTADISTICAS_ESTUDIANTE",
                    StudentId = idEstudiante,
                    IdGestion = idGestion
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo estadísticas del estudiante: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Verifica si un estudiante existe por su RU
        /// </summary>
        /// <param name="ru">Número de registro del estudiante</param>
        /// <returns>True si existe, False si no existe</returns>
        public bool ExisteEstudiante(string ru)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "EXISTE_ESTUDIANTE",
                    RU = ru
                };

                var response = SendRequest(request);
                if (response.Success && response.Data != null)
                {
                    // Intentar convertir la respuesta a booleano
                    if (bool.TryParse(response.Data.ToString(), out bool existe))
                        return existe;

                    // Si no es booleano, intentar convertir a entero (1 = existe, 0 = no existe)
                    if (int.TryParse(response.Data.ToString(), out int count))
                        return count > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando existencia del estudiante: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Genera el reporte de notas para un estudiante en una gestión específica
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <param name="idGestion">ID de la gestión</param>
        /// <returns>DataTable con el reporte de notas</returns>
        public DataTable GenerarReporteNotas(int idEstudiante, int idGestion)
        {
            try
            {
                // Primero hacer una verificación de debug si es necesario
                var debugRequest = new DatabaseRequest
                {
                    Operation = "VERIFICAR_DATOS_ESTUDIANTE",
                    StudentId = idEstudiante,
                    IdGestion = idGestion
                };

                try
                {
                    SendRequest(debugRequest);
                }
                catch (Exception debugEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en debug: {debugEx.Message}");
                }

                // Ahora ejecutar el reporte principal
                var request = new DatabaseRequest
                {
                    Operation = "GENERAR_REPORTE_NOTAS_ESTUDIANTE",
                    StudentId = idEstudiante,
                    IdGestion = idGestion
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando reporte de notas: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Método alternativo para obtener todas las notas (sin filtro de gestión)
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <returns>DataTable con todas las notas del estudiante</returns>
        public DataTable ObtenerTodasNotasEstudiante(int idEstudiante)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "OBTENER_TODAS_NOTAS_ESTUDIANTE",
                    StudentId = idEstudiante
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo todas las notas del estudiante: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Genera el reporte de materias para un estudiante
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante</param>
        /// <returns>DataTable con el reporte de materias</returns>
        public DataTable GenerarReporteMaterias(int idEstudiante)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GENERAR_REPORTE_MATERIAS_ESTUDIANTE",
                    StudentId = idEstudiante
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando reporte de materias: {ex.Message}");
                return new DataTable();
            }
        }
    }
}