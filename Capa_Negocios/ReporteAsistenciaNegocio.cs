using Capa_Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteAsistenciaNegocio : TcpServiceBase
    {
        public DataTable ObtenerCarreras()
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_CARRERAS"
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo carreras: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable ObtenerMateriasPorCarrera(int idCarrera)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_MATERIAS_POR_CARRERA",
                    IdCarrera = idCarrera
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo materias por carrera: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable ObtenerFechasAsistencia(int idMateria, int idGestion)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_FECHAS_ASISTENCIA",
                    IdMateria = idMateria,
                    IdGestion = idGestion
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo fechas de asistencia: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable GenerarReporteAsistencia(int idCarrera, int idMateria, int idGestion, DateTime fecha)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GENERAR_REPORTE_ASISTENCIA",
                    IdCarrera = idCarrera,
                    IdMateria = idMateria,
                    IdGestion = idGestion,
                    Fecha = fecha
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando reporte de asistencia: {ex.Message}");
                return new DataTable();
            }
        }
    }
}