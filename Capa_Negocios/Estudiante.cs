using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocios
{
    public class Estudiante
    {
        private int idEstudiante;
        private string connectionString;

        public int IdEstudiante
        {
            get => idEstudiante;
            set => idEstudiante = value;
        }

        public Estudiante()
        {
            connectionString = new Capa_Datos.gDatos().ObtenerCadenaConexion();
        }

        public string ObtenerNombreCompleto()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerNombreEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", idEstudiante);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
        }
    }
}
