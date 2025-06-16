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
    public class Gestion : TcpServiceBase
    {
        public DataTable ObtenerTodas()
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
    }

}
