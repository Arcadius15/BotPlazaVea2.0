using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Utils
{
    public static class LoggingService
    {
        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

        public static async Task LogAsync(string mensaje, TipoCodigo svr, Exception exc = null)
        {

            if (exc != null)
            {
                await Append(GetSeverity(svr), GetConsoleColor(svr));
                await Append($" {mensaje + exc.Message}\n", GetConsoleColor(TipoCodigo.ERROR_INFO));
                await WriteToFile(exc.Message);
            }
            else if (svr.Equals(TipoCodigo.LOG))
            {
                await Append(GetSeverity(svr), GetConsoleColor(svr));
                await Append($" Creando {mensaje}...\n", GetConsoleColor(TipoCodigo.NORMAL));
                await DeleteFile();
                await CreateFile();
                await Append(GetSeverity(svr), GetConsoleColor(svr));
                await Append($" {mensaje} creado\n", GetConsoleColor(TipoCodigo.NORMAL));

            }
            else
            {
                await Append(GetSeverity(svr), GetConsoleColor(svr));
                await Append($" {mensaje}\n", GetConsoleColor(TipoCodigo.NORMAL));
            }


        }

        private static async Task Append(string mensaje, ConsoleColor color)
        {
            await Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(mensaje);
            });
        }

        public static string GetSeverity(TipoCodigo svr)
        {
            switch (svr)
            {
                case TipoCodigo.ERROR:
                    return "[ERROR]";
                case TipoCodigo.INFO:
                    return "[INFO]";
                case TipoCodigo.WARN:
                    return "[WARN]";
                case TipoCodigo.DATA:
                    return "[DATA]";
                case TipoCodigo.HEAD:
                    return "[HEADER]";
                default:
                    return "[DEBUG]";
            }
        }

        private static ConsoleColor GetConsoleColor(TipoCodigo severity)
        {
            switch (severity)
            {
                case TipoCodigo.ERROR:
                    return ConsoleColor.Red;
                case TipoCodigo.WARN:
                    return ConsoleColor.Yellow;
                case TipoCodigo.INFO:
                    return ConsoleColor.Green;
                case TipoCodigo.DATA:
                    return ConsoleColor.Blue;
                case TipoCodigo.HEAD:
                    return ConsoleColor.Magenta;
                case TipoCodigo.ERROR_INFO:
                    return ConsoleColor.DarkRed;
                case TipoCodigo.NORMAL:
                    return ConsoleColor.Gray;
                case TipoCodigo.LOG:
                    return ConsoleColor.DarkMagenta;
                default: return ConsoleColor.White;
            }
        }

        private static async Task WriteToFile(string Message)
        {
            await Task.Run(() => {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";


                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }

            });

        }

        private static async Task CreateFile()
        {
            await Task.Run(() => {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            });
        }

        private static async Task DeleteFile()
        {
            await Task.Run(() => {
                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }

            });
        }
    }
}
