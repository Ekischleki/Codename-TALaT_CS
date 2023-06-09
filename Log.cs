using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Codename_TALaT_CS
{
    internal class Log
    {
        private static Log? shared;
        private bool enableLogger;
        private StreamWriter logWriter;

        public static Log Shared
        {
            get
            {
                return shared ?? throw new Exception("Logger.Not_Created: The shared logger has not been created");
            }
            set
            {
                if (shared != null) throw new Exception("Logger.Already_Created: The shared logger has already been created.");
                shared = value;
            }
        }
        public Log(string logPath, bool enableLogger)
        {
            this.enableLogger = enableLogger;
            logWriter = new(logPath);
        }
        public void LogL(string message)
        {
            if (!enableLogger) return;
            Console.Clear();
            Console.WriteLine($"Please wait while\n{message}");
            logWriter.WriteLine(message);
        }
        public void LogN(string message)
        {
            if (!enableLogger) return;

            logWriter.WriteLine(message);
        }
        public void LogE(string message)
        {
            if (!enableLogger) return;
            Console.Clear();
            Console.WriteLine(message);
            logWriter.WriteLine(message);
        }
    }
}
