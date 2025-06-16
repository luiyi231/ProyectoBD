using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Capa_Datos
{
    public class gDatos
    {
        private string serverIP = "10.26.11.194"; // localhost
        private int serverPort = 8888;
        private string tb, cSel;
        public DataSet ds = new DataSet();

        // Propiedades
        public string TB
        {
            set { if (value != null) tb = value; }
        }

        public string CSel
        {
            set { if (value != null) cSel = value; }
        }

        // Método para conectar y buscar
        public void Conectar()
        {
            try
            {
                var request = new DatabaseRequest
                {
                    Operation = "SEARCH",
                    Table = tb,
                    SearchCriteria = cSel
                };

                var response = SendRequest(request);
                if (response.Success && response.Data != null)
                {
                    ds = new DataSet();
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(response.Data.ToString());
                    dt.TableName = tb;
                    ds.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error conectando al servidor: {ex.Message}");
            }
        }

        public void CrearTablaRAM()
        {
            // Este método ya no es necesario ya que la data se obtiene en Conectar()
            // Mantenemos compatibilidad
        }

        public string ObtenerCadenaConexion()
        {
            // Mantenemos este método para compatibilidad, aunque ya no se usa
            return $"Server={serverIP};Port={serverPort}";
        }

        private DatabaseResponse SendRequest(DatabaseRequest request)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIP, serverPort);
                    NetworkStream stream = client.GetStream();

                    // Enviar solicitud
                    string jsonRequest = JsonConvert.SerializeObject(request);
                    byte[] data = Encoding.UTF8.GetBytes(jsonRequest);
                    stream.Write(data, 0, data.Length);

                    // Recibir respuesta
                    byte[] buffer = new byte[8192];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    return JsonConvert.DeserializeObject<DatabaseResponse>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                return new DatabaseResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    // Clases auxiliares para la comunicación
    public class DatabaseRequest
    {
        public string Operation { get; set; }
        public string Table { get; set; }
        public string SearchCriteria { get; set; }
        public int StudentId { get; set; }
        public string ListaCodEd { get; set; }
        public int CodEd { get; set; }
        public int CodEdActual { get; set; }
        public int CodEdNueva { get; set; }
        public int CodGestion { get; set; }
        public int IdCarrera { get; set; }
        public int IdMateria { get; set; }
        public int IdGestion { get; set; }
        public int IdPlanEstudio { get; set; }
        public DateTime Fecha { get; set; }
        public string RU { get; set; }
    }

    public class DatabaseResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string ErrorMessage { get; set; }
    }

    // Clase base para servicios TCP
    public class TcpServiceBase
    {
        protected string serverIP = "10.26.11.194";
        protected int serverPort = 8888;

        protected DatabaseResponse SendRequest(DatabaseRequest request)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.SendTimeout = 30000; // 30 segundos para enviar
                    client.ReceiveTimeout = 30000; // 30 segundos para recibir
                    client.Connect(serverIP, serverPort);
                    NetworkStream stream = client.GetStream();

                    // Enviar solicitud
                    string jsonRequest = JsonConvert.SerializeObject(request);
                    byte[] data = Encoding.UTF8.GetBytes(jsonRequest);
                    stream.Write(data, 0, data.Length);

                    // Recibir respuesta
                    byte[] buffer = new byte[8192];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    return JsonConvert.DeserializeObject<DatabaseResponse>(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                return new DatabaseResponse
                {
                    Success = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}"
                };
            }
        }

        protected DataTable ConvertToDataTable(object data)
        {
            if (data == null) return new DataTable();

            try
            {
                // Primero intenta deserializar directamente como DataTable
                if (data is DataTable dt)
                    return dt;

                // Si no, intenta deserializar desde JSON
                return JsonConvert.DeserializeObject<DataTable>(data.ToString());
            }
            catch
            {
                return new DataTable();
            }
        }
    }
}