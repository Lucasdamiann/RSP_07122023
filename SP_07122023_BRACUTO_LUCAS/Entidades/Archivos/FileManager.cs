using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entidades.Files
{

    public static class FileManager
    {
        private static string path;

        static FileManager()
        {
            string directorioParcial = "\\SP_07122023_BRACUTO_LUCAS\\";
            FileManager.path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), directorioParcial);
            FileManager.ValidaExistenciaDeDirectorio();
        }

        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(FileManager.path, nombreArchivo), append))
                {
                    sw.WriteLine(data);
                }
            }
            catch (Exception e)
            {
                throw new FileManagerException("Error al guardar el archivo", e);
            }
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo)
        {
            if (elemento.GetType().IsByRef)
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                string jsonString = JsonSerializer.Serialize(elemento, options);
                FileManager.Guardar(jsonString, nombreArchivo, true);
                return true;
            }
            return false;
        }

        private static void ValidaExistenciaDeDirectorio()
        {
            try
            {
                if (!Directory.Exists(FileManager.path))
                {
                    Directory.CreateDirectory(FileManager.path);
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException("Error al crear el directorio", ex);
            }
        }

        public static void AgregarAlLog()
        {
            string nombreArchivo = $"logs_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt";
            string dataArchivo = "";
            dataArchivo += $"{DateTime.Now} | {Environment.StackTrace}\n";
            try 
            {
                Guardar(dataArchivo, nombreArchivo, false);
            }
            catch(Exception ex)
            {
                throw new FileManagerException($"Error al guardar: {nombreArchivo}", ex);
            }
        }
    }
}
