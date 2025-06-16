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
    public class Inscripcion : TcpServiceBase
    {
        private int idEstudiante;
        private int codEd;

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
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "INSCRIBIR_MATERIAS",
                    StudentId = idEstudiante,
                    ListaCodEd = listaCodEd
                };

                var response = SendRequest(request);
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inscribiendo materias: {ex.Message}");
                return false;
            }
        }

        public bool Eliminar()
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "ELIMINAR_INSCRIPCION",
                    StudentId = idEstudiante,
                    CodEd = codEd
                };

                var response = SendRequest(request);
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando inscripción: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarEdicion(int codEdActual, int codEdNueva)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "ACTUALIZAR_EDICION",
                    StudentId = idEstudiante,
                    CodEdActual = codEdActual,
                    CodEdNueva = codEdNueva
                };

                var response = SendRequest(request);
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando edición: {ex.Message}");
                return false;
            }
        }

        public DataTable ObtenerMateriasInscritas(int codGestion)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_MATERIAS_INSCRITAS",
                    StudentId = idEstudiante,
                    CodGestion = codGestion
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo materias inscritas: {ex.Message}");
                return new DataTable();
            }
        }

        public DataTable ObtenerMateriasOfertadas(int codGestion)
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "GET_MATERIAS_OFERTADAS",
                    StudentId = idEstudiante,
                    CodGestion = codGestion
                };

                var response = SendRequest(request);
                return response.Success ? ConvertToDataTable(response.Data) : new DataTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo materias ofertadas: {ex.Message}");
                return new DataTable();
            }
        }
    }
}
