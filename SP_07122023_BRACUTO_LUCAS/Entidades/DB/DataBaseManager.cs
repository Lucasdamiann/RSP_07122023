using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static SqlConnection connection;
        private static string stringConnection;

        static DataBaseManager()
        {
            DataBaseManager.stringConnection = "Server=.;Database=20230622SP;Trusted_Connection=True;";
        }

        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DataBaseManager.stringConnection))
                {
                    string query = "SELECT imagen FROM comidas WHERE tipo_comida = @tipo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("tipo", tipo);
                    connection.Open();
                    string retorno = string.Empty;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            retorno = reader["imagen"].ToString();
                        }
                        else
                        {
                            throw new ComidaInvalidaExeption("Comida invalida");
                        }
                    }
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                throw new DataBaseManagerException("Error al leer archivo", ex);
            }
        }

        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible
        {
            if (comida.GetType().IsInterface && comida.GetType().IsPublic)
            {
                try
                {
                    using (DataBaseManager.connection = new SqlConnection(DataBaseManager.stringConnection))
                    {
                        string query = "INSERT[dbo].[comidas]([empleado], [ticket]) VALUES(nombre, ticket)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("nombreEmpleado", nombreEmpleado);
                        command.Parameters.AddWithValue("ticket", comida);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new DataBaseManagerException("Error al escribir datos", ex);
                }
            }
            return false;
        }
    }
}
