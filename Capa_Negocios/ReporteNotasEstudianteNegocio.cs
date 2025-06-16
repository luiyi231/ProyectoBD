using Capa_Datos;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Negocios
{
    public class ReporteNotasEstudianteNegocio : TcpServiceBase
    {
        public DataTable ObtenerGestiones()
        {
            var request = new DatabaseRequest
            {
                Operation = "GET_GESTIONES"
            };

            var response = SendRequest(request);
            return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
        }

        public DataTable BuscarEstudiantePorRU(string ru)
        {
            var request = new DatabaseRequest
            {
                Operation = "BUSCAR_ESTUDIANTE_POR_RU",
                RU = ru
            };

            var response = SendRequest(request);
            return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
        }

        public DataTable ObtenerEstadisticasEstudiante(int idEstudiante, int idGestion)
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

        public bool ExisteEstudiante(string ru)
        {
            var request = new DatabaseRequest
            {
                Operation = "EXISTE_ESTUDIANTE",
                RU = ru
            };

            var response = SendRequest(request);
            if (response.Success && response.Data != null)
            {
                return Convert.ToBoolean(response.Data);
            }
            return false;
        }

        public DataTable GenerarReporteNotas(int idEstudiante, int idGestion)
        {
            var request = new DatabaseRequest
            {
                Operation = "GENERAR_REPORTE_NOTAS",
                StudentId = idEstudiante,
                IdGestion = idGestion
            };

            var response = SendRequest(request);
            return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
        }

        public DataTable ObtenerTodasNotasEstudiante(int idEstudiante)
        {
            var request = new DatabaseRequest
            {
                Operation = "OBTENER_TODAS_NOTAS_ESTUDIANTE",
                StudentId = idEstudiante
            };

            var response = SendRequest(request);
            return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
        }

        public DataTable GenerarReporteMaterias(int idEstudiante)
        {
            var request = new DatabaseRequest
            {
                Operation = "GENERAR_REPORTE_MATERIAS_ESTUDIANTE",
                StudentId = idEstudiante
            };

            var response = SendRequest(request);
            return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
        }
    }
}