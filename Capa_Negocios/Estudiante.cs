using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;

namespace Capa_Negocios
{
    public class Estudiante : TcpServiceBase
    {
        private int idEstudiante;

        public int IdEstudiante
        {
            get => idEstudiante;
            set => idEstudiante = value;
        }

        public string ObtenerNombreCompleto()
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_STUDENT_NAME",
                    StudentId = idEstudiante
                };

                var response = SendRequest(request);
                return response.Success ? response.Data?.ToString() ?? "" : "";
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                System.Diagnostics.Debug.WriteLine($"Error obteniendo nombre del estudiante: {ex.Message}");
                return "";
            }
        }
    }
}
