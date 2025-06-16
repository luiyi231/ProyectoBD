using Capa_Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteMateriasNegocio : TcpServiceBase
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

        public DataTable ObtenerPlanesPorCarrera(int idCarrera)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_PLANES_POR_CARRERA",
                    IdCarrera = idCarrera
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo planes por carrera: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable GenerarReporte(int idCarrera, int idPlanEstudio, int idGestion)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GENERAR_REPORTE_MATERIAS_OFERTADAS",
                    IdCarrera = idCarrera,
                    IdPlanEstudio = idPlanEstudio,
                    IdGestion = idGestion
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