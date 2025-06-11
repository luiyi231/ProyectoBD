using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Datos
{
    public class gDatos
    {
        private string bd = "Proyecto";
        private string tb, cSel;
        public DataSet ds = new DataSet();
        private string SQL;
        private SqlConnection miCon;
        private SqlDataAdapter adap;

        // Propiedades
        public string TB
        {
            set { if (value != null) tb = value; }
        }

        public string CSel
        {
            set { if (value != null) cSel = value; }
        }

        // Método para obtener la cadena de conexión
        public string ObtenerCadenaConexion()
        {
            return @"Data Source=DESKTOP-QU50UN3;Initial Catalog=" + bd + ";Integrated Security=True;";
        }

        // Método para conectar y generar SQL dinámico de búsqueda
        public void Conectar()
        {
            miCon = new SqlConnection(ObtenerCadenaConexion());
            string crit = "%" + cSel + "%";

            string columnaBusqueda = "nombre"; // Por defecto
            switch (tb)
            {
                case "Persona":
                    columnaBusqueda = "nombre";
                    break;
                case "Materia":
                    columnaBusqueda = "nombre";
                    break;
                case "Facultad":
                    columnaBusqueda = "nombre";
                    break;
                case "Estudiante":
                    columnaBusqueda = "numero_registro";
                    break;
                default:
                    columnaBusqueda = "descripcion";
                    break;
            }

            SQL = "SELECT * FROM " + tb + " WHERE " + columnaBusqueda + " LIKE '" + crit + "'";
        }

        // Método para ejecutar consulta y cargar DataSet
        public void CrearTablaRAM()
        {
            adap = new SqlDataAdapter(SQL, miCon);
            ds = new DataSet();
            adap.Fill(ds, tb);
        }
    }
}
